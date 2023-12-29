// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 粉丝页面.
/// </summary>
public sealed partial class FansPage : FansPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FansPage"/> class.
    /// </summary>
    public FansPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is FansDetailViewModel vm)
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
/// 粉丝页面基类.
/// </summary>
public abstract class FansPageBase : PageBase<FansDetailViewModel>
{
}
