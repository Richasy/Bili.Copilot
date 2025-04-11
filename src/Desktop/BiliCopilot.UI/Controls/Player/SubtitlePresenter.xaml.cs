// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// 字幕显示器.
/// </summary>
public sealed partial class SubtitlePresenter : SubtitleControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubtitlePresenter"/> class.
    /// </summary>
    public SubtitlePresenter() => InitializeComponent();

    public void ArrangeSubtitleSize(double windowWidth)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            const int initialWidth = 1280;
            const int initialFontSize = 28;
            const int initialPaddingLeft = 16;
            const int initialPaddingTop = 12;

            // 根据实际宽度调整字幕大小，线性增长，每增加100px，字体大小增加3，横向边距增加8，纵向边距增加6.
            var width = ActualWidth;
            FontSize = Math.Max(12, (double)(initialFontSize + ((width - initialWidth) / 100 * 1.5)));
            var horizontalPadding = Math.Max(8, initialPaddingLeft + ((width - initialWidth) / 100 * 2.5));
            var verticalPadding = Math.Max(8, initialPaddingTop + ((width - initialWidth) / 100 * 1));
            Padding = new Thickness(horizontalPadding, verticalPadding, horizontalPadding, verticalPadding);
        });
    }
}
