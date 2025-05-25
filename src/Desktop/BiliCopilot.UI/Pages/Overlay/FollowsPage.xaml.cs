// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Pages.Overlay;

/// <summary>
/// 关注页面.
/// </summary>
public sealed partial class FollowsPage : FollowsPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FollowsPage"/> class.
    /// </summary>
    public FollowsPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded() => ViewModel.InitializeCommand.Execute(default);
}

/// <summary>
/// 关注页面基类.
/// </summary>
public abstract class FollowsPageBase : LayoutPageBase<FollowsPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FollowsPageBase"/> class.
    /// </summary>
    protected FollowsPageBase() => ViewModel = this.Get<FollowsPageViewModel>();
}
