// Copyright (c) Bili Copilot. All rights reserved.

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
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;

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
        NavigateItems = new ObservableCollection<NavigateItem>();
    }

    /// <summary>
    /// 初始化.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task InitializeAsync()
    {
        IsNavigationMenuShown = false;
        IsSigningIn = true;
        var isSignedIn = await AuthorizeProvider.Instance.TrySignInAsync();
        IsSigningIn = false;
        if (!isSignedIn)
        {
            Navigate(PageType.SignIn);
        }
        else
        {
            LoadNavItems();
            var lastOpenPage = SettingsToolkit.ReadLocalSetting(SettingNames.LastOpenPageType, PageType.Home);
            if (!NavigateItems.Any(p => p.Id == lastOpenPage))
            {
                lastOpenPage = NavigateItems.First().Id;
            }

            Navigate(lastOpenPage);
            if (lastOpenPage != PageType.Home)
            {
                AccountViewModel.Instance.InitializeCommand.Execute(default);
            }
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

        Logger.Trace($"Navigate {page}");
        NavigateRequest?.Invoke(this, new AppNavigationEventArgs(page, parameter));
        CurrentPage = page;
        if (CurrentNavigateItem?.Id != page)
        {
            CurrentNavigateItem = NavigateItems.FirstOrDefault(p => p.Id == CurrentPage);
        }

        IsNavigationMenuShown = page != PageType.SignIn;
        if (IsNavigationMenuShown && page != PageType.Settings)
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
        NavigateItems.Add(new NavigateItem(PageType.Home, ResourceToolkit.GetLocalizedString(StringNames.Home), FluentSymbol.Home));
        NavigateItems.Add(new NavigateItem(PageType.Partition, ResourceToolkit.GetLocalizedString(StringNames.Partition), FluentSymbol.Apps));
        NavigateItems.Add(new NavigateItem(PageType.Dynamic, ResourceToolkit.GetLocalizedString(StringNames.DynamicFeed), FluentSymbol.DesignIdeas));
        NavigateItems.Add(new NavigateItem(PageType.Popular, ResourceToolkit.GetLocalizedString(StringNames.Popular), FluentSymbol.Rocket));
        NavigateItems.Add(new NavigateItem(PageType.Live, ResourceToolkit.GetLocalizedString(StringNames.Live), FluentSymbol.Video));
        NavigateItems.Add(new NavigateItem(PageType.Anime, ResourceToolkit.GetLocalizedString(StringNames.Anime), FluentSymbol.Dust));
        NavigateItems.Add(new NavigateItem(PageType.Film, ResourceToolkit.GetLocalizedString(StringNames.Film), FluentSymbol.FilmstripPlay));
        NavigateItems.Add(new NavigateItem(PageType.Article, ResourceToolkit.GetLocalizedString(StringNames.SpecialColumn), FluentSymbol.DocumentBulletList));
        NavigateItems.Add(new NavigateItem(PageType.Watchlist, ResourceToolkit.GetLocalizedString(StringNames.Watchlist), FluentSymbol.VideoClipMultiple));
    }

    partial void OnCurrentNavigateItemChanged(NavigateItem value)
    {
        if (value != null)
        {
            Navigate(value.Id);
        }
    }
}
