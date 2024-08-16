// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls.Danmaku;

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
        DependencyProperty.Register(nameof(TransportControls), typeof(PlayerControlBase), typeof(MpvPlayer), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="DanmakuControls"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty DanmakuControlsProperty =
        DependencyProperty.Register(nameof(DanmakuControls), typeof(VideoDanmakuPanel), typeof(MpvPlayer), new PropertyMetadata(default));

    /// <summary>
    /// 媒体传输控件.
    /// </summary>
    public PlayerControlBase TransportControls
    {
        get => (PlayerControlBase)GetValue(TransportControlsProperty);
        set => SetValue(TransportControlsProperty, value);
    }

    /// <summary>
    /// 弹幕控件.
    /// </summary>
    public VideoDanmakuPanel DanmakuControls
    {
        get => (VideoDanmakuPanel)GetValue(DanmakuControlsProperty);
        set => SetValue(DanmakuControlsProperty, value);
    }
}
