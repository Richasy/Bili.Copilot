// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using WebDav;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// WEB DAV 源视图模型.
/// </summary>
public sealed partial class WebDavSourceViewModel
{
    private readonly ILogger<WebDavSourceViewModel> _logger;

    private WebDavResource? _cachedVideo;
    private double _initialProgress;
    private double _lastPosition;

    private string _videoUrl;

    public string Id { get; set; }

    [ObservableProperty]
    public partial WebDavStorageItemViewModel? Current { get; set; }

    [ObservableProperty]
    public partial string? Title { get; set; }

    [ObservableProperty]
    public partial bool HasNextVideo { get; set; }

    [ObservableProperty]
    public partial string? NextVideoTip { get; set; }

    [ObservableProperty]
    public partial bool HasPrevVideo { get; set; }

    [ObservableProperty]
    public partial string? PrevVideoTip { get; set; }

    [ObservableProperty]
    public partial bool CanVideoNavigate { get; set; }

    [ObservableProperty]
    public partial string? ErrorMessage { get; set; }

    [ObservableProperty]
    public partial List<WebDavStorageItemViewModel>? Playlist { get; set; }

    [ObservableProperty]
    public partial bool IsPlaylistSectionPanelVisible { get; set; }

    /// <summary>
    /// 视频选择更改事件.
    /// </summary>
    public event EventHandler VideoSelectionChanged;
}
