// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Pages;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Bili.User;
using Richasy.WinUIKernel.Share.ViewModels;
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
        IMessageService messageService,
        NavigationViewModel navService,
        ILogger<AccountViewModel> logger)
    {
        _navService = navService;
        _messageService = messageService;
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
            Introduce = string.IsNullOrEmpty(MyProfile.Introduce)
                ? ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.NoSelfIntroduce)
                : MyProfile.Introduce;
            this.Get<AppViewModel>().IsInitialLoading = false;
            this.Get<AppViewModel>().CheckUpateCommand.Execute(default);
            UpdateUnreadCommand.Execute(default);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "无法获取登录用户的资料");

            // 用户可能更改了密码，提醒用户是否需要重新登录.
            var dialog = new ContentDialog()
            {
                Title = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.LoginExpiredTitle),
                Content = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.LoginExpiredDescription),
                PrimaryButtonText = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.ReSignIn),
                CloseButtonText = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.Exit),
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.Get<AppViewModel>().ActivatedWindow.Content.XamlRoot,
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                await SignOutAsync();
            }
            else
            {
                App.Current.Exit();
            }
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
            UpdateUnreadCommand.Execute(default);
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
        => _navService.NavigateToOver(typeof(FansPage), MyProfile.User.Id);

    [RelayCommand]
    private void ShowFollows()
        => _navService.NavigateToOver(typeof(FollowsPage), MyProfile.User.Id);

    [RelayCommand]
    private void ShowMoments()
        => _navService.NavigateToOver(typeof(UserSpacePage), MyProfile.User);

    [RelayCommand]
    private void ShowViewLater()
        => _navService.NavigateToOver(typeof(ViewLaterPage));

    [RelayCommand]
    private void ShowHistory()
        => _navService.NavigateToOver(typeof(HistoryPage));

    [RelayCommand]
    private void ShowFavorites()
        => _navService.NavigateToOver(typeof(FavoritesPage));

    [RelayCommand]
    private async Task UpdateUnreadAsync()
    {
        try
        {
            var unreadInfo = await _messageService.GetUnreadInformationAsync();
            _navService.FooterItems.First(p => p.PageKey == typeof(MessagePage).FullName).ShowUnread = unreadInfo.Total > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "未读消息获取失败");
        }
    }
}
