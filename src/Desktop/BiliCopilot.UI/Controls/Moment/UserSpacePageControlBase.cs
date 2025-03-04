// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Moment;

/// <summary>
/// 用户空间页面控件基类.
/// </summary>
public abstract class UserSpacePageControlBase : LayoutUserControlBase<UserSpacePageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserSpacePageControlBase"/> class.
    /// </summary>
    protected UserSpacePageControlBase()
        => ViewModel = this.Get<UserSpacePageViewModel>();
}
