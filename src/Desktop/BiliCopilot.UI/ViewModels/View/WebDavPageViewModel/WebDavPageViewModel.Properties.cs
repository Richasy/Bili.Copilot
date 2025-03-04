// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using WebDav;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// WebDav 页面视图模型.
/// </summary>
public sealed partial class WebDavPageViewModel
{
    private readonly ILogger<WebDavPageViewModel> _logger;
    private IWebDavClient _client;
    private WebDavConfig _config;

    [ObservableProperty]
    private bool _isInvalidConfig;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isItemsEmpty;

    [ObservableProperty]
    private List<WebDavStorageItemViewModel> _items;

    /// <summary>
    /// 路径段集合.
    /// </summary>
    public ObservableCollection<WebDavPathSegment> PathSegments { get; } = new();
}
