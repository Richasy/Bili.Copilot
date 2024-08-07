// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Moment;

/// <summary>
/// 综合动态分区详情控件.
/// </summary>
public sealed partial class ComprehensiveMomentSectionDetailControl : ComprehensiveSectionControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComprehensiveMomentSectionDetailControl"/> class.
    /// </summary>
    public ComprehensiveMomentSectionDetailControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);
}
