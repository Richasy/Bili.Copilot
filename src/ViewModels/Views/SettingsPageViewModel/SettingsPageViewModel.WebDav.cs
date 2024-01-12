// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.App.Other;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 设置视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel
{
    [RelayCommand]
    private async Task InitializeWebDavConfigAsync()
    {
        IsWebDavEnabled = SettingsToolkit.ReadLocalSetting(Models.Constants.App.SettingNames.IsWebDavEnabled, false);
        TryClear(WebDavConfigs);
        var configList = await FileToolkit.ReadLocalDataAsync<List<WebDavConfig>>(AppConstants.WebDavConfigFileName, "[]");
        IsWebDavEmpty = configList.Count == 0;
        if (!IsWebDavEmpty)
        {
            configList.ForEach(WebDavConfigs.Add);
        }

        var selectedDav = SettingsToolkit.ReadLocalSetting(Models.Constants.App.SettingNames.SelectedWebDav, string.Empty);
        SelectedWebDav = !string.IsNullOrEmpty(selectedDav)
            ? WebDavConfigs.FirstOrDefault(p => p.Id.Equals(selectedDav))
            : WebDavConfigs.FirstOrDefault();
    }

    [RelayCommand]
    private async Task AddWebDavConfigAsync(WebDavConfig webDavConfig)
    {
        WebDavConfigs.Add(webDavConfig);
        var json = JsonSerializer.Serialize(WebDavConfigs.ToList());
        await FileToolkit.WriteLocalDataAsync(AppConstants.WebDavConfigFileName, json);
        IsWebDavEmpty = false;

        SelectedWebDav ??= WebDavConfigs.FirstOrDefault();
    }

    [RelayCommand]
    private async Task UpdateWebDavConfigAsync(WebDavConfig config)
    {
        var sourceConfig = WebDavConfigs.FirstOrDefault(p => p.Id.Equals(config.Id));
        if (sourceConfig is null)
        {
            return;
        }

        WebDavConfigs.Remove(sourceConfig);
        WebDavConfigs.Add(config);
        var json = JsonSerializer.Serialize(WebDavConfigs.ToList());
        await FileToolkit.WriteLocalDataAsync(AppConstants.WebDavConfigFileName, json);
        IsWebDavEmpty = WebDavConfigs.Count == 0;

        if (SelectedWebDav.Id.Equals(config.Id))
        {
            SelectedWebDav = null;
            SelectedWebDav = config;
        }
    }

    [RelayCommand]
    private async Task RemoveWebDavConfigAsync(WebDavConfig webDavConfig)
    {
        WebDavConfigs.Remove(webDavConfig);
        var json = JsonSerializer.Serialize(WebDavConfigs.ToList());
        await FileToolkit.WriteLocalDataAsync(AppConstants.WebDavConfigFileName, json);
        IsWebDavEmpty = WebDavConfigs.Count == 0;

        if (SelectedWebDav.Equals(webDavConfig))
        {
            SelectedWebDav = WebDavConfigs.FirstOrDefault();
        }
    }
}
