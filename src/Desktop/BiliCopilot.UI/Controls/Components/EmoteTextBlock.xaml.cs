// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Forms;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media.Imaging;
using Richasy.BiliKernel.Models.Appearance;
using Richasy.WinUIKernel.Share.Base;
using System.Text.RegularExpressions;
using System.Windows.Input;

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
    /// <see cref="SeekCommand"/> 的依赖属性.
    /// </summary>
    /// <remarks>
    /// 用于把正文里的 <c>mm:ss</c> 时间戳渲染成可点击链接，参数为跳转秒数.
    /// </remarks>
    public static readonly DependencyProperty SeekCommandProperty =
        DependencyProperty.Register(nameof(SeekCommand), typeof(ICommand), typeof(EmoteTextBlock), new PropertyMetadata(default));

    private static readonly Regex TimeStampRegex = new(@"(?<!\d)(\d{1,2}):([0-5]\d)(?::([0-5]\d))?(?!\d)", RegexOptions.Compiled);

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

    /// <summary>
    /// 时间戳点击后执行跳转的命令，参数为秒数.
    /// </summary>
    public ICommand? SeekCommand
    {
        get => (ICommand?)GetValue(SeekCommandProperty);
        set => SetValue(SeekCommandProperty, value);
    }

    protected override void OnControlUnloaded()
        => Gallery?.ItemsSource = default;

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as EmoteTextBlock;
        if (e.NewValue != null)
        {
            instance.InitializeContent();
        }

        instance.FlyoutRichBlock?.Blocks?.Clear();
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
                RichBlock.Visibility = Visibility.Visible;
                RichBlock.Blocks.Add(para);
            }
            else
            {
                RichBlock.Visibility = Visibility.Collapsed;
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

    private Paragraph? ParseText()
    {
        if (string.IsNullOrEmpty(Text.Text))
        {
            return default;
        }

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
                        AppendInlineText(para, content);
                    }
                }
                else
                {
                    AppendInlineText(para, content);
                }
            }
        }
        else
        {
            AppendInlineText(para, text);
        }

        return para;
    }

    /// <summary>
    /// 判断文本是否含有可跳转的时间戳.
    /// </summary>
    /// <param name="content">评论文本.</param>
    /// <returns>是否包含时间戳.</returns>
    internal static bool ContainsTimeStamp(string? content)
        => !string.IsNullOrEmpty(content) && TimeStampRegex.IsMatch(content);

    /// <summary>
    /// 把形如 <c>01:23</c> / <c>1:02:03</c> 的时间戳解析为秒数.
    /// </summary>
    /// <param name="match">正则匹配结果.</param>
    /// <returns>秒数，无法解析时返回 <see cref="double.NaN"/>.</returns>
    private static double ParseTimeStamp(Match match)
    {
        if (!int.TryParse(match.Groups[1].Value, out var first)
            || !int.TryParse(match.Groups[2].Value, out var second))
        {
            return double.NaN;
        }

        if (match.Groups[3].Success)
        {
            return int.TryParse(match.Groups[3].Value, out var third)
                ? (first * 3600d) + (second * 60d) + third
                : double.NaN;
        }

        return (first * 60d) + second;
    }

    /// <summary>
    /// 把一段纯文本写入段落，其中的时间戳渲染为可点击链接.
    /// </summary>
    /// <param name="para">目标段落.</param>
    /// <param name="content">文本内容.</param>
    private void AppendInlineText(Paragraph para, string content)
    {
        if (SeekCommand is null || string.IsNullOrEmpty(content))
        {
            para.Inlines.Add(new Run { Text = content });
            return;
        }

        var lastIndex = 0;
        foreach (Match match in TimeStampRegex.Matches(content))
        {
            var seconds = ParseTimeStamp(match);
            if (double.IsNaN(seconds) || seconds <= 0)
            {
                continue;
            }

            if (match.Index > lastIndex)
            {
                para.Inlines.Add(new Run { Text = content[lastIndex..match.Index] });
            }

            var link = new Hyperlink();
            link.Inlines.Add(new Run { Text = match.Value });
            link.Click += (_, _) =>
            {
                if (SeekCommand?.CanExecute(seconds) == true)
                {
                    SeekCommand.Execute(seconds);
                }
            };

            para.Inlines.Add(link);
            lastIndex = match.Index + match.Length;
        }

        if (lastIndex < content.Length)
        {
            para.Inlines.Add(new Run { Text = content[lastIndex..] });
        }
    }

    private void CheckTextTrim()
    {
        OverflowButton.Visibility = RichBlock.IsTextTrimmed ? Visibility.Visible : Visibility.Collapsed;
    }

    private void OnImageTapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        var image = (sender as FrameworkElement).DataContext as BiliImage;
        if (image is not null)
        {
            new GalleryWindow(image, [.. Text.Pictures]).Activate();
        }
    }

    private void OnOverflowFlyoutOpened(object sender, object e)
    {
        if (FlyoutRichBlock.Blocks.Count > 0)
        {
            return;
        }

        var para = ParseText();
        if (para != null)
        {
            FlyoutRichBlock.Blocks.Add(para);
        }
    }
}
