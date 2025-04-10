// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 播放器页面视图模型基类.
/// </summary>
public abstract partial class PlayerPageViewModelBase : LayoutPageViewModelBase
{
    [ObservableProperty]
    private PlayerViewModelBase _player;

    [ObservableProperty]
    private bool _isSeparatorWindowPlayer;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerPageViewModelBase"/> class.
    /// </summary>
    protected PlayerPageViewModelBase()
    {
        var playerType = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerType, PlayerType.Island);
        if (playerType == PlayerType.Mpv)
        {
            playerType = PlayerType.Island;
            SettingsToolkit.WriteLocalSetting(SettingNames.PlayerType, playerType);
        }

        Player = playerType switch
        {
            PlayerType.External => new ExternalPlayerViewModel(),
            _ => new NativePlayerViewModel(),
        };

        Player.AttachedWindow = this.Get<AppViewModel>().ActivatedWindow;
    }
}
