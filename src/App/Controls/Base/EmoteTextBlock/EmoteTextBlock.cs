﻿// Copyright (c) Bili Copilot. All rights reserved.

using System.Text.RegularExpressions;
using Bili.Copilot.Models.Data.Appearance;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media.Imaging;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 带表情的文本.
/// </summary>
public sealed class EmoteTextBlock : Control
{
    /// <summary>
    /// <see cref="MaxLines"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty MaxLinesProperty =
        DependencyProperty.Register(nameof(MaxLines), typeof(int), typeof(EmoteTextBlock), new PropertyMetadata(4));

    /// <summary>
    /// <see cref="Text"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(EmoteText), typeof(EmoteTextBlock), new PropertyMetadata(default, new PropertyChangedCallback(OnTextChanged)));

    private RichTextBlock _richBlock;
    private RichTextBlock _flyoutRichBlock;
    private DynamicImageItem _picturePanel;
    private Button _overflowButton;
    private bool _isOverflowInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmoteTextBlock"/> class.
    /// </summary>
    public EmoteTextBlock() => DefaultStyleKey = typeof(EmoteTextBlock);

    /// <summary>
    /// 最大行数.
    /// </summary>
    public int MaxLines
    {
        get => (int)GetValue(MaxLinesProperty);
        set => SetValue(MaxLinesProperty, value);
    }

    /// <summary>
    /// 输入的文本.
    /// </summary>
    public EmoteText Text
    {
        get => (EmoteText)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// 重置状态.
    /// </summary>
    public void Reset() => _richBlock?.Blocks?.Clear();

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _richBlock = GetTemplateChild("RichBlock") as RichTextBlock;
        _flyoutRichBlock = GetTemplateChild("FlyoutRichBlock") as RichTextBlock;
        _picturePanel = GetTemplateChild("PicturePanel") as DynamicImageItem;
        _overflowButton = GetTemplateChild("OverflowButton") as Button;

        _richBlock.IsTextTrimmedChanged += OnIsTextTrimmedChanged;
        _overflowButton.Click += OnOverflowButtonClick;

        InitializeContent();
        base.OnApplyTemplate();
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as EmoteTextBlock;
        if (e.NewValue != null)
        {
            instance.InitializeContent();
        }
    }

    private void OnIsTextTrimmedChanged(RichTextBlock sender, IsTextTrimmedChangedEventArgs args)
        => _overflowButton.Visibility = sender.IsTextTrimmed ? Visibility.Visible : Visibility.Collapsed;

    private void InitializeContent()
    {
        _isOverflowInitialized = false;
        if (_richBlock != null)
        {
            _richBlock.Blocks.Clear();
            Paragraph para = null;
            if (Text != null)
            {
                para = ParseText();
                _picturePanel.ItemsSource = Text?.Pictures;
                _picturePanel.Visibility = Text.Pictures != null && Text.Pictures.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }

            if (para != null)
            {
                _richBlock.Blocks.Add(para);
            }
        }

        if (_overflowButton != null && _richBlock != null)
        {
            _overflowButton.Visibility = _richBlock.IsTextTrimmed ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private void OnOverflowButtonClick(object sender, RoutedEventArgs e)
    {
        if (!_isOverflowInitialized)
        {
            _flyoutRichBlock.Blocks.Clear();

            if (Text != null)
            {
                var para = ParseText();
                _flyoutRichBlock.Blocks.Add(para);
            }
        }
    }

    private void OnImageTapped(object sender, TappedRoutedEventArgs e)
    {
        var image = sender as ImageEx;
        var sources = Text.Pictures;
        var index = sources.ToList().IndexOf(image.DataContext as Models.Data.Appearance.Image);
        AppViewModel.Instance.ShowImagesCommand.Execute(new Models.App.Args.ShowImageEventArgs(sources, index));
    }

    private Paragraph ParseText()
    {
        var text = Text.Text;
        var emotes = Text.Emotes;
        var para = new Paragraph();

        if (emotes != null && emotes.Count > 0)
        {
            // 有表情存在，进行处理.
            var emojiRegex = new Regex(@"(\[.*?\])");
            var splitContents = emojiRegex.Split(text).Where(p => p.Length > 0).ToArray();
            foreach (var content in splitContents)
            {
                if (emojiRegex.IsMatch(content))
                {
                    _ = emotes.TryGetValue(content, out var emoji);
                    if (emoji != null && !string.IsNullOrEmpty(emoji.Uri))
                    {
                        var inlineCon = new InlineUIContainer();
                        var img = new Microsoft.UI.Xaml.Controls.Image() { Width = 20, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(2, 0, 2, -4) };
                        var bitmap = new BitmapImage(new Uri(emoji.Uri)) { DecodePixelWidth = 40 };
                        img.Source = bitmap;
                        inlineCon.Child = img;
                        para.Inlines.Add(inlineCon);
                    }
                    else
                    {
                        para.Inlines.Add(new Run { Text = content });
                    }
                }
                else
                {
                    para.Inlines.Add(new Run { Text = content });
                }
            }
        }
        else
        {
            para.Inlines.Add(new Run { Text = text });
        }

        return para;
    }
}
