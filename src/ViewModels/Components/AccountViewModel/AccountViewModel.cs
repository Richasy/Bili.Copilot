// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
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
    private static void OpenFollows()
        => AppViewModel.Instance.ShowFollowsCommand.Execute(default);

    [RelayCommand]
    private static void ShowViewLater()
        => AppViewModel.Instance.ShowViewLaterCommand.Execute(default);

    [RelayCommand]
    private static void ShowHistory()
        => AppViewModel.Instance.ShowHistoryCommand.Execute(default);

    [RelayCommand]
    private static void ShowFavorites()
        => AppViewModel.Instance.ShowFavoritesCommand.Execute(default);

    [RelayCommand]
    private void OpenFans()
        => AppViewModel.Instance.ShowFansCommand.Execute(AccountInformation.User);

    /// <summary>
    /// 登出.
    /// </summary>
    [RelayCommand]
    private async Task SignOutAsync()
    {
        _isInitialized = false;
        AuthorizeProvider.Instance.SignOut();
        var webview2 = new WebView2();
        await webview2.EnsureCoreWebView2Async();
        await webview2.CoreWebView2.Profile.ClearBrowsingDataAsync(Microsoft.Web.WebView2.Core.CoreWebView2BrowsingDataKinds.AllProfile);
        AppViewModel.Instance.RestartCommand.Execute(default);
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
    private void OpenDynamic()
        => AppViewModel.Instance.ShowUserDetailCommand.Execute(AccountInformation.User);

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
    [RelayCommand]
    private async Task InitialCommunityInformationAsync()
    {
        try
        {
            var data = await AccountProvider.Instance.GetMyCommunityInformationAsync();
            DynamicCount = NumberToolkit.GetCountText(data.DynamicCount);
            FollowCount = NumberToolkit.GetCountText(data.FollowCount);
            FansCount = NumberToolkit.GetCountText(data.FansCount);
            CoinCount = NumberToolkit.GetCountText(data.CoinCount);
            await InitializeUnreadAsync();
        }
        catch (Exception)
        {
        }
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
        MessageCountInt = unreadInformation.Total > 99 ? 99 : unreadInformation.Total;
        MessageCount = unreadInformation.Total > 99 ? "99+" : unreadInformation.Total.ToString();

        if (AppViewModel.Instance.MessageItem != null)
        {
            AppViewModel.Instance.MessageItem.HasUnread = MessageCountInt > 0;
        }
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
