// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 剧集卡片控件.
/// </summary>
public sealed class SeasonCardControl : LayoutControlBase<SeasonItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeasonCardControl"/> class.
    /// </summary>
    public SeasonCardControl()
    {
        DefaultStyleKey = typeof(SeasonCardControl);
    }
}
