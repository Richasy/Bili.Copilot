// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace BiliCopilot.UI.ViewModels.Core;

public sealed partial class PlayerViewModel
{
    [RelayCommand]
    private async Task InitializeDanmakuAsync()
    {
        IsDanmakuEnabled = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsDanmakuEnabled, false);
        IsDanmakuControlVisible = IsDanmakuEnabled;
        if (!IsDanmakuEnabled)
        {
            return;
        }

        IsDanmakuLoading = true;
        
        try
        {
            // TODO: 初始化弹幕.
            await Task.CompletedTask;
            IsDanmakuLoading = false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize danmaku.");
            IsDanmakuLoading = false;
            IsDanmakuControlVisible = false;
        }
    }

    [RelayCommand]
    private async Task ReloadDanmakuAsync()
    {
        if (Player is null || Window is null || Window.IsClosed || !Player.IsPlaybackInitialized || Danmaku is null)
        {
            return;
        }

        Danmaku.ClearAll();
        await InitializeDanmakuAsync();
    }
}
