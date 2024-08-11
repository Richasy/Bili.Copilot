// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Bili.User;
using Richasy.WinUI.Share.ViewModels;
using Windows.System;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 账户视图模型.
/// </summary>
public sealed partial class AccountViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AccountViewModel"/> class.
    /// </summary>
    public AccountViewModel(
        IMyProfileService myProfileService,
        NavigationViewModel navService,
        ILogger<AccountViewModel> logger)
    {
        _navService = navService;
        _myProfileService = myProfileService;
        _logger = logger;
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (MyProfile is not null)
        {
            return;
        }

        IsInitializing = true;
        try
        {
            MyProfile = await _myProfileService.GetMyProfileAsync();
            this.Get<AppViewModel>().IsInitialLoading = false;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "无法获取登录用户的资料");
        }

        IsInitializing = false;
    }

    [RelayCommand]
    private async Task UpdateCommunityInformationAsync()
    {
        if (MyProfile is null)
        {
            return;
        }

        try
        {
            var communityInformation = await _myProfileService.GetMyCommunityInformationAsync();
            MomentCount = communityInformation.MomentCount ?? 0;
            FollowCount = communityInformation.FollowCount ?? 0;
            FansCount = communityInformation.FansCount ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "已登录用户的社区信息获取失败");
        }
    }

    [RelayCommand]
    private async Task SignOutAsync()
    {
        await this.Get<IAuthenticationService>().SignOutAsync().ConfigureAwait(false);
        MyProfile = null;
        this.Get<AppViewModel>().RestartCommand.Execute(default);
    }

    [RelayCommand]
    private async Task OpenPersonalWebsiteAsync()
    {
        if (MyProfile is null)
        {
            return;
        }

        await Launcher.LaunchUriAsync(new Uri($"https://space.bilibili.com/{MyProfile.User.Id}"));
    }

    [RelayCommand]
    private void ShowFans()
        => _navService.NavigateToOver(typeof(FansPage).FullName, MyProfile.User.Id);

    [RelayCommand]
    private void ShowFollows()
        => _navService.NavigateToOver(typeof(FollowsPage).FullName, MyProfile.User.Id);

    [RelayCommand]
    private void ShowMoments()
        => _navService.NavigateToOver(typeof(MyMomentsPage).FullName, MyProfile.User.Id);

    [RelayCommand]
    private void ShowViewLater()
        => _navService.NavigateToOver(typeof(ViewLaterPage).FullName);

    [RelayCommand]
    private void ShowHistory()
        => _navService.NavigateToOver(typeof(HistoryPage).FullName);

    [RelayCommand]
    private void ShowFavorites()
        => _navService.NavigateToOver(typeof(FavoritesPage).FullName);
}
