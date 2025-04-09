// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using CommunityToolkit.Mvvm.Input;

namespace BiliCopilot.UI.ViewModels.Core;

public sealed partial class MpvPlayerWindowViewModel
{
    [RelayCommand]
    private async Task IncreaseVolumeAsync()
    {
        var newVolume = Math.Min(100, Volume + 5);
        if (Math.Abs(newVolume - Volume) > 1)
        {
            Volume = newVolume;
            await Client.SetVolumeAsync(newVolume);
        }
    }

    [RelayCommand]
    private async Task DecreaseVolumeAsync()
    {
        var newVolume = Math.Max(0, Volume - 5);
        if (Math.Abs(newVolume - Volume) > 1)
        {
            Volume = newVolume;
            await Client.SetVolumeAsync(newVolume);
        }
    }

    [RelayCommand]
    private async Task IncreaseSpeedAsync()
    {
        var newSpeed = Math.Min(2, Speed + 0.25);
        if (Math.Abs(newSpeed - Speed) > 0.01)
        {
            Speed = newSpeed;
            await Client.SetSpeedAsync(newSpeed);
        }
    }

    [RelayCommand]
    private async Task DecreaseSpeedAsync()
    {
        var newSpeed = Math.Max(0.1, Speed - 0.25);
        if (Math.Abs(newSpeed - Speed) > 0.01)
        {
            Speed = newSpeed;
            await Client.SetSpeedAsync(newSpeed);
        }
    }

    [RelayCommand]
    private async Task BackToDefaultModeAsync()
    {
        if (IsFullScreen)
        {
            await Client.SetFullScreenState(false);
        }
        else if (IsCompactOverlay)
        {
            await Client.SetCompactOverlayState(false);
        }
    }

    [RelayCommand]
    private async Task ForwardSkipAsync()
    {
        var seconds = SettingsToolkit.ReadLocalSetting(SettingNames.SingleFastForwardAndRewindSpan, 15d);
        if (seconds <= 0 || CurrentPosition <= 0)
        {
            return;
        }

        var pos = Math.Min(Duration, CurrentPosition + seconds);
        if (Math.Abs(pos - CurrentPosition) > 1)
        {
            await Client.SetCurrentPositionAsync(pos);
        }
    }

    [RelayCommand]
    private async Task BackwardSkipAsync()
    {
        var seconds = SettingsToolkit.ReadLocalSetting(SettingNames.SingleFastForwardAndRewindSpan, 15d);
        if (seconds <= 0 || CurrentPosition <= 0)
        {
            return;
        }

        var pos = Math.Max(0, CurrentPosition - seconds);
        await Client.SetCurrentPositionAsync(pos);
    }

    [RelayCommand]
    private async Task ToggleCompactOverlayAsync()
        => await Client.SetCompactOverlayState(!IsCompactOverlay);

    [RelayCommand]
    private async Task ToggleFullScreenAsync()
        => await Client.SetFullScreenState(!IsFullScreen);
}
