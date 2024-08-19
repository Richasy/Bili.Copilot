// Copyright (c) Bili Copilot. All rights reserved.

using System.Text.RegularExpressions;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media.Imaging;
using Richasy.BiliKernel.Models.Appearance;
using Richasy.WinUI.Share.Base;
using Windows.System;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 表情富文本.
/// </summary>
public sealed partial class EmoteTextBlock : LayoutUserControlBase
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

    /// <summary>
    /// Initializes a new instance of the <see cref="EmoteTextBlock"/> class.
    /// </summary>
    public EmoteTextBlock() => InitializeComponent();

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

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as EmoteTextBlock;
        if (e.NewValue != null)
        {
            instance.InitializeContent();
        }
    }

    private void InitializeContent()
    {
        if (RichBlock != null)
        {
            RichBlock.Blocks.Clear();
            Paragraph para = null;
            if (Text != null)
            {
                para = ParseText();
            }

            if (para != null)
            {
                RichBlock.Blocks.Add(para);
            }

            CheckTextTrim();
        }

        if (Text.Pictures?.Count > 0)
        {
            var gallery = FindName("Gallery") as ItemsRepeater;
            gallery.ItemsSource = Text.Pictures;
            gallery.Visibility = Visibility.Visible;
        }
        else if (Gallery is not null)
        {
            Gallery.ItemsSource = default;
            Gallery.Visibility = Visibility.Collapsed;
        }
    }

    private void OnTextTrimChanged(RichTextBlock sender, IsTextTrimmedChangedEventArgs args)
        => CheckTextTrim();

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
                    if (emoji != null && emoji.Uri is not null)
                    {
                        var inlineCon = new InlineUIContainer();
                        var img = new Image() { Width = 20, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(2, 0, 2, -4) };
                        var bitmap = new BitmapImage(emoji.Uri) { DecodePixelWidth = 40 };
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

    private void CheckTextTrim()
    {
        if (RichBlock.IsTextTrimmed)
        {
            if (Tip.IsEnabled)
            {
                return;
            }

            var richBlock = new RichTextBlock()
            {
                MaxWidth = 500,
                IsTextSelectionEnabled = true,
                LineHeight = 24,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
            };

            var para = ParseText();
            richBlock.Blocks.Add(para);
            Tip.Content = richBlock;
            Tip.IsEnabled = true;
        }
        else
        {
            if (Tip.Content is not null)
            {
                Tip.Content = default;
            }

            Tip.IsEnabled = false;
        }
    }

    private async void OnImageTappedAsync(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        var uri = (sender as Image).Tag as Uri;
        if (uri is not null)
        {
            await Launcher.LaunchUriAsync(uri).AsTask();
        }
    }
}
