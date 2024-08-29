// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Pgc;

/// <summary>
/// 动漫时间轴主控件.
/// </summary>
public sealed partial class AnimeTimelineMainControl : AnimeTimelineControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnimeTimelineMainControl"/> class.
    /// </summary>
    public AnimeTimelineMainControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);
}
