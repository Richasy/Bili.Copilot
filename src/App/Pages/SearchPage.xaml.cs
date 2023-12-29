// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 搜索页.
/// </summary>
public sealed partial class SearchPage : SearchPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchPage"/> class.
    /// </summary>
    public SearchPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is SearchDetailViewModel vm)
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
/// 搜索页基类.
/// </summary>
public abstract class SearchPageBase : PageBase<SearchDetailViewModel>
{
}
