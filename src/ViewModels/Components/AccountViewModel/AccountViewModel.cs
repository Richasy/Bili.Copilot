// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using CommunityToolkit.Mvvm.Input;
using Windows.System;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 账户视图模型.
/// </summary>
public sealed partial class AccountViewModel : ViewModelBase
{
    private AccountViewModel()
    {
        AttachIsRunningToAsyncCommand(p => IsInitializing = p, InitializeCommand);
        AttachExceptionHandlerToAsyncCommand(LogException, InitializeCommand);
    }

    [RelayCommand]
    private static void OpenMessage()
        => HomePageViewModel.Instance.OpenMessageCommand.Execute(default);

    /// <summary>
    /// 登出.
    /// </summary>
    [RelayCommand]
    private async Task SignOutAsync()
    {
        _isInitialized = false;
        AuthorizeProvider.Instance.SignOut();
        await AppViewModel.Instance.InitializeAsync();
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (_isInitialized || IsInitializing)
        {
            return;
        }

        await GetMyProfileAsync();
        await InitialCommunityInformationAsync();
        _isInitialized = true;
    }

    [RelayCommand]
    private Task OpenPersonalSiteAsync()
        => Launcher.LaunchUriAsync(new Uri($"https://space.bilibili.com/{_accountInformation.User.Id}")).AsTask();

    [RelayCommand]
    private Task OpenDynamicAsync()
        => Launcher.LaunchUriAsync(new Uri($"https://space.bilibili.com/{_accountInformation.User.Id}/dynamic")).AsTask();

    [RelayCommand]
    private Task OpenFollowAsync()
        => Launcher.LaunchUriAsync(new Uri($"https://space.bilibili.com/{_accountInformation.User.Id}/fans/follow")).AsTask();

    [RelayCommand]
    private Task OpenFansAsync()
        => Launcher.LaunchUriAsync(new Uri($"https://space.bilibili.com/{_accountInformation.User.Id}/fans/fans")).AsTask();

    /// <summary>
    /// 获取我的账户资料.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    private async Task GetMyProfileAsync()
    {
        _accountInformation = await AccountProvider.Instance.GetMyInformationAsync();
        InitializeAccountInformation();
    }

    /// <summary>
    /// 初始化用户社交信息.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    private async Task InitialCommunityInformationAsync()
    {
        var data = await AccountProvider.Instance.GetMyCommunityInformationAsync();
        DynamicCount = NumberToolkit.GetCountText(data.DynamicCount);
        FollowCount = NumberToolkit.GetCountText(data.FollowCount);
        FansCount = NumberToolkit.GetCountText(data.FansCount);
        CoinCount = NumberToolkit.GetCountText(data.CoinCount);
        await InitializeUnreadAsync();
    }

    /// <summary>
    /// 加载未读消息数据.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    [RelayCommand]
    private async Task InitializeUnreadAsync()
    {
        var unreadInformation = await AccountProvider.GetUnreadMessageAsync();
        UnreadInformation = unreadInformation;
        MessageCount = unreadInformation.Total > 99 ? "99+" : unreadInformation.Total.ToString();
    }

    private void InitializeAccountInformation()
    {
        if (_accountInformation == null)
        {
            return;
        }

        Avatar = _accountInformation.User.Avatar.GetSourceUri().ToString();
        Name = _accountInformation.User.Name;
        IsVip = _accountInformation.IsVip;
        Introduce = string.IsNullOrEmpty(_accountInformation.Introduce)
            ? ResourceToolkit.GetLocalizedString(StringNames.NoSelfIntroduce)
            : _accountInformation.Introduce;
        LevelImage = $"ms-appx:///Assets/Level/level_{_accountInformation.Level}.png";
    }
}
