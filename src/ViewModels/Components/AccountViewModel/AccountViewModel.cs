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

    [RelayCommand]
    private static void OpenFans()
        => HomePageViewModel.Instance.OpenFansCommand.Execute(default);

    [RelayCommand]
    private static void OpenFollows()
        => HomePageViewModel.Instance.OpenFollowsCommand.Execute(default);

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
        => Launcher.LaunchUriAsync(new Uri($"https://space.bilibili.com/{AccountInformation.User.Id}")).AsTask();

    [RelayCommand]
    private Task OpenDynamicAsync()
        => Launcher.LaunchUriAsync(new Uri($"https://space.bilibili.com/{AccountInformation.User.Id}/dynamic")).AsTask();

    /// <summary>
    /// 获取我的账户资料.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    private async Task GetMyProfileAsync()
    {
        AccountInformation = await AccountProvider.Instance.GetMyInformationAsync();
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
        if (AccountInformation == null)
        {
            return;
        }

        Avatar = AccountInformation.User.Avatar.GetSourceUri().ToString();
        Name = AccountInformation.User.Name;
        IsVip = AccountInformation.IsVip;
        Introduce = string.IsNullOrEmpty(AccountInformation.Introduce)
            ? ResourceToolkit.GetLocalizedString(StringNames.NoSelfIntroduce)
            : AccountInformation.Introduce;
        LevelImage = $"ms-appx:///Assets/Level/level_{AccountInformation.Level}.png";
    }
}
