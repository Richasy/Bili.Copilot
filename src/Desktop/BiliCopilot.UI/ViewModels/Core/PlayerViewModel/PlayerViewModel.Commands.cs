// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.Input;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 播放器视图模型.
/// </summary>
public sealed partial class PlayerViewModel
{
    [RelayCommand]
    private Task TogglePlayPauseAsync()
        => Player.ExecuteAfterMediaLoadedAsync("cycle pause");
}
