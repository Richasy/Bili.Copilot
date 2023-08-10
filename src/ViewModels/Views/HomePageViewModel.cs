// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
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
    private HomeCustomModuleType _customModule;

    [ObservableProperty]
    private string _customModuleTitle;

    [ObservableProperty]
    private bool _isHotSearchModuleShown;

    [ObservableProperty]
    private bool _isFixedModuleShown;

    [ObservableProperty]
    private SearchDetailViewModel _search;

    [ObservableProperty]
    private MessageDetailViewModel _message;

    [ObservableProperty]
    private FansDetailViewModel _fans;

    [ObservableProperty]
    private MyFollowsDetailViewModel _follows;

    /// <summary>
    /// Initializes a new instance of the <see cref="HomePageViewModel"/> class.
    /// </summary>
    private HomePageViewModel()
    {
        Search = new SearchDetailViewModel();
        Message = MessageDetailViewModel.Instance;
        Fans = new FansDetailViewModel();
        Follows = MyFollowsDetailViewModel.Instance;
        IsHomeShown = true;
        CustomModule = SettingsToolkit.ReadLocalSetting(SettingNames.HomeCustomModuleType, HomeCustomModuleType.HotSearch);
        CheckCustomModule();
    }

    /// <summary>
    /// 实例.
    /// </summary>
    public static HomePageViewModel Instance { get; } = new();

    [RelayCommand]
    private void OpenSearch(string keyword)
    {
        Reset();
        IsInSearch = true;
        Search.SetKeyword(keyword);
        Search.InitializeCommand.Execute(default);
    }

    [RelayCommand]
    private void OpenMessage()
    {
        Reset();
        IsInMessage = true;
        Message.InitializeCommand.Execute(default);
    }

    [RelayCommand]
    private void OpenFans()
    {
        Reset();
        IsInFans = true;
        Fans.SetProfile(AccountViewModel.Instance.AccountInformation.User);
        Fans.InitializeCommand.Execute(default);
    }

    [RelayCommand]
    private void OpenFollows()
    {
        Reset();
        IsInFollows = true;
        Follows.InitializeCommand.Execute(default);
    }

    [RelayCommand]
    private void Reset()
    {
        IsInSearch = false;
        IsInMessage = false;
        IsInFans = false;
        IsInFollows = false;
    }

    private void CheckIsHomeShown()
        => IsHomeShown = !IsInSearch && !IsInMessage && !IsInFans && !IsInFollows;

    private void CheckCustomModule()
    {
        IsFixedModuleShown = CustomModule == HomeCustomModuleType.Fixed;
        IsHotSearchModuleShown = CustomModule == HomeCustomModuleType.HotSearch;
        CustomModuleTitle = CustomModule switch
        {
            HomeCustomModuleType.HotSearch => ResourceToolkit.GetLocalizedString(StringNames.HotSearch),
            HomeCustomModuleType.Fixed => ResourceToolkit.GetLocalizedString(StringNames.FixedContent),
            _ => string.Empty
        };
    }

    partial void OnIsInSearchChanged(bool value)
        => CheckIsHomeShown();

    partial void OnIsInMessageChanged(bool value)
        => CheckIsHomeShown();

    partial void OnIsInFansChanged(bool value)
        => CheckIsHomeShown();

    partial void OnIsInFollowsChanged(bool value)
        => CheckIsHomeShown();

    partial void OnIsHomeShownChanged(bool value)
        => AppViewModel.Instance.IsBackButtonShown = !value;

    partial void OnCustomModuleChanged(HomeCustomModuleType value)
    {
        CheckCustomModule();
        SettingsToolkit.WriteLocalSetting(SettingNames.HomeCustomModuleType, value);
    }
}
