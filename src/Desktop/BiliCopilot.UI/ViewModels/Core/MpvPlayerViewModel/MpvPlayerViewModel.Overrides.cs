// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 播放器视图模型.
/// </summary>
public sealed partial class MpvPlayerViewModel
{
    /// <inheritdoc/>
    protected override void BeforeLoadPlayData()
        => Player.RerunEventLoop();

    /// <inheritdoc/>
    protected override Task OnCloseAsync()
        => Player?.DisposeAsync() ?? Task.CompletedTask;

    /// <inheritdoc/>
    protected override bool IsMediaLoaded()
        => Player?.IsMediaLoaded() == true;

    /// <inheritdoc/>
    protected override Task OnTogglePlayPauseAsync()
        => Player.ExecuteAfterMediaLoadedAsync("cycle pause");

    /// <inheritdoc/>
    protected override Task OnSeekAsync(TimeSpan position)
    {
        Player.Seek(position);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override void OnSetVolume(int value)
        => Player.SetVolume(value);

    /// <inheritdoc/>
    protected override void OnSetSpeed(double speed)
        => Player.SetSpeed(speed);

    /// <inheritdoc/>
    protected override Task OnTakeScreenshotAsync(string path)
        => Player.TakeScreenshotAsync(path);

    /// <inheritdoc/>
    protected override async Task OnLoadPlayDataAsync()
    {
        if (!_autoPlay)
        {
            Player.Client.SetOption("pause", "yes");
        }
        else
        {
            Player.Client.SetOption("pause", "no");
        }

        Player.Client.SetOption("start", Position.ToString());

        if (!string.IsNullOrEmpty(_videoUrl))
        {
            await Player.Client.ExecuteAsync(["loadfile", _videoUrl, "replace"]);

            if (!string.IsNullOrEmpty(_audioUrl))
            {
                await WaitUntilAddAudioAsync(_audioUrl);
            }
        }
        else if (!string.IsNullOrEmpty(_audioUrl))
        {
            await Player.Client.ExecuteAsync(["loadfile", _audioUrl, "replace"]);
        }

        SetVolumeCommand.Execute(SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.PlayerVolume, 100));
        if (!IsLive)
        {
            Player.ResetDuration();
            SetSpeedCommand.Execute(SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.PlayerSpeed, 1d));
            Duration = Convert.ToInt32(Player.Duration!.Value.TotalSeconds);
        }
    }
}
