// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using Windows.System;
using WinUIEx;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 直播播放页视图模型.
/// </summary>
public sealed partial class LivePlayerPageViewModel
{
    [RelayCommand]
    private async Task ToggleFollowAsync()
    {
        try
        {
            if (IsFollow)
            {
                await _relationshipService.UnfollowUserAsync(_view.Information.User.Id);
                IsFollow = false;
            }
            else
            {
                await _relationshipService.FollowUserAsync(_view.Information.User.Id);
                IsFollow = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试关注/取消关注 UP 主时失败.");
        }
    }

    [RelayCommand]
    private void CopyUrl()
    {
        var url = $"https://live.bilibili.com/{_view.Information.Identifier.Id}";
        var dp = new DataPackage();
        dp.SetText(url);
        dp.SetWebLink(new Uri(url));
        Clipboard.SetContent(dp);
        this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Copied), InfoType.Success));
    }

    [RelayCommand]
    private void ShareUrl()
    {
        var handle = this.Get<AppViewModel>().ActivatedWindow.GetWindowHandle();
        var transferManager = DataTransferManagerInterop.GetForWindow(handle);
        transferManager.DataRequested += OnTransferDataRequested;
        DataTransferManagerInterop.ShowShareUIForWindow(handle);

        void OnTransferDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var url = $"https://live.bilibili.com/{_view.Information.Identifier.Id}";
            var dp = new DataPackage();
            dp.SetText(url);
            dp.SetWebLink(new Uri(url));
            dp.Properties.Title = Title;
            dp.Properties.Description = Description;
            dp.Properties.Thumbnail = RandomAccessStreamReference.CreateFromUri(_view.Information.Identifier.Cover.Uri);
            args.Request.Data = dp;
            sender.DataRequested -= OnTransferDataRequested;
        }
    }

    [RelayCommand]
    private async Task OpenInBroswerAsync()
    {
        var url = $"https://live.bilibili.com/{_view.Information.Identifier.Id}";
        await Launcher.LaunchUriAsync(new Uri(url)).AsTask();
    }

    [RelayCommand]
    private void OpenUserSpace()
    {
        var profile = _view.Information.User;
        this.Get<NavigationViewModel>().NavigateToOver(typeof(UserSpacePage).FullName, profile);
    }

    [RelayCommand]
    private void Pin()
    {
        var pinItem = new PinItem(_view.Information.Identifier.Id, _view.Information.Identifier.Title, _view.Information.Identifier.Cover.Uri.ToString(), PinContentType.Video);
        this.Get<PinnerViewModel>().AddItemCommand.Execute(pinItem);
    }

    private void PlayerProgressChanged(int progress, int duration)
    {
        this.Get<Microsoft.UI.Dispatching.DispatcherQueue>().TryEnqueue(() =>
        {
            Duration = Convert.ToInt32((DateTimeOffset.Now - StartTime).TotalSeconds);
        });
    }

    private void DisplayDanmaku(string text)
        => Danmaku.AddDanmakuCommand.Execute(text);

    private async Task SendDanmakuAsync(string content)
    {
        try
        {
            var color = Colors.White;
            var danmakuColor = (color.R * 256 * 256) + (color.G * 256) + color.B;
            await _danmakuService.SendLiveDanmakuAsync(content, RoomId, danmakuColor.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送直播弹幕时失败.");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.FailedToSendDanmaku), InfoType.Error));
        }
    }
}
