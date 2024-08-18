// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls.Danmaku;
using BiliCopilot.UI.Controls.Player;

namespace BiliCopilot.UI.Controls.Mpv;

/// <summary>
/// 播放器.
/// </summary>
public sealed partial class MpvPlayer
{
    /// <summary>
    /// <see cref="TransportControls"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TransportControlsProperty =
        DependencyProperty.Register(nameof(TransportControls), typeof(FrameworkElement), typeof(MpvPlayer), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="SubtitleControls"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty SubtitleControlsProperty =
        DependencyProperty.Register(nameof(SubtitleControls), typeof(SubtitlePresenter), typeof(MpvPlayer), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="DanmakuControls"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty DanmakuControlsProperty =
        DependencyProperty.Register(nameof(DanmakuControls), typeof(DanmakuControlBase), typeof(MpvPlayer), new PropertyMetadata(default));

    /// <summary>
    /// 媒体传输控件.
    /// </summary>
    public FrameworkElement TransportControls
    {
        get => (FrameworkElement)GetValue(TransportControlsProperty);
        set => SetValue(TransportControlsProperty, value);
    }

    /// <summary>
    /// 弹幕控件.
    /// </summary>
    public DanmakuControlBase DanmakuControls
    {
        get => (DanmakuControlBase)GetValue(DanmakuControlsProperty);
        set => SetValue(DanmakuControlsProperty, value);
    }

    /// <summary>
    /// 字幕控件.
    /// </summary>
    public SubtitlePresenter SubtitleControls
    {
        get => (SubtitlePresenter)GetValue(SubtitleControlsProperty);
        set => SetValue(SubtitleControlsProperty, value);
    }
}
