// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.ComponentModel;
using System.Linq;
using Bili.Copilot.Libs.Flyleaf.MediaFramework.MediaContext;
using Bili.Copilot.Libs.Flyleaf.MediaFramework.MediaPlaylist;
using Bili.Copilot.Libs.Flyleaf.MediaPlayer;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Constants.App;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 使用 FFmpeg 的播放器视图模型.
/// </summary>
public sealed partial class MediaPlayerViewModel
{
    /// <summary>
    /// 获取当前正在播放的媒体信息.
    /// </summary>
    /// <returns><see cref="MediaStats"/>.</returns>
    public MediaStats GetMediaInformation()
    {
        if (Player == null || !Player.CanPlay)
        {
            return null;
        }

        var video = Player.Video;
        var audio = Player.Audio;
        var fps = video?.FPS ?? -1;
        var width = video.Width;
        var height = video.Height;
        var videoCodec = video.Codec;
        var audioCodec = audio?.Codec ?? "N/A";
        var pixelFormat = video?.PixelFormat ?? "--";
        var bitrate = video?.Streams.FirstOrDefault()?.BitRate ?? 0;
        var colorSpace = video?.Streams.FirstOrDefault()?.ColorSpace.ToString() ?? "--";
        var sampleFormat = audio?.SampleFormat ?? "--";
        var sampleRate = audio?.SampleRate ?? 0;
        var channels = audio?.Channels ?? 0;

        var mediaInfo = new MediaStats
        {
            Fps = Math.Round(fps, 1),
            Width = width,
            Height = height,
            VideoCodec = videoCodec,
            AudioCodec = audioCodec,
            PixelFormat = pixelFormat,
            Bitrate = bitrate,
            ColorSpace = colorSpace,
            AudioSampleFormat = sampleFormat,
            AudioSampleRate = sampleRate,
            AudioChannels = channels,
        };

        return mediaInfo;
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

        var cookie = AuthorizeProvider.GetCookieString();
        if (!string.IsNullOrEmpty(cookie))
        {
            playItem.Tag.Add("cookie", cookie);
        }

        Player.OpenAsync(playItem);
    }

    private void LoadDashLiveSource(string url)
    {
        try
        {
            var playItem = new PlaylistItem
            {
                Title = "video",
            };
            playItem.Tag.Add("live", url);
            var cookie = AuthorizeProvider.GetCookieString();
            if (!string.IsNullOrEmpty(cookie))
            {
                playItem.Tag.Add("cookie", cookie);
            }

            Player.OpenAsync(playItem);
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
        _dispatcherQueue.TryEnqueue(() =>
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
        });
    }

    private void OnPlayerPlaybackStopped(object sender, PlaybackStoppedArgs e)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (Player == null)
            {
                return;
            }

            if (e.Success)
            {
                Status = Player.Status == Libs.Flyleaf.MediaPlayer.Status.Paused ? PlayerStatus.Pause : PlayerStatus.End;
                if (Status == PlayerStatus.End)
                {
                    MediaEnded?.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                Status = PlayerStatus.Failed;
                var arg = new MediaStateChangedEventArgs(Status, e.Error);
                StateChanged?.Invoke(this, arg);
                LogException(new Exception($"播放失败: {e.Error}"));
            }
        });
    }

    private void OnOpenPlaylistItemCompleted(object sender, DecoderContext.OpenPlaylistItemCompletedArgs e)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (e.Success)
            {
                Status = PlayerStatus.Opened;
                MediaOpened?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Status = PlayerStatus.Failed;
                var arg = new MediaStateChangedEventArgs(Status, e.Error);
                StateChanged?.Invoke(this, arg);
                LogException(new Exception($"播放失败: {e.Error}"));
            }
        });
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
