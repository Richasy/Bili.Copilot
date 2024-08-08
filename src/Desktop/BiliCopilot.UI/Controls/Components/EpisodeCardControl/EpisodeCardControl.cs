// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 单集卡片控件.
/// </summary>
public sealed class EpisodeCardControl : LayoutControlBase<EpisodeItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EpisodeCardControl"/> class.
    /// </summary>
    public EpisodeCardControl()
    {
        DefaultStyleKey = typeof(EpisodeCardControl);
    }
}
