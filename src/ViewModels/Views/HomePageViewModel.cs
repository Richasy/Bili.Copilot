// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.ViewModels.Views;
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

    [ObservableProperty]
    private bool _isInSearch;

    [ObservableProperty]
    private bool _isInMessage;

    [ObservableProperty]
    private bool _isInFans;

    [ObservableProperty]
    private bool _isInFollows;

    [ObservableProperty]
    private SearchDetailViewModel _search;

    [ObservableProperty]
    private MessageDetailViewModel _message;

    [ObservableProperty]
    private FansDetailViewModel _fans;

    /// <summary>
    /// Initializes a new instance of the <see cref="HomePageViewModel"/> class.
    /// </summary>
    private HomePageViewModel()
    {
        Search = new SearchDetailViewModel();
        Message = MessageDetailViewModel.Instance;
        Fans = new FansDetailViewModel();
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

    [RelayCommand]
    private void OpenMessage()
    {
        IsInMessage = true;
        Message.InitializeCommand.Execute(default);
    }

    [RelayCommand]
    private void OpenFans()
    {
        IsInFans = true;
        Fans.SetProfile(AccountViewModel.Instance.AccountInformation.User);
        Fans.InitializeCommand.Execute(default);
    }

    private void CheckIsHomeShown()
        => IsHomeShown = !IsInSearch && !IsInMessage && !IsInFans && !IsInFollows;

    partial void OnIsInSearchChanged(bool value)
        => CheckIsHomeShown();

    partial void OnIsInMessageChanged(bool value)
        => CheckIsHomeShown();

    partial void OnIsInFansChanged(bool value)
        => CheckIsHomeShown();

    partial void OnIsHomeShownChanged(bool value)
        => AppViewModel.Instance.IsBackButtonShown = !value;
}
