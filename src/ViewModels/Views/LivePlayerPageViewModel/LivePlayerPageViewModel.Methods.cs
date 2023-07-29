// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Specialized;
using System.ComponentModel;
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
        _ = await Launcher.LaunchUriAsync(new Uri(uri));
    }

    private void OnMessageReceived(object sender, LiveMessageEventArgs e)
    {
        _ = _dispatcherQueue.TryEnqueue(() =>
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
        PlayerDetail.PropertyChanged += OnPlayerDetailPropertyChanged;
        PlayerDetail.RequestOpenInBrowser += OnRequestOpenInBrowserAsync;
    }

    private void CheckSectionVisibility()
    {
        if (CurrentSection == null)
        {
            IsShowInformation = false;
            IsShowChat = false;
            return;
        }

        IsShowInformation = CurrentSection.Type == PlayerSectionType.LiveInformation;
        IsShowChat = CurrentSection.Type == PlayerSectionType.Chat;
    }

    private async void OnRequestOpenInBrowserAsync(object sender, EventArgs e) => await OpenInBroswerAsync();

    private void OnAuthorizeStateChanged(object sender, AuthorizeStateChangedEventArgs e)
        => IsSignedIn = e.NewState == AuthorizeState.SignedIn;

    private async void OnHeartBeatTimerTickAsync(object sender, object e)
    {
        if (PlayerDetail.Status is PlayerStatus.NotLoad
            or PlayerStatus.End)
        {
            return;
        }

        await LiveProvider.Instance.SendHeartBeatAsync();
    }

    private void OnDanmakusCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => IsDanmakusEmpty = Danmakus.Count == 0;

    private void OnPlayerDetailPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName.Equals(nameof(PlayerDetail.Status)))
        {
            UpdateLiveMediaInformation();
        }
    }

    private void UpdateLiveMediaInformation()
    {
        var info = PlayerDetail.Player?.GetMediaInformation();
        if (info == null)
        {
            return;
        }

        var media = new Models.App.Other.LiveMediaStats(info)
        {
            PlayUrl = PlayerDetail.GetLivePlayUrl(),
        };
        Stats = media;
    }
}
