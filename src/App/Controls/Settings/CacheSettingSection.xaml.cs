// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;
using Windows.Storage;

namespace Bili.Copilot.App.Controls.Settings;

/// <summary>
/// 缓存设置区块.
/// </summary>
public sealed partial class CacheSettingSection : SettingSection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CacheSettingSection"/> class.
    /// </summary>
    public CacheSettingSection() => InitializeComponent();

    private async void OnClearButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var cacheFolder = ApplicationData.Current.LocalCacheFolder;
        LoadingRing.Visibility = Visibility.Visible;
        ClearButton.IsEnabled = false;

        try
        {
            var children = await cacheFolder.GetItemsAsync();
            foreach (var child in children)
            {
                if (child is StorageFile file)
                {
                    await file.DeleteAsync();
                }
                else if (child is StorageFolder folder)
                {
                    await folder.DeleteAsync();
                }
            }

            AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(StringNames.CacheCleared), InfoType.Success);
        }
        catch
        {
        }
        finally
        {
            LoadingRing.Visibility = Visibility.Collapsed;
            ClearButton.IsEnabled = true;
        }
    }
}
