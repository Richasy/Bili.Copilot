// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Forms;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Windows.ApplicationModel.DataTransfer;
using WinUIEx;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// WebDav 播放页面视图模型.
/// </summary>
public sealed partial class WebDavPlayerPageViewModel
{
    private string GetVideoUrl()
    {
        var config = this.Get<WebDavPageViewModel>().GetCurrentConfig();
        return AppToolkit.GetWebDavServer(config.GetServer(), Current.Data.Uri);
    }

    [RelayCommand]
    private void CopyVideoUrl()
    {
        var url = GetVideoUrl();
        var dp = new DataPackage();
        dp.SetText(url);
        dp.SetWebLink(new Uri(url));
        Clipboard.SetContent(dp);
        this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Copied), InfoType.Success));
    }

    [RelayCommand]
    private void ShareVideoUrl()
    {
        var handle = this.Get<AppViewModel>().ActivatedWindow.GetWindowHandle();
        var transferManager = DataTransferManagerInterop.GetForWindow(handle);
        transferManager.DataRequested += OnTransferDataRequested;
        DataTransferManagerInterop.ShowShareUIForWindow(handle);

        void OnTransferDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var url = GetVideoUrl();
            var dp = new DataPackage();
            dp.SetText(url);
            dp.SetWebLink(new Uri(url));
            dp.Properties.Title = Title;
            args.Request.Data = dp;
            sender.DataRequested -= OnTransferDataRequested;
        }
    }

    [RelayCommand]
    private void PlayNextVideo()
    {
        if (Player.IsPlayerDataLoading)
        {
            return;
        }

        var next = FindNextVideo();
        if (next is null)
        {
            Player.BackToDefaultModeCommand.Execute(default);
            return;
        }

        InitializeCommand.Execute(next.Data);
    }

    [RelayCommand]
    private void OpenInNewWindow()
    {
        if (!Player.IsPaused)
        {
            Player.TogglePlayPauseCommand.Execute(default);
        }

        new PlayerWindow().OpenWebDav(Current.Data, Playlist.Select(p => p.Data).ToList());
    }

    private WebDavStorageItemViewModel? FindNextVideo()
    {
        if (Playlist is not null)
        {
            var index = Playlist.ToList().IndexOf(Current);
            if (index < Playlist.Count - 1)
            {
                return Playlist[index + 1];
            }
        }

        return default;
    }

    private void PlayerMediaEnded()
    {
        this.Get<Microsoft.UI.Dispatching.DispatcherQueue>().TryEnqueue(() =>
        {
            var autoNext = SettingsToolkit.ReadLocalSetting(SettingNames.AutoPlayNext, true);
            if (!autoNext || !HasNextVideo)
            {
                Player.BackToDefaultModeCommand.Execute(default);
                return;
            }

            var next = FindNextVideo();
            if (next is null)
            {
                return;
            }

            var tip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.NextVideoNotificationTemplate), next.Data.DisplayName);
            var notification = new PlayerNotification(PlayNextVideo, tip, 5);
            Player.ShowNotification(notification);
        });
    }
}
