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
using WebDav;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// WebDav 页面视图模型.
/// </summary>
public sealed partial class WebDavPageViewModel : ViewModelBase, IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavPageViewModel"/> class.
    /// </summary>
    public WebDavPageViewModel()
    {
        CurrentItems = new ObservableCollection<WebDavStorageItemViewModel>();
        PathSegments = new ObservableCollection<WebDavPathSegment>();
        IsListLayout = SettingsToolkit.ReadLocalSetting(Models.Constants.App.SettingNames.IsWebDavListLayout, true);

        AttachIsRunningToAsyncCommand(p => IsLoading = p, LoadPathCommand);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _client?.Dispose();
        CurrentItems.Clear();
        PathSegments.Clear();
    }

    [RelayCommand]
    private async Task InitializeAsync(bool force = false)
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

        if (config.Id == _config?.Id && !force)
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
            var clientParameters = new WebDavClientParams
            {
                Credentials = new NetworkCredential(_config.UserName, _config.Password),
            };

            _client = new WebDavClient(clientParameters);
        }
        else
        {
            _client = new WebDavClient();
        }

        PathSegments.Clear();
        PathSegments.Add(new WebDavPathSegment { Name = ResourceToolkit.GetLocalizedString(Models.Constants.App.StringNames.RootDirectory), Path = "/" });
        TryClear(CurrentItems);

        var previousPath = SettingsToolkit.ReadLocalSetting(Models.Constants.App.SettingNames.WebDavLastPath, "/");
        if (string.IsNullOrEmpty(previousPath))
        {
            previousPath = "/";
        }

        var rootPath = new Uri(_config.Host).PathAndQuery.Split("?").First();
        if (previousPath.StartsWith(rootPath))
        {
            var relativePath = previousPath.Substring(rootPath.Length);
            var relativeSegs = relativePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < relativeSegs.Length - 1; i++)
            {
                var segment = new WebDavPathSegment { Name = Uri.UnescapeDataString(relativeSegs[i]), Path = rootPath.TrimEnd('/') + "/" + string.Join('/', relativeSegs.Take(i + 1)) };
                if (!PathSegments.Contains(segment))
                {
                    PathSegments.Add(segment);
                }
            }
        }

        LoadPathCommand.Execute(previousPath);
    }

    [RelayCommand]
    private async Task RefreshAsync()
        => await InitializeAsync(true);

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
            var server = AppToolkit.GetWebDavServer(_config.Host, _config.Port, path);
            var items = (await _client.Propfind(server + path)).Resources.ToList();
            var rootPath = new Uri(_config.Host).PathAndQuery.Split("?").First();
            items = items
                .Where(p => !p.IsHidden && p.Uri.Trim('/') != path.Trim('/') && p.Uri.Trim('/') != rootPath.Trim('/'))
                .OrderBy(p => p.IsCollection)
                .ThenBy(p => p.DisplayName)
                .ToList();
            foreach (var item in items)
            {
                CurrentItems.Add(new WebDavStorageItemViewModel(item));
            }

            SettingsToolkit.WriteLocalSetting(Models.Constants.App.SettingNames.WebDavLastPath, path);
        }
        catch (Exception ex)
        {
            LogException(ex);
            AppViewModel.Instance.ShowMessage(ex.Message);
        }

        IsItemsEmpty = CurrentItems.Count == 0;
    }

    [RelayCommand]
    private void OpenVideo(WebDavStorageItemViewModel item)
    {
        var list = CurrentItems.Where(p => !p.IsFolder && p.IsEnabled).ToList();
        foreach (var data in list)
        {
            data.IsSelected = data.Equals(item);
        }

        AppViewModel.Instance.OpenWebDavCommand.Execute(list);
    }

    partial void OnIsListLayoutChanged(bool value)
        => SettingsToolkit.ReadLocalSetting(Models.Constants.App.SettingNames.IsWebDavListLayout, value);
}
