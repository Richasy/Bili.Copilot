// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using Microsoft.Extensions.Logging;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 岛播放器视图模型.
/// </summary>
public sealed partial class IslandPlayerViewModel
{
    /// <inheritdoc/>
    protected override void BeforeLoadPlayData()
        => Player.RerunEventLoop();

    /// <inheritdoc/>
    protected override async Task OnCloseAsync()
    {
        if (IsMediaLoaded())
        {
            Player?.Pause();
        }

        await Player?.DisposeAsync();
        _playerWindow?.Dispose();
        _overlayWindow?.Dispose();
    }

    /// <inheritdoc/>
    protected override bool IsMediaLoaded()
        => Player?.IsMediaLoaded() == true;

    /// <inheritdoc/>
    protected override Task OnTogglePlayPauseAsync()
        => Player.ExecuteAfterMediaLoadedAsync("cycle pause");

    /// <inheritdoc/>
    protected override Task ForcePlayAsync()
    {
        Player.Play();
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override Task OnSeekAsync(TimeSpan position)
    {
        Player.Seek(position);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override void OnSetVolume(int value)
        => Player?.SetVolume(value);

    /// <inheritdoc/>
    protected override void OnSetSpeed(double speed)
    {
        Player?.SetSpeed(speed);
        _speedAction?.Invoke(speed);
    }

    /// <inheritdoc/>
    protected override Task OnTakeScreenshotAsync(string path)
        => Player.TakeScreenshotAsync(path);

    /// <inheritdoc/>
    protected override async Task OnLoadPlayDataAsync()
    {
        if (IsWebDav)
        {
            LoadWebDavAuthorization();
        }

        UpdateState(Models.Constants.PlayerState.None);

        try
        {
            var fileUrl = string.IsNullOrEmpty(_videoUrl) ? _audioUrl : _videoUrl;
            if (Player?.IsDisposed != false || string.IsNullOrEmpty(fileUrl))
            {
                _logger.LogWarning("播放器已被释放或播放地址为空，无法加载播放数据.");
                return;
            }

            List<string> commandArgs = ["loadfile", $"\"{fileUrl}\"", "replace", "0"];
            List<string> commandOptions = [];
            if (!_autoPlay)
            {
                commandOptions.Add("pause=yes");
            }

            if (Position > 0)
            {
                commandOptions.Add($"start={Position}.0");
            }

            var speed = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.PlayerSpeed, 1d);
            if (Math.Abs(speed - 1) >= 0.1)
            {
                commandOptions.Add($"speed={Math.Round(speed, 2)}");
            }

            if (!string.IsNullOrEmpty(_videoUrl) && !string.IsNullOrEmpty(_audioUrl))
            {
                commandOptions.Add($"audio-file=\"{_audioUrl}\"");
            }

            commandArgs.Add(string.Join(',', commandOptions));
            await Player.Client.ExecuteAsync([.. commandArgs]);

            if (!IsLive && !IsWebDav)
            {
                await Task.Delay(500);
                Player.ResetDuration();
                if (!_autoPlay)
                {
                    IsPaused = true;
                }

                Duration = Convert.ToInt32(Player.Duration!.Value.TotalSeconds);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加载播放数据时失败.");
            UpdateState(Models.Constants.PlayerState.Failed);
        }
    }
}
