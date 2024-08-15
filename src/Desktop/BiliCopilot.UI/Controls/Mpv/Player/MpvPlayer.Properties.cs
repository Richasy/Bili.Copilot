// Copyright (c) Bili Copilot. All rights reserved.

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
        DependencyProperty.Register(nameof(TransportControls), typeof(MpvTransportControl), typeof(MpvPlayer), new PropertyMetadata(default));

    /// <summary>
    /// 媒体传输控件.
    /// </summary>
    public MpvTransportControl TransportControls
    {
        get => (MpvTransportControl)GetValue(TransportControlsProperty);
        set => SetValue(TransportControlsProperty, value);
    }
}
