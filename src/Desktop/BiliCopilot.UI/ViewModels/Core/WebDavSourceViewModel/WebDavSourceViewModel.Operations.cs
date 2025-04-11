// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using BiliCopilot.UI.ViewModels.View;
using CommunityToolkit.Mvvm.Input;
using Richasy.WinUIKernel.Share.Base;
using Windows.ApplicationModel.DataTransfer;
using WinUIEx;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// WEB DAV 源视图模型.
/// </summary>
public sealed partial class WebDavSourceViewModel
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
    private async Task PlayPrevVideoAsync()
    {
        var prev = FindPrevVideo();
        if (prev is null)
        {
            return;
        }

        _initialProgress = 0;
        _cachedVideo = prev.Data;
        await InitializeAsync();
    }

    [RelayCommand]
    private async Task PlayNextVideoAsync()
    {
        var next = FindNextVideo();
        if (next is null)
        {
            return;
        }

        _initialProgress = 0;
        _cachedVideo = next.Data;
        await InitializeAsync();
    }

    private WebDavStorageItemViewModel? FindPrevVideo()
    {
        if (Playlist is not null)
        {
            var index = Playlist.ToList().IndexOf(Current);
            if (index > 0)
            {
                return Playlist[index - 1];
            }
        }

        return default;
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
}
