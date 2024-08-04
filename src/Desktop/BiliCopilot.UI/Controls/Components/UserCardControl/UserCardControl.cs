// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 用户卡片.
/// </summary>
public sealed class UserCardControl : LayoutControlBase<UserItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserCardControl"/> class.
    /// </summary>
    public UserCardControl()
    {
        DefaultStyleKey = typeof(UserCardControl);
    }
}
