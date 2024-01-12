// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using WebDAVClient;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// WebDav 页面视图模型.
/// </summary>
public sealed partial class WebDavPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavPageViewModel"/> class.
    /// </summary>
    private WebDavPageViewModel()
    {
        CurrentItems = new ObservableCollection<WebDavStorageItemViewModel>();
        PathSegments = new ObservableCollection<WebDavPathSegment>();
        IsListLayout = SettingsToolkit.ReadLocalSetting(Models.Constants.App.SettingNames.IsWebDavListLayout, true);

        AttachIsRunningToAsyncCommand(p => IsLoading = p, LoadPathCommand);
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        var currentConfigId = SettingsToolkit.ReadLocalSetting(Models.Constants.App.SettingNames.SelectedWebDav, string.Empty);
        if (string.IsNullOrEmpty(currentConfigId))
        {
            IsInvalidConfig = true;
            return;
        }

        var configList = await FileToolkit.ReadLocalDataAsync<List<WebDavConfig>>(AppConstants.WebDavConfigFileName, "[]");
        var config = configList.FirstOrDefault(p => p.Id == currentConfigId);
        if (config is null)
        {
            IsInvalidConfig = true;
            return;
        }

        if (config.Id == _config?.Id)
        {
            return;
        }

        if (_client != null)
        {
            _client.Dispose();
            _client = default;
        }

        _config = config;
        if (!string.IsNullOrEmpty(_config.UserName))
        {
            var cred = new NetworkCredential(_config.UserName, _config.Password);
            _client = new Client(cred);
        }
        else
        {
            _client = new Client();
        }

        _client.Server = _config.Host + ":" + _config.Port;
        PathSegments.Clear();
        PathSegments.Add(new WebDavPathSegment { Name = ResourceToolkit.GetLocalizedString(Models.Constants.App.StringNames.RootDirectory), Path = "/" });
        TryClear(CurrentItems);
        LoadPathCommand.Execute("/");
    }

    [RelayCommand]
    private async Task LoadPathAsync(string path)
    {
        var lastPath = PathSegments.Last();
        if (lastPath.Path != path)
        {
            var lastEqualIndex = -1;
            for (var i = 0; i < PathSegments.Count; i++)
            {
                if (PathSegments[i].Path == path)
                {
                    lastEqualIndex = i;
                    break;
                }
            }

            if (lastEqualIndex != -1)
            {
                for (var i = PathSegments.Count - 1; i > lastEqualIndex; i--)
                {
                    PathSegments.RemoveAt(i);
                }
            }
            else
            {
                var directoryName = path.Split('/', StringSplitOptions.RemoveEmptyEntries).Last();
                PathSegments.Add(new WebDavPathSegment { Name = Uri.UnescapeDataString(directoryName), Path = path });
            }
        }

        try
        {
            TryClear(CurrentItems);
            var items = (await _client.List(path)).ToList();
            items = items
                .Where(p => p.IsCollection || p.ContentType.StartsWith("video"))
                .OrderBy(p => p.IsCollection)
                .ThenBy(p => p.DisplayName)
                .ToList();
            foreach (var item in items)
            {
                CurrentItems.Add(new WebDavStorageItemViewModel(item));
            }
        }
        catch (Exception ex)
        {
            LogException(ex);
            AppViewModel.Instance.ShowMessage(ex.Message);
        }

        IsItemsEmpty = CurrentItems.Count == 0;
    }

    partial void OnIsListLayoutChanged(bool value)
        => SettingsToolkit.ReadLocalSetting(Models.Constants.App.SettingNames.IsWebDavListLayout, value);
}
