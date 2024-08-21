// Copyright (c) Bili Copilot. All rights reserved.

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
        InitializeDecode();
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

        if (!IsLive)
        {
            Player.ResetDuration();
            Duration = Convert.ToInt32(Player.Duration!.Value.TotalSeconds);
        }
    }
}
