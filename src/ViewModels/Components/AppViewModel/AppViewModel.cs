﻿// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Article;
using Bili.Copilot.Models.Data.Local;
using Bili.Copilot.Models.Data.User;
using Bili.Copilot.Models.Data.Video;
using Bili.Copilot.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 应用视图模型.
/// </summary>
public sealed partial class AppViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppViewModel"/> class.
    /// </summary>
    private AppViewModel()
    {
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        NavigateItems = new ObservableCollection<NavigateItemViewModel>();
        DisplayWindows = new List<Window>();
    }

    /// <summary>
    /// 初始化.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task InitializeAsync()
    {
        IsSigningIn = true;
        var isSignedIn = await AuthorizeProvider.Instance.TrySignInAsync();
        IsSigningIn = false;
        if (!isSignedIn)
        {
            AuthorizeProvider.Instance.SignOut();
            RestartCommand.Execute(default);
        }
        else
        {
            LoadNavItems();
            var lastOpenPage = SettingsToolkit.ReadLocalSetting(SettingNames.LastOpenPageType, PageType.Popular);
            if (!NavigateItems.Any(p => p.Data?.Id == lastOpenPage))
            {
                lastOpenPage = NavigateItems.First(p => p.Data != null).Data.Id;
            }

            AccountViewModel.Instance.InitializeCommand.Execute(default);
            Navigate(lastOpenPage);
            FixModuleViewModel.Instance.InitializeCommand.Execute(default);
        }
    }

    /// <summary>
    /// 导航到指定页面.
    /// </summary>
    /// <param name="page">页面.</param>
    /// <param name="parameter">参数.</param>
    public void Navigate(PageType page, object parameter = null)
    {
        if (CurrentPage == page)
        {
            return;
        }

        SettingsItem.IsSelected = page == PageType.Settings;
        foreach (var item in NavigateItems)
        {
            item.IsSelected = page == item.Data.Id;
        }

        Logger.Trace($"Navigate {page}");
        NavigateRequest?.Invoke(this, new AppNavigationEventArgs(page, parameter));
        CurrentPage = page;

        if (page != PageType.Settings)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.LastOpenPageType, page);
        }
    }

    /// <summary>
    /// 显示提示.
    /// </summary>
    /// <param name="message">提示内容.</param>
    /// <param name="type">提示类型.</param>
    public void ShowTip(string message, InfoType type = InfoType.Information)
        => RequestShowTip?.Invoke(this, new AppTipNotification(message, type));

    /// <summary>
    /// 显示消息.
    /// </summary>
    /// <param name="message">消息内容.</param>
    public void ShowMessage(string message)
        => RequestShowMessage?.Invoke(this, message);

    /// <summary>
    /// 激活主窗口.
    /// </summary>
    public void ActivateMainWindow()
        => ActiveMainWindow?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// 修改主题.
    /// </summary>
    /// <param name="theme">主题类型.</param>
    public void ChangeTheme(ElementTheme theme)
    {
        if (DisplayWindows.Count == 0)
        {
            return;
        }

        foreach (var window in DisplayWindows)
        {
            (window.Content as FrameworkElement).RequestedTheme = theme;
            if (theme == ElementTheme.Dark)
            {
                window.AppWindow.TitleBar.ButtonForegroundColor = Colors.White;
            }
            else if (theme == ElementTheme.Light)
            {
                window.AppWindow.TitleBar.ButtonForegroundColor = Colors.Black;
            }
            else
            {
                window.AppWindow.TitleBar.ButtonForegroundColor = default;
            }
        }
    }

    [RelayCommand]
    private static void Restart()
    {
        AppInstance.GetCurrent().UnregisterKey();
        _ = AppInstance.Restart(default);
    }

    [RelayCommand]
    private void ShowImages(ShowImageEventArgs args)
        => RequestShowImages?.Invoke(this, args);

    [RelayCommand]
    private void OpenReader(ArticleInformation article)
        => RequestRead?.Invoke(this, article);

    [RelayCommand]
    private void ShowUserDetail(UserProfile user)
        => RequestShowUserSpace.Invoke(this, user);

    [RelayCommand]
    private void ShowCommentWindow(ShowCommentEventArgs args)
        => RequestShowCommentWindow.Invoke(this, args);

    [RelayCommand]
    private void OpenPlayer(PlaySnapshot snapshot)
        => RequestPlay?.Invoke(this, snapshot);

    [RelayCommand]
    private void OpenPlaylist(List<VideoInformation> playlist)
        => RequestPlaylist?.Invoke(this, playlist);

    [RelayCommand]
    private void Search(string text)
        => RequestSearch?.Invoke(this, text);

    [RelayCommand]
    private void SummarizeVideoContent(VideoIdentifier video)
        => RequestSummarizeVideoContent?.Invoke(this, video);

    [RelayCommand]
    private void SummarizeArticleContent(ArticleIdentifier article)
        => RequestSummarizeArticleContent?.Invoke(this, article);

    [RelayCommand]
    private void EvaluateVideoContent(VideoIdentifier video)
        => RequestEvaluateVideo?.Invoke(this, video);

    [RelayCommand]
    private void Back()
    {
        if (!IsBackButtonShown)
        {
            return;
        }

        BackRequest?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task CheckAIFeatureAsync()
    {
        var handlers = await Windows.System.Launcher.FindUriSchemeHandlersAsync("fancop").AsTask();
        IsAISupported = handlers.Any();
    }

    [RelayCommand]
    private void CheckBBDownExist()
    {
        try
        {
            var process = new Process();
            process.StartInfo.FileName = "BBDown";
            process.StartInfo.Arguments = "-h";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
            _dispatcherQueue.TryEnqueue(() =>
            {
                IsDownloadSupported = process.ExitCode == 0;
            });
        }
        catch (Exception)
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                IsDownloadSupported = false;
            });
        }
    }

    [RelayCommand]
    private async Task CheckUpdateAsync()
    {
        var data = await UpdateProvider.GetGitHubLatestReleaseAsync();
        var currentVersion = AppToolkit.GetPackageVersion();
        var ignoreVersion = SettingsToolkit.ReadLocalSetting(SettingNames.IgnoreVersion, string.Empty);
        var args = new UpdateEventArgs(data);
        if (args.Version != currentVersion && args.Version != ignoreVersion)
        {
            RequestShowUpdateDialog?.Invoke(this, args);
        }
    }

    private void LoadNavItems()
    {
        TryClear(NavigateItems);
        NavigateItems.Add(new NavigateItemViewModel(new NavigateItem(PageType.Popular, ResourceToolkit.GetLocalizedString(StringNames.PopularSlim))));
        NavigateItems.Add(new NavigateItemViewModel(new NavigateItem(PageType.Dynamic, ResourceToolkit.GetLocalizedString(StringNames.DynamicFeed))));
        NavigateItems.Add(new NavigateItemViewModel(new NavigateItem(PageType.Partition, ResourceToolkit.GetLocalizedString(StringNames.Partition))));
        NavigateItems.Add(new NavigateItemViewModel(new NavigateItem(PageType.Live, ResourceToolkit.GetLocalizedString(StringNames.Live))));
        NavigateItems.Add(new NavigateItemViewModel(new NavigateItem(PageType.Anime, ResourceToolkit.GetLocalizedString(StringNames.Anime))));
        NavigateItems.Add(new NavigateItemViewModel(new NavigateItem(PageType.Film, ResourceToolkit.GetLocalizedString(StringNames.Film))));
        NavigateItems.Add(new NavigateItemViewModel(new NavigateItem(PageType.Article, ResourceToolkit.GetLocalizedString(StringNames.SpecialColumn))));

        SettingsItem = new NavigateItemViewModel(new NavigateItem(PageType.Settings, ResourceToolkit.GetLocalizedString(StringNames.Settings)));
        SettingsItem.IsSelected = CurrentPage == PageType.Settings;
        foreach (var item in NavigateItems)
        {
            item.IsSelected = CurrentPage == item.Data.Id;
        }
    }
}
