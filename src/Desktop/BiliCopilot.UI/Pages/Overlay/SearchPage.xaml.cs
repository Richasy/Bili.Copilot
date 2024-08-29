// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Microsoft.UI.Xaml.Navigation;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Pages.Overlay;

/// <summary>
/// 搜索页面.
/// </summary>
public sealed partial class SearchPage : SearchPageBase
{
    private string _keyword = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchPage"/> class.
    /// </summary>
    public SearchPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.NavigationMode == NavigationMode.Back)
        {
            return;
        }

        if (e.Parameter is string keyword)
        {
            _keyword = keyword;
        }
    }

    /// <inheritdoc/>
    protected override void OnNavigatedFrom(NavigationEventArgs e)
        => _keyword = string.Empty;

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        if (!string.IsNullOrEmpty(_keyword))
        {
            ViewModel.SearchCommand.Execute(_keyword);
        }
    }
}

/// <summary>
/// 搜索页面基类.
/// </summary>
public abstract class SearchPageBase : LayoutPageBase<SearchPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchPageBase"/> class.
    /// </summary>
    protected SearchPageBase() => ViewModel = this.Get<SearchPageViewModel>();
}
