// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Moment;

/// <summary>
/// 动态用户条目.
/// </summary>
public sealed partial class MomentUperItemControl : MomentUperSectionControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MomentUperItemControl"/> class.
    /// </summary>
    public MomentUperItemControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);
}
