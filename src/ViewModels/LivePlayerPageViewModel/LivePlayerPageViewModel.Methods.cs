// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Authorize;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Live;
using CommunityToolkit.Mvvm.Input;
using Windows.System;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 直播播放页面视图模型.
/// </summary>
public sealed partial class LivePlayerPageViewModel
{
    [RelayCommand]
    private async Task OpenInBroswerAsync()
    {
        var uri = $"https://live.bilibili.com/{View.Information.Identifier.Id}";
        await Launcher.LaunchUriAsync(new Uri(uri));
    }

    private void OnMessageReceived(object sender, LiveMessageEventArgs e)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (e.Type == LiveMessageType.ConnectSuccess)
            {
                _heartBeatTimer.Start();
            }
            else if (e.Type == LiveMessageType.Danmaku)
            {
                var data = e.Data as LiveDanmakuInformation;
                Danmakus.Add(data);
                PlayerDetail.DanmakuViewModel.AddLiveDanmakuCommand.Execute(data);

                if (Danmakus.Count > 1000)
                {
                    var reserveMessages = Danmakus.TakeLast(100);
                    Danmakus.Clear();
                    foreach (var item in reserveMessages)
                    {
                        Danmakus.Add(item);
                    }
                }

                if (IsDanmakusAutoScroll)
                {
                    RequestDanmakusScrollToBottom?.Invoke(this, EventArgs.Empty);
                }
            }
        });
    }

    private void ReloadMediaPlayer()
    {
        if (PlayerDetail != null)
        {
            return;
        }

        PlayerDetail = new PlayerDetailViewModel(_attachedWindow);
        PlayerDetail.RequestOpenInBrowser += OnRequestOpenInBrowserAsync;
    }

    private async void OnRequestOpenInBrowserAsync(object sender, EventArgs e)
    {
        var uri = $"https://live.bilibili.com/{View.Information.Identifier.Id}";
        await Launcher.LaunchUriAsync(new Uri(uri));
    }

    private void OnAuthorizeStateChanged(object sender, AuthorizeStateChangedEventArgs e)
        => IsSignedIn = e.NewState == AuthorizeState.SignedIn;

    private async void OnHeartBeatTimerTickAsync(object sender, object e)
    {
        if (PlayerDetail.Status == PlayerStatus.NotLoad
            || PlayerDetail.Status == PlayerStatus.End)
        {
            return;
        }

        await LiveProvider.Instance.SendHeartBeatAsync();
    }

    private void OnDanmakusCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => IsDanmakusEmpty = Danmakus.Count == 0;
}
