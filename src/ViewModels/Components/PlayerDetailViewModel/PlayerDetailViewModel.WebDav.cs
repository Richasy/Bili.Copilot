// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 媒体播放器视图模型.
/// </summary>
public sealed partial class PlayerDetailViewModel
{
    private async Task LoadWebDavAsync()
    {
        ResetPlayer();
        InitializePlayer();

        try
        {
            await Player.SetWebDavAsync(_webDavVideo);
            StartTimers();
        }
        catch (Exception ex)
        {
            IsError = true;
            ErrorText = ResourceToolkit.GetLocalizedString(StringNames.RequestVideoFailed);
            LogException(ex);
        }
    }

    private async Task RefreshWebDavAsync()
    {
        var needResume = Status == PlayerStatus.Playing;
        _initializeProgress = Player.Position;

        Player.Stop();
        await LoadWebDavAsync();
        if (needResume)
        {
            Player.Play();
        }
    }
}
