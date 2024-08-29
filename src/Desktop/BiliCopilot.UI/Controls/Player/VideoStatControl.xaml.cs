// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// 视频统计区域.
/// </summary>
public sealed partial class VideoStatControl : VideoPlayerPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoStatControl"/> class.
    /// </summary>
    public VideoStatControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new(Bindings.Initialize, Bindings.StopTracking);
}
