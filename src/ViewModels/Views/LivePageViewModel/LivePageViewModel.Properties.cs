// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.App;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 直播页面视图模型.
/// </summary>
public sealed partial class LivePageViewModel
{
    private bool _isInitialized;

    [ObservableProperty]
    private LiveDisplayType _currentType;

    [ObservableProperty]
    private bool _isRecommendShown;

    [ObservableProperty]
    private bool _isPartitionShown;

    [ObservableProperty]
    private bool _isPartitionDetailShown;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private bool _isReloading;

    /// <summary>
    /// 实例.
    /// </summary>
    public static LivePageViewModel Instance { get; } = new();
}
