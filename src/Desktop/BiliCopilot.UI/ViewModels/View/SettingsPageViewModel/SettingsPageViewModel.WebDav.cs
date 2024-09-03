﻿// Copyright (c) Bili Copilot. All rights reserved.

using System.Text.Json;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using CommunityToolkit.Mvvm.Input;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 设置页面视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel
{
    [RelayCommand]
    private async Task InitializeWebDavConfigAsync()
    {
        IsWebDavEnabled = SettingsToolkit.ReadLocalSetting(SettingNames.IsWebDavEnabled, false);
        WebDavConfigs.Clear();
        var configList = await FileToolkit.ReadLocalDataAsync<List<WebDavConfig>>("__webdav.json", "[]");
        IsWebDavEmpty = configList.Count == 0;
        if (!IsWebDavEmpty)
        {
            configList.ForEach(WebDavConfigs.Add);
        }

        var selectedDav = SettingsToolkit.ReadLocalSetting(SettingNames.SelectedWebDavConfigId, string.Empty);
        SelectedWebDav = !string.IsNullOrEmpty(selectedDav)
            ? WebDavConfigs.FirstOrDefault(p => p.Id.Equals(selectedDav))
            : WebDavConfigs.FirstOrDefault();
    }

    [RelayCommand]
    private async Task AddWebDavConfigAsync(WebDavConfig webDavConfig)
    {
        WebDavConfigs.Add(webDavConfig);
        var json = JsonSerializer.Serialize(WebDavConfigs.ToList());
        await FileToolkit.WriteLocalDataAsync("__webdav.json", json);
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
        await FileToolkit.WriteLocalDataAsync("__webdav.json", json);
        IsWebDavEmpty = WebDavConfigs.Count == 0;
        SelectedWebDav = config;
    }

    [RelayCommand]
    private async Task RemoveWebDavConfigAsync(WebDavConfig webDavConfig)
    {
        WebDavConfigs.Remove(webDavConfig);
        var json = JsonSerializer.Serialize(WebDavConfigs.ToList());
        await FileToolkit.WriteLocalDataAsync("__webdav.json", json);
        IsWebDavEmpty = WebDavConfigs.Count == 0;

        if (SelectedWebDav?.Equals(webDavConfig) == true)
        {
            SelectedWebDav = WebDavConfigs.FirstOrDefault();
        }
    }
}
