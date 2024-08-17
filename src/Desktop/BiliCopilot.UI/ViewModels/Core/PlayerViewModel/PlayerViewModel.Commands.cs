// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Windows.Storage;
using Windows.System;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 播放器视图模型.
/// </summary>
public sealed partial class PlayerViewModel
{
    [RelayCommand]
    private async Task TogglePlayPauseAsync()
    {
        if (IsPaused && !Player.IsMediaLoaded() && Math.Abs(Position - Duration) <= 1)
        {
            await SetPlayDataAsync(_videoUrl, _audioUrl, true, 0);
            return;
        }

        await Player.ExecuteAfterMediaLoadedAsync("cycle pause");
    }

    [RelayCommand]
    private async Task SeekAsync(double value)
    {
        if (Math.Abs(value - Position) < 3 || IsPlayerDataLoading || value > Duration)
        {
            return;
        }

        // 如果媒体已经播放结束，尝试重新加载媒体.
        if (!Player.IsMediaLoaded())
        {
            await SetPlayDataAsync(_videoUrl, _audioUrl, _autoPlay, Convert.ToInt32(value));
            return;
        }

        try
        {
            Player.Seek(TimeSpan.FromSeconds(value));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "尝试跳转视频时失败.");
        }
    }

    [RelayCommand]
    private void SetVolume(int value)
    {
        if (!Player.IsMediaLoaded())
        {
            return;
        }

        if (value < 0)
        {
            value = 0;
        }
        else if (value > 100)
        {
            value = 100;
        }

        if (Volume != value)
        {
            Volume = value;
            Player.SetVolume(value);
            SettingsToolkit.WriteLocalSetting(SettingNames.PlayerVolume, value);
        }
    }

    [RelayCommand]
    private void SetSpeed(double speed)
    {
        if (!Player.IsMediaLoaded() || speed > MaxSpeed)
        {
            return;
        }

        if (Speed != speed)
        {
            Speed = speed;
            Player.SetSpeed(speed);
            SettingsToolkit.WriteLocalSetting(SettingNames.PlayerSpeed, speed);
        }
    }

    [RelayCommand]
    private void ToggleFullScreen()
    {
        IsFullScreen = !IsFullScreen;
        if (IsFullScreen)
        {
            this.Get<AppViewModel>().MakeCurrentWindowEnterFullScreenCommand.Execute(default);
        }
        else
        {
            this.Get<AppViewModel>().MakeCurrentWindowExitFullScreenCommand.Execute(default);
        }
    }

    [RelayCommand]
    private void ToggleCompactOverlay()
    {
        IsCompactOverlay = !IsCompactOverlay;
        if (IsCompactOverlay)
        {
            this.Get<AppViewModel>().MakeCurrentWindowEnterCompactOverlayCommand.Execute(default);
        }
        else
        {
            this.Get<AppViewModel>().MakeCurrentWindowExitCompactOverlayCommand.Execute(default);
        }
    }

    [RelayCommand]
    private void IncreaseSpeed()
    {
        if (IsLive)
        {
            return;
        }

        var speed = Speed + 0.5;
        if (speed > MaxSpeed)
        {
            speed = MaxSpeed;
        }

        SetSpeed(speed);
    }

    [RelayCommand]
    private void DecreaseSpeed()
    {
        if (IsLive)
        {
            return;
        }

        var speed = Speed - 0.5;
        if (speed < 0.5)
        {
            speed = 0.5;
        }

        SetSpeed(speed);
    }

    [RelayCommand]
    private void IncreaseVolume()
    {
        var volume = Volume + 5;
        if (volume > 100)
        {
            volume = 100;
        }

        SetVolume(volume);
    }

    [RelayCommand]
    private void DecreaseVolume()
    {
        var volume = Volume - 5;
        if (volume < 0)
        {
            volume = 0;
        }

        SetVolume(volume);
    }

    [RelayCommand]
    private void BackToDefaultMode()
    {
        if (IsFullScreen)
        {
            ToggleFullScreen();
        }
        else if (IsCompactOverlay)
        {
            ToggleCompactOverlay();
        }
    }

    [RelayCommand]
    private void ForwardSkip()
    {
        var seconds = SettingsToolkit.ReadLocalSetting(SettingNames.SingleFastForwardAndRewindSpan, 15d);
        if (seconds <= 0 || !Player.IsMediaLoaded())
        {
            return;
        }

        _dispatcherQueue.TryEnqueue(() =>
        {
            if (Duration - Position < seconds)
            {
                SeekCommand.Execute(Duration);
            }
            else
            {
                SeekCommand.Execute(Position + seconds);
            }
        });
    }

    [RelayCommand]
    private void BackwardSkip()
    {
        var seconds = SettingsToolkit.ReadLocalSetting(SettingNames.SingleFastForwardAndRewindSpan, 15d);
        if (seconds <= 0 || !Player.IsMediaLoaded())
        {
            return;
        }

        _dispatcherQueue.TryEnqueue(() =>
        {
            if (Position < seconds)
            {
                SeekCommand.Execute(0);
            }
            else
            {
                SeekCommand.Execute(Position - seconds);
            }
        });
    }

    [RelayCommand]
    private async Task TakeScreenshotAsync()
    {
        try
        {
            var fileName = DateTimeOffset.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".png";
            var path = Path.Combine(GetScreenshotFolderPath(), fileName);
            if(!Directory.Exists(GetScreenshotFolderPath()))
            {
                Directory.CreateDirectory(GetScreenshotFolderPath());
            }

            await Player.TakeScreenshotAsync(path);
            if (File.Exists(path))
            {
                var file = await StorageFile.GetFileFromPathAsync(path);
                await Launcher.LaunchFileAsync(file).AsTask();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "尝试截图时失败.");
        }
    }

    [RelayCommand]
    private async Task ReloadAsync()
    {
        if (string.IsNullOrEmpty(_videoUrl) && string.IsNullOrEmpty(_audioUrl))
        {
            return;
        }

        await SetPlayDataAsync(_videoUrl, _audioUrl, _autoPlay, Position);
    }
}
