// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;

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
    }

    [RelayCommand]
    private async Task OpenInBroswerAsync()
    {
        var url = $"https://live.bilibili.com/{_view.Information.Identifier.Id}";
        await Launcher.LaunchUriAsync(new Uri(url)).AsTask();
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
}
