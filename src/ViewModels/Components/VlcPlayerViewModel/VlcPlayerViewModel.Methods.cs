// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.Constants.App;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using Windows.Storage;
using Windows.System;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// VLC 播放器视图模型.
/// </summary>
public sealed partial class VlcPlayerViewModel
{
    /// <inheritdoc/>
    public Models.App.Other.MediaStats GetMediaInformation()
    {
        var player = (MediaPlayer)Player;
        return default;
    }

    [RelayCommand]
    private static void StartRecording()
    {
        // 暂不支持录制.
        return;
    }

    [RelayCommand]
    private static Task StopRecordingAsync()
        => Task.CompletedTask;

    private static HttpClient GetVideoClient()
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Referer", "https://www.bilibili.com");
        httpClient.DefaultRequestHeaders.Add("User-Agent", ServiceConstants.DefaultUserAgentString);
        var cookie = AuthorizeProvider.GetCookieString();
        if (!string.IsNullOrEmpty(cookie))
        {
            httpClient.DefaultRequestHeaders.Add("Cookie", cookie);
        }

        return httpClient;
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
            ((MediaPlayer)Player).TakeSnapshot(0, path, 0, 0);
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

    private async void LoadDashVideoSourceAsync(bool onlyAudio)
    {
        var httpClient = GetVideoClient();
        if (onlyAudio && !string.IsNullOrEmpty(_audio?.BaseUrl))
        {
            _currentInput = await HttpMediaInput.CreateAsync(httpClient, new Uri(_audio.BaseUrl));
            _currentMedia = new Media(_libVlc, _currentInput);
            ((MediaPlayer)Player).Play(_currentMedia);
        }
        else
        {
            _currentInput = await HttpMediaInput.CreateAsync(httpClient, new Uri(_video.BaseUrl));
            _currentMedia = new Media(_libVlc, _currentInput);
            _currentMedia.AddOption(":http-referrer=https://www.bilibili.com");
            _currentMedia.AddOption($":http-user-agent={ServiceConstants.DefaultUserAgentString}");

            if (!string.IsNullOrEmpty(_audio?.BaseUrl))
            {
                _currentMedia.AddSlave(MediaSlaveType.Audio, 4, _audio.BaseUrl);
            }

            ((MediaPlayer)Player).Play(_currentMedia);
        }
    }

    private void LoadDashLiveSource(string url, bool onlyAudio)
    {
        try
        {
            _currentMedia = new Media(_libVlc, url, FromType.FromLocation);
            if (onlyAudio)
            {
                _currentMedia.AddOption(":no-video");
            }

            ((MediaPlayer)Player).Play(_currentMedia);
        }
        catch (Exception ex)
        {
            Status = PlayerStatus.Failed;
            var msg = ResourceToolkit.GetLocalizedString(StringNames.RequestLivePlayInformationFailed) + "\n" + ex.Message;
            StateChanged?.Invoke(this, new Models.App.Args.MediaStateChangedEventArgs(Status, msg));
            LogException(ex);
        }
    }

