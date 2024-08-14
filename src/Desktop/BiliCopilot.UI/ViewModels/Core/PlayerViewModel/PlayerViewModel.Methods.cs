﻿// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 播放器视图模型.
/// </summary>
public sealed partial class PlayerViewModel
{
    private async Task TryLoadPlayDataAsync()
    {
        if (string.IsNullOrEmpty(_videoUrl) && string.IsNullOrEmpty(_audioUrl))
        {
            return;
        }

        if (!_autoPlay)
        {
            Player.Client.SetOption("pause", "yes");
        }

        IsPlayerDataLoading = true;
        IsPaused = true;
        if (!string.IsNullOrEmpty(_videoUrl))
        {
            await Player.Client.ExecuteAsync(["loadfile", _videoUrl, "replace"]);

            if (!string.IsNullOrEmpty(_audioUrl))
            {
                await Player.Client.ExecuteAsync(["audio-add", _audioUrl]);
            }
        }
        else if (!string.IsNullOrEmpty(_audioUrl))
        {
            await Player.Client.ExecuteAsync(["loadfile", _audioUrl, "replace"]);
        }

        PlayerDataLoaded?.Invoke(this, EventArgs.Empty);
        IsPlayerDataLoading = false;
    }
}
