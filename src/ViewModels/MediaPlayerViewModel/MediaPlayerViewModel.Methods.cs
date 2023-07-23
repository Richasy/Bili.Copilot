// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.ComponentModel;
using Bili.Copilot.Libs.Flyleaf.MediaFramework.MediaPlaylist;
using Bili.Copilot.Libs.Flyleaf.MediaPlayer;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.Constants.App;
using CommunityToolkit.Mvvm.Input;
using Windows.Web.Http;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 使用 FFmpeg 的播放器视图模型.
/// </summary>
public sealed partial class MediaPlayerViewModel
{
    private static HttpClient GetVideoClient()
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Referer = new Uri("https://www.bilibili.com");
        httpClient.DefaultRequestHeaders.Add("User-Agent", ServiceConstants.DefaultUserAgentString);
        return httpClient;
    }

    private static HttpClient GetLiveClient()
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Referer = new Uri("https://live.bilibili.com/");
        httpClient.DefaultRequestHeaders.Add("rtsp_transport", "tcp");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 BiliDroid/1.12.0 (bbcallen@gmail.com)");
        return httpClient;
    }

    private void LoadDashVideoSource()
    {
        var playItem = new PlaylistItem();
        playItem.Title = "video";
        playItem.Tag.Add("video", _video);

        if (_audio != null)
        {
            playItem.Tag.Add("audio", _audio);
        }

        Player.OpenAsync(playItem);
    }

    private void LoadDashLiveSource(string url)
    {
        try
        {
            Player.OpenAsync(url);
        }
        catch (Exception ex)
        {
            Status = PlayerStatus.Failed;
            var msg = ResourceToolkit.GetLocalizedString(StringNames.RequestLivePlayInformationFailed) + "\n" + ex.Message;
            StateChanged?.Invoke(this, new MediaStateChangedEventArgs(Status, msg));
            LogException(ex);
        }
    }

    private void OnPlayerOpenCompleted(object sender, OpenCompletedArgs e)
    {
        if (e.Success)
        {
            MediaOpened?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Status = PlayerStatus.Failed;
            var arg = new MediaStateChangedEventArgs(Status, e.Error);
            StateChanged?.Invoke(this, arg);
            LogException(new Exception($"播放失败: {e.Error}"));
        }
    }

    private void OnPlayerPlaybackStopped(object sender, PlaybackStoppedArgs e)
    {
        if (e.Success)
        {
            Status = PlayerStatus.End;
            MediaEnded?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Status = PlayerStatus.Failed;
            var arg = new MediaStateChangedEventArgs(Status, e.Error);
            StateChanged?.Invoke(this, arg);
            LogException(new Exception($"播放失败: {e.Error}"));
        }
    }

    private void OnPlayerPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Player.Status))
        {
            if (Player == null)
            {
                Status = PlayerStatus.NotLoad;
                StateChanged?.Invoke(this, new MediaStateChangedEventArgs(Status, null));
                return;
            }

            Status = Player.Status switch
            {
                Libs.Flyleaf.MediaPlayer.Status.Paused => PlayerStatus.Pause,
                Libs.Flyleaf.MediaPlayer.Status.Playing => PlayerStatus.Playing,
                Libs.Flyleaf.MediaPlayer.Status.Stopped => PlayerStatus.End,
                Libs.Flyleaf.MediaPlayer.Status.Opening => PlayerStatus.Buffering,
                Libs.Flyleaf.MediaPlayer.Status.Failed => PlayerStatus.Failed,
                _ => PlayerStatus.NotLoad,
            };
            var arg = new MediaStateChangedEventArgs(Status, null);
            StateChanged?.Invoke(this, arg);
        }
        else if (e.PropertyName == nameof(Player.CurTime))
        {
            PositionChanged?.Invoke(this, new MediaPositionChangedEventArgs(Position, TimeSpan.FromTicks(Player?.Duration ?? 0)));
        }
    }

    [RelayCommand]
    private void Clear()
    {
        Player?.Dispose();
        Status = PlayerStatus.NotLoad;
    }
}