    private void OnTimeChanged(object sender, MediaPlayerTimeChangedEventArgs e)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            var duration = Player is MediaPlayer player ? player.Length : 0;
            PositionChanged?.Invoke(this, new Models.App.Args.MediaPositionChangedEventArgs(Position, TimeSpan.FromMilliseconds(duration)));
        });
    }

    private void OnStopped(object sender, EventArgs e)
    {
        _ = _dispatcherQueue.TryEnqueue(() =>
        {
            if (Status == PlayerStatus.End)
            {
                PositionChanged?.Invoke(this, new Models.App.Args.MediaPositionChangedEventArgs(Duration, Duration));
                return;
            }

            Status = PlayerStatus.NotLoad;
            StateChanged?.Invoke(this, new Models.App.Args.MediaStateChangedEventArgs(Status, string.Empty));
        });
    }

    private void OnPaused(object sender, EventArgs e)
    {
        _ = _dispatcherQueue.TryEnqueue(() =>
        {
            Status = PlayerStatus.Pause;
            StateChanged?.Invoke(this, new Models.App.Args.MediaStateChangedEventArgs(Status, string.Empty));
        });
    }

    private void OnPlaying(object sender, EventArgs e)
    {
        _ = _dispatcherQueue.TryEnqueue(() =>
        {
            Status = PlayerStatus.Playing;
            StateChanged?.Invoke(this, new Models.App.Args.MediaStateChangedEventArgs(Status, string.Empty));
        });
    }

    private void OnOpening(object sender, EventArgs e)
    {
        _ = _dispatcherQueue.TryEnqueue(() =>
        {
            Status = PlayerStatus.Buffering;
            StateChanged?.Invoke(this, new Models.App.Args.MediaStateChangedEventArgs(Status, string.Empty));
        });
    }

    private void OnBuffering(object sender, MediaPlayerBufferingEventArgs e)
    {
        _ = _dispatcherQueue.TryEnqueue(() =>
        {
            if (e.Cache < 100)
            {
                Status = PlayerStatus.Buffering;
                StateChanged?.Invoke(this, new Models.App.Args.MediaStateChangedEventArgs(Status, string.Empty));
            }
            else
            {
                Status = ((MediaPlayer)Player).IsPlaying ? PlayerStatus.Playing : PlayerStatus.Pause;
                StateChanged?.Invoke(this, new Models.App.Args.MediaStateChangedEventArgs(Status, string.Empty));
            }
        });
    }

    private void OnCorked(object sender, EventArgs e)
        => Debug.WriteLine("Corked");

    private void OnUncorked(object sender, EventArgs e)
        => Debug.WriteLine("Uncorked");

    private void OnAudioDevice(object sender, MediaPlayerAudioDeviceEventArgs e)
    {
        Debug.WriteLine($"Device name: {e.AudioDevice}");
    }

    private void OnSeekableChanged(object sender, MediaPlayerSeekableChangedEventArgs e)
    {
        _ = _dispatcherQueue.TryEnqueue(() =>
        {
            if (!_isOpened)
            {
                if (e.Seekable == 1)
                {
                    OnPropertyChanged(nameof(IsPlayerReady));
                    MediaOpened?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    Status = PlayerStatus.Failed;
                    var arg = new Models.App.Args.MediaStateChangedEventArgs(Status, _libVlc.LastLibVLCError ?? "初始化失败");
                    StateChanged?.Invoke(this, arg);
                    LogException(new Exception($"播放失败: {_libVlc.LastLibVLCError ?? "初始化失败"}"));
                }

                _isOpened = true;
            }
        });
    }

    private void OnEndReached(object sender, EventArgs e)
    {
        _ = _dispatcherQueue.TryEnqueue(() =>
        {
            Status = PlayerStatus.End;
            MediaEnded?.Invoke(this, EventArgs.Empty);
            StateChanged?.Invoke(this, new Models.App.Args.MediaStateChangedEventArgs(Status, string.Empty));

            if (IsLoop)
            {
                ((MediaPlayer)Player).SeekTo(TimeSpan.Zero);
                ((MediaPlayer)Player).Play();
            }
        });
    }

    private void OnError(object sender, EventArgs e)
    {
        _ = _dispatcherQueue.TryEnqueue(() =>
        {
            Status = PlayerStatus.Failed;
            var arg = new Models.App.Args.MediaStateChangedEventArgs(Status, _libVlc.LastLibVLCError ?? "未知错误");
            StateChanged?.Invoke(this, arg);
            LogException(new Exception($"VLC播放失败: {_libVlc.LastLibVLCError ?? "未知错误"}"));
        });
    }

    [RelayCommand]
    private void Clear()
    {
        _currentInput?.Close();
        _currentInput = null;

        if (_currentMedia != null)
        {
            _currentMedia.Dispose();
            _currentMedia = null;
        }

        Status = PlayerStatus.NotLoad;
    }
}
