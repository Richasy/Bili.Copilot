// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.Player;

namespace Bili.Copilot.App.Controls.Settings;

/// <summary>
/// 播放器模式设置区块.
/// </summary>
public sealed partial class PlayerModeSettingSection : SettingSection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerModeSettingSection"/> class.
    /// </summary>
    public PlayerModeSettingSection()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var playerWindowSelectIndex = (int)ViewModel.PlayerWindowBehavior;
        PlayerWindowBehaviorComboBox.SelectedIndex = playerWindowSelectIndex;
    }

    private void OnPlayerWindowBehaviorComboBoxSelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        ViewModel.PlayerWindowBehavior = (PlayerWindowBehavior)PlayerWindowBehaviorComboBox.SelectedIndex;
    }
}
