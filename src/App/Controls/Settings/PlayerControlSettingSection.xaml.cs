// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.App.Controls.Settings;

/// <summary>
/// 播放器设置区块.
/// </summary>
public sealed partial class PlayerControlSettingSection : SettingSection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerControlSettingSection"/> class.
    /// </summary>
    public PlayerControlSettingSection()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var isManual = ViewModel.IsPlayerControlModeManual;
        PlayerControlModeComboBox.SelectedIndex = isManual ? 1 : 0;
    }

    private void OnPlayerControlModeChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        ViewModel.IsPlayerControlModeManual = PlayerControlModeComboBox.SelectedIndex == 1;
    }
}
