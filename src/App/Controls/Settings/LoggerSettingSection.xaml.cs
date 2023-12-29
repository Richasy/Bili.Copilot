// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.ViewModels;
using Windows.Storage;
using Windows.System;

namespace Bili.Copilot.App.Controls.Settings;

/// <summary>
/// 日志记录设置区块.
/// </summary>
public sealed partial class LoggerSettingSection : SettingSection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerSettingSection"/> class.
    /// </summary>
    public LoggerSettingSection() => InitializeComponent();

    private async void OnOpenLoggerFolderButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(ControllerConstants.Names.LoggerFolder, CreationCollisionOption.OpenIfExists).AsTask();
        _ = await Launcher.LaunchFolderAsync(folder);
    }

    private async void OnCleanLoggerButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(ControllerConstants.Names.LoggerFolder, CreationCollisionOption.OpenIfExists).AsTask();
        try
        {
            await folder.DeleteAsync(StorageDeleteOption.PermanentDelete).AsTask();
            _ = await ApplicationData.Current.LocalFolder.CreateFolderAsync(ControllerConstants.Names.LoggerFolder, CreationCollisionOption.OpenIfExists).AsTask();
        }
        catch (Exception)
        {
        }
        finally
        {
            AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(StringNames.LogEmptied), InfoType.Success);
        }
    }
}
