// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// 视频操作区域.
/// </summary>
public sealed partial class VideoOperationControl : VideoPlayerPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoOperationControl"/> class.
    /// </summary>
    public VideoOperationControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new(Bindings.Initialize, Bindings.StopTracking);
}
