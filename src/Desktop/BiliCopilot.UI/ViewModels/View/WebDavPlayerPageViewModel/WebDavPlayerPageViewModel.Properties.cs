// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// WebDav 播放页面视图模型.
/// </summary>
public sealed partial class WebDavPlayerPageViewModel
{
    private readonly ILogger<WebDavPlayerPageViewModel> _logger;

    [ObservableProperty]
    private WebDavStorageItemViewModel _current;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private IReadOnlyList<WebDavStorageItemViewModel> _playlist;
}
