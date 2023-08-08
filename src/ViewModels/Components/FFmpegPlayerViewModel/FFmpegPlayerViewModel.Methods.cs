// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Flyleaf.MediaFramework.MediaContext;
using Bili.Copilot.Libs.Flyleaf.MediaFramework.MediaPlaylist;
using Bili.Copilot.Libs.Flyleaf.MediaPlayer;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Constants.App;
using CommunityToolkit.Mvvm.Input;
using Windows.Storage;
using Windows.System;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 使用 FFmpeg 的播放器视图模型.
/// </summary>
public sealed partial class FFmpegPlayerViewModel
{
    /// <summary>
    /// 获取当前正在播放的媒体信息.
    /// </summary>
    /// <returns><see cref="MediaStats"/>.</returns>
    public MediaStats GetMediaInformation()
    {
        if (Player == null || !((Player)Player).CanPlay)
        {
            return null;
        }

        var video = ((Player)Player).Video;
        var audio = ((Player)Player).Audio;
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

    private static PlayerStatus ConvertPlayerStatus(Libs.Flyleaf.MediaPlayer.Status status)
    {
        var s = status switch
        {
            Libs.Flyleaf.MediaPlayer.Status.Paused => PlayerStatus.Pause,
            Libs.Flyleaf.MediaPlayer.Status.Playing => PlayerStatus.Playing,
            Libs.Flyleaf.MediaPlayer.Status.Stopped => PlayerStatus.End,
            Libs.Flyleaf.MediaPlayer.Status.Opening => PlayerStatus.Buffering,
            Libs.Flyleaf.MediaPlayer.Status.Failed => PlayerStatus.Failed,
            _ => PlayerStatus.NotLoad,
        };

        return s;
    }

    [RelayCommand]
    private async Task TakeScreenshotAsync()
    {
        if (Player == null)
        {
            return;
        }

        var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "哔哩截图");
        if (!Directory.Exists(folderPath))
        {
            _ = Directory.CreateDirectory(folderPath);
        }

        var fileName = $"{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.png";
        var path = Path.Combine(folderPath, fileName);
        await Task.Run(() =>
        {
            ((Player)Player).TakeSnapshotToFile(path);
        });

        _ = _dispatcherQueue.TryEnqueue(async () =>
        {
            if (File.Exists(path))
            {
                var storageFile = await StorageFile.GetFileFromPathAsync(path).AsTask();
                _ = await Launcher.LaunchFileAsync(storageFile);
            }
        });
    }

    [RelayCommand]
    private void StartRecording()
    {
        if (Player == null || IsRecording)
        {
            return;
        }

        var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "哔哩录屏");
        if (!Directory.Exists(folderPath))
        {
            _ = Directory.CreateDirectory(folderPath);
        }

        var fileName = $"{DateTime.Now:yyyy_MM_dd_HH_mm_ss}";
        _recordingFileName = Path.Combine(folderPath, fileName);
        ((Player)Player).StartRecording(ref _recordingFileName);
    }

    [RelayCommand]
    private async Task StopRecordingAsync()
    {
        if (Player == null || !IsRecording)
        {
            return;
        }

        ((Player)Player).StopRecording();
        if (File.Exists(_recordingFileName))
        {
            var file = await StorageFile.GetFileFromPathAsync(_recordingFileName).AsTask();
            await Launcher.LaunchFileAsync(file);
        }
    }

    private void LoadDashVideoSource(bool onlyAudio)
    {
        var playItem = new PlaylistItem
        {
            Title = "video",
        };

        if (_video != null)
        {
            playItem.Tag.Add("video", _video);
        }

        if (_audio != null)
        {
            playItem.Tag.Add("audio", _audio);
        }

        playItem.Tag.Add("onlyAudio", onlyAudio);
        var cookie = AuthorizeProvider.GetCookieString();
        if (!string.IsNullOrEmpty(cookie))
        {
            playItem.Tag.Add("cookie", cookie);
        }

        ((Player)Player).OpenAsync(playItem);
    }

    private void LoadDashLiveSource(string url, bool onlyAudio)
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

            playItem.Tag.Add("onlyAudio", onlyAudio);
            ((Player)Player).OpenAsync(playItem);
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
        _ = _dispatcherQueue.TryEnqueue(() =>
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

    private void OnPlayerBufferingCompleted(object sender, BufferingCompletedArgs e)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (Player == null)
            {
                return;
            }

            if (e.Success)
            {
                Status = ConvertPlayerStatus(((Player)Player).Status);
                var arg = new MediaStateChangedEventArgs(Status, string.Empty);
                StateChanged?.Invoke(this, arg);
            }
            else
            {
                Status = PlayerStatus.Failed;
                var arg = new MediaStateChangedEventArgs(Status, e.Error);
                StateChanged?.Invoke(this, arg);
                LogException(new Exception($"缓冲失败: {e.Error}"));
            }
        });
    }

    private void OnPlayerBufferingStarted(object sender, EventArgs e)
    {
        _ = _dispatcherQueue.TryEnqueue(() =>
        {
            if (Player == null)
            {
                return;
            }

            Status = PlayerStatus.Buffering;
            var arg = new MediaStateChangedEventArgs(Status, string.Empty);
            StateChanged?.Invoke(this, arg);
        });
    }

    private void OnPlayerPlaybackStopped(object sender, PlaybackStoppedArgs e)
    {
        _ = _dispatcherQueue.TryEnqueue(() =>
        {
            if (Player == null)
            {
                return;
            }

            if (e.Success)
            {
                Status = ((Player)Player).Status == Libs.Flyleaf.MediaPlayer.Status.Paused ? PlayerStatus.Pause : PlayerStatus.End;
                if (Status == PlayerStatus.End)
                {
                    MediaEnded?.Invoke(this, EventArgs.Empty);
                }

                var arg = new MediaStateChangedEventArgs(Status, string.Empty);
                StateChanged?.Invoke(this, arg);
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
        _ = _dispatcherQueue.TryEnqueue(() =>
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
        if (e.PropertyName == "Status")
        {
            if (Player == null)
            {
                Status = PlayerStatus.NotLoad;
                StateChanged?.Invoke(this, new MediaStateChangedEventArgs(Status, null));
                return;
            }

            Status = ConvertPlayerStatus(((Player)Player).Status);

            var arg = new MediaStateChangedEventArgs(Status, null);
            StateChanged?.Invoke(this, arg);
        }
        else if (e.PropertyName == "CurTime")
        {
            var duration = Player is Player player ? player.Duration : 0;
            PositionChanged?.Invoke(this, new MediaPositionChangedEventArgs(Position, TimeSpan.FromTicks(duration)));
        }
        else if (e.PropertyName == "IsRecording")
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                IsRecording = ((Player)Player).IsRecording;

                if (!IsRecording)
                {
                    _recordingFileName = string.Empty;
                }
            });
        }
        else if (e.PropertyName == "CanPlay")
        {
            OnPropertyChanged(nameof(IsPlayerReady));
        }
    }

    [RelayCommand]
    private void Clear()
    {
        if (Player is IDisposable disposable)
        {
            disposable.Dispose();
        }

        Status = PlayerStatus.NotLoad;
    }
}
