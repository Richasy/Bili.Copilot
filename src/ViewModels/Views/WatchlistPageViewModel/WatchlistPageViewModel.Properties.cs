// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.App;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 观看列表页面的视图模型.
/// </summary>
public sealed partial class WatchlistPageViewModel
{
    private bool _isInitialized;

    [ObservableProperty]
    private WatchlistType _currentType;

    [ObservableProperty]
    private bool _isHistoryShown;

    [ObservableProperty]
    private bool _isViewLaterShown;

    [ObservableProperty]
    private bool _isFavoriteShown;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private bool _isReloading;

    [ObservableProperty]
    private bool _isClearing;

    /// <summary>
    /// 实例.
    /// </summary>
    public static WatchlistPageViewModel Instance { get; } = new();
}
