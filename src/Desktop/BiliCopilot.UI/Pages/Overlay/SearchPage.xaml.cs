// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Pages.Overlay;

/// <summary>
/// 搜索页面.
/// </summary>
public sealed partial class SearchPage : SearchPageBase, IParameterPage
{
    private string _keyword = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchPage"/> class.
    /// </summary>
    public SearchPage() => InitializeComponent();

    public void SetParameter(object? parameter)
    {
        if (parameter is string keyword)
        {
            _keyword = keyword;
        }
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        if (!string.IsNullOrEmpty(_keyword))
        {
            ViewModel.SearchCommand.Execute(_keyword);
        }
    }

    protected override void OnPageUnloaded()
    {
        _keyword = string.Empty;
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
