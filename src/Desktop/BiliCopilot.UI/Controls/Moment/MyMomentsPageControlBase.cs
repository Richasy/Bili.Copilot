// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Moment;

/// <summary>
/// 我的动态页面控件基类.
/// </summary>
public abstract class MyMomentsPageControlBase : LayoutUserControlBase<MyMomentsPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MyMomentsPageControlBase"/> class.
    /// </summary>
    protected MyMomentsPageControlBase() => ViewModel = this.Get<MyMomentsPageViewModel>();
}
