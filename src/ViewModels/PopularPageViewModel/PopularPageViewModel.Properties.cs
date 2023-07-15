// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 流行视频页面的视图模型.
/// </summary>
public sealed partial class PopularPageViewModel
{
    private readonly Dictionary<PopularType, IEnumerable<VideoInformation>> _caches;

    [ObservableProperty]
    private PopularType _currentType;

    [ObservableProperty]
    private bool _isRecommendShown;

    [ObservableProperty]
    private bool _isHotShown;

    [ObservableProperty]
    private bool _isRankShown;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private string _title;

    /// <summary>
    /// 实例.
    /// </summary>
    public static PopularPageViewModel Instance { get; } = new();
}
