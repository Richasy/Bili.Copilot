// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using WebDav;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// WebDav 页面视图模型.
/// </summary>
public sealed partial class WebDavPageViewModel
{
    private IWebDavClient _client;
    private WebDavConfig _config;

    [ObservableProperty]
    private bool _isInvalidConfig;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isItemsEmpty;

    [ObservableProperty]
    private bool _isListLayout;

    /// <summary>
    /// 路径段集合.
    /// </summary>
    public ObservableCollection<WebDavPathSegment> PathSegments { get; }

    /// <summary>
    /// 当前项目集合.
    /// </summary>
    public ObservableCollection<WebDavStorageItemViewModel> CurrentItems { get; }
}
