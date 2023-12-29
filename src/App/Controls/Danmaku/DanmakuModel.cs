// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.App;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Bili.Copilot.App.Controls.Danmaku;

/// <summary>
/// 弹幕模型.
/// </summary>
public class DanmakuModel
{
    /// <summary>
    /// 文本.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// 弹幕大小.
    /// </summary>
    public double Size { get; set; }

    /// <summary>
    /// 弹幕颜色.
    /// </summary>
    public Color Color { get; set; }

    /// <summary>
    /// 弹幕出现时间.
    /// </summary>
    public int Time { get; set; }

    /// <summary>
    /// 弹幕ID.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 弹幕出现位置.
    /// </summary>
    public DanmakuLocation Location { get; set; }

    /// <summary>
    /// 前景色.
    /// </summary>
    public SolidColorBrush Foreground => new(Color);

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is DanmakuModel model && Text == model.Text;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Text);
}
