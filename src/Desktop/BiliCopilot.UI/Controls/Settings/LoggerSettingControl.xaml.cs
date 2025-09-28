// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.MpvKernel;
using Windows.Storage;
using Windows.System;

namespace BiliCopilot.UI.Controls.Settings;

/// <summary>
/// 日志设置控件.
/// </summary>
public sealed partial class LoggerSettingControl : SettingsPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerSettingControl"/> class.
    /// </summary>
    public LoggerSettingControl() => InitializeComponent();

    private static string LoggerFolder => Path.Combine(ApplicationData.Current.LocalFolder.Path, "Logger");

    private async void OnOpenLoggerFolderButtonClick(object sender, RoutedEventArgs e)
    {
        var folder = await StorageFolder.GetFolderFromPathAsync(LoggerFolder).AsTask();
        _ = await Launcher.LaunchFolderAsync(folder).AsTask();
    }

    protected override void OnControlLoaded()
    {
        var index = ViewModel.LogLevel switch
        {
            MpvLogLevel.V => 3,
            MpvLogLevel.Info => 2,
            MpvLogLevel.Warn => 1,
            MpvLogLevel.Error => 0,
            _ => 0
        };
        LevelComboBox.SelectedIndex = index;
    }

    private void OnLevelChanged(object sender, SelectionChangedEventArgs e)
    {
        var index = LevelComboBox.SelectedIndex;
        if (index >= 0 && IsLoaded)
        {
            ViewModel.LogLevel = index switch
            {
                3 => MpvLogLevel.V,
                2 => MpvLogLevel.Info,
                1 => MpvLogLevel.Warn,
                _ => MpvLogLevel.Error
            };
        }
    }
}
