// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Pages;

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
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is MyFollowsDetailViewModel vm)
        {
            ViewModel = vm;
        }
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        CoreViewModel.BackRequest += OnBackRequest;
        ViewModel.InitializeCommand.Execute(default);
    }

    /// <inheritdoc/>
    protected override void OnPageUnloaded()
        => CoreViewModel.BackRequest -= OnBackRequest;

    private void OnBackRequest(object sender, EventArgs e)
        => ViewModel.ResetStateCommand.Execute(default);
}

/// <summary>
/// 关注页面基类.
/// </summary>
public abstract class FollowsPageBase : PageBase<MyFollowsDetailViewModel>
{
}
