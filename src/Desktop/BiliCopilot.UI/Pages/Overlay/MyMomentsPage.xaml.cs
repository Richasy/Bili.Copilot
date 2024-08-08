// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Pages.Overlay;

/// <summary>
/// 我的动态页面.
/// </summary>
public sealed partial class MyMomentsPage : MyMomentsPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MyMomentsPage"/> class.
    /// </summary>
    public MyMomentsPage()
    {
        this.InitializeComponent();
    }
}

/// <summary>
/// 我的动态页面基类.
/// </summary>
public abstract class MyMomentsPageBase : LayoutPageBase<MyMomentsPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MyMomentsPageBase"/> class.
    /// </summary>
    protected MyMomentsPageBase() => ViewModel = this.Get<MyMomentsPageViewModel>();
}
