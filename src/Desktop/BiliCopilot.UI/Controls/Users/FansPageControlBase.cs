// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Users;

/// <summary>
/// 粉丝页面控件基类.
/// </summary>
public abstract class FansPageControlBase : LayoutUserControlBase<FansPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FansPageControlBase"/> class.
    /// </summary>
    protected FansPageControlBase() => ViewModel = this.Get<FansPageViewModel>();
}
