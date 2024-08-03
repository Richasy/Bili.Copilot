// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 直播卡片控件.
/// </summary>
public sealed class LiveCardControl : LayoutControlBase<LiveItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveCardControl"/> class.
    /// </summary>
    public LiveCardControl()
    {
        DefaultStyleKey = typeof(LiveCardControl);
    }
}
