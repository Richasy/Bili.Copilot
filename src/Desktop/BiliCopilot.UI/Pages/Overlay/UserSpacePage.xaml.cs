// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.BiliKernel.Models.User;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Pages.Overlay;

/// <summary>
/// 用户空间页面.
/// </summary>
public sealed partial class UserSpacePage : UserSpacePageBase, IParameterPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserSpacePage"/> class.
    /// </summary>
    public UserSpacePage() => InitializeComponent();

    public void SetParameter(object? parameter)
    {
        if (parameter is UserProfile profile)
        {
            ViewModel.InitializeCommand.Execute(profile);
        }
    }
}

/// <summary>
/// 用户空间页面基类.
/// </summary>
public abstract class UserSpacePageBase : LayoutPageBase<UserSpacePageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserSpacePageBase"/> class.
    /// </summary>
    protected UserSpacePageBase()
    {
        ViewModel = this.Get<UserSpacePageViewModel>();
    }
}
