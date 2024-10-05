// Copyright (c) Bili Copilot. All rights reserved.

using System.Net;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.WinUI.Share.Base;
using Richasy.WinUI.Share.ViewModels;
using WebDav;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// WebDav 页面视图模型.
/// </summary>
public sealed partial class WebDavPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavPageViewModel"/> class.
    /// </summary>
    public WebDavPageViewModel(
        ILogger<WebDavPageViewModel> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取当前配置.
    /// </summary>
    /// <returns>配置.</returns>
    public WebDavConfig? GetCurrentConfig()
        => _config;

    [RelayCommand]
    private async Task InitializeAsync()
    {
        var localConfigId = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.SelectedWebDavConfigId, string.Empty);
        if (string.IsNullOrEmpty(localConfigId))
        {
            IsInvalidConfig = true;
            return;
        }

        var configList = await FileToolkit.ReadLocalDataAsync<List<WebDavConfig>>("__webdav.json", GlobalSerializeContext.Default.ListWebDavConfig, "[]");
        var config = configList.FirstOrDefault(p => p.Id.Equals(localConfigId));
        if (config is null)
        {
            IsInvalidConfig = true;
            return;
        }

        if (config.Id == _config?.Id)
        {
            return;
        }

        PathSegments.Clear();
        _client?.Dispose();
        _config = config;
        _client = !string.IsNullOrEmpty(_config.UserName)
            ? new WebDavClient(new WebDavClientParams
            {
                Credentials = new NetworkCredential(_config.UserName, _config.Password),
            })
            : new WebDavClient();

        if (PathSegments.Count == 0)
        {
            PathSegments.Add(new WebDavPathSegment { Name = ResourceToolkit.GetLocalizedString(StringNames.RootDirectory), Path = "/" });
        }

        LoadPathCommand.Execute(PathSegments.Last().Path);
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        _config = default;
        Items = default;
        IsItemsEmpty = false;
        IsInvalidConfig = false;
        await InitializeAsync();
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
            IsLoading = true;
            Items = default;
            var reqPath = path == "/" ? _config.ToString() : AppToolkit.GetWebDavServer(_config.GetServer(), path);
            var items = (await _client.Propfind(reqPath)).Resources.ToList();
            var rootPath = new Uri(_config.ToString()).PathAndQuery.Split("?").First();
            items = items
                .Where(p => !p.IsHidden && p.Uri.Trim('/') != path.Trim('/') && p.Uri.Trim('/') != rootPath.Trim('/'))
                .OrderBy(p => p.IsCollection)
                .ThenBy(p => Path.GetExtension(p.Uri))
                .ThenBy(p => p.DisplayName)
                .ToList();
            Items = items.Select(p => new WebDavStorageItemViewModel(p)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加载 WebDav 路径失败.");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ex.Message, InfoType.Error));
        }
        finally
        {
            IsLoading = false;
            IsItemsEmpty = Items is null || Items.Count == 0;
        }
    }
}
