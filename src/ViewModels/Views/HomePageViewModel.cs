// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 首页视图模型.
/// </summary>
public sealed partial class HomePageViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _isHomeShown;

    /// <summary>
    /// 是否处于搜索状态.
    /// </summary>
    [ObservableProperty]
    private bool _isInSearch;

    [ObservableProperty]
    private SearchDetailViewModel _search;

    /// <summary>
    /// Initializes a new instance of the <see cref="HomePageViewModel"/> class.
    /// </summary>
    private HomePageViewModel()
    {
        Search = new SearchDetailViewModel();
        IsHomeShown = true;
    }

    /// <summary>
    /// 实例.
    /// </summary>
    public static HomePageViewModel Instance { get; } = new();

    [RelayCommand]
    private void OpenSearch(string keyword)
    {
        IsInSearch = true;
        Search.SetKeyword(keyword);
        Search.InitializeCommand.Execute(default);
    }

    private void CheckIsHomeShown()
        => IsHomeShown = !IsInSearch;

    partial void OnIsInSearchChanged(bool value)
        => CheckIsHomeShown();

    partial void OnIsHomeShownChanged(bool value)
        => AppViewModel.Instance.IsBackButtonShown = !value;
}
