// Copyright (c) Bili Copilot. All rights reserved.

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

    private async void OnOpenLoggerFolderButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var folder = await StorageFolder.GetFolderFromPathAsync(App.LoggerFolder).AsTask();
        _ = await Launcher.LaunchFolderAsync(folder).AsTask();
    }

    private async void OnCleanLoggerButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var folder = await StorageFolder.GetFolderFromPathAsync(App.LoggerFolder).AsTask();
        await folder.DeleteAsync(StorageDeleteOption.PermanentDelete).AsTask();
        _ = await ApplicationData.Current.LocalFolder.CreateFolderAsync(App.LoggerFolder, CreationCollisionOption.OpenIfExists).AsTask();
    }
}
