// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// PGC 播放器页面剧集区域.
/// </summary>
public sealed partial class PgcSeasonsSection : PgcSeasonsSectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgcSeasonsSection"/> class.
    /// </summary>
    public PgcSeasonsSection() => InitializeComponent();
}

/// <summary>
/// PGC 播放器页面剧集区域基类.
/// </summary>
public abstract class PgcSeasonsSectionBase : LayoutUserControlBase<PgcPlayerSeasonSectionDetailViewModel>
{
}
