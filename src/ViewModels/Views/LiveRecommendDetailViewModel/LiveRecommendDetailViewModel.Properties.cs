// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 直播推荐详情的视图模型.
/// </summary>
public sealed partial class LiveRecommendDetailViewModel
{
    [ObservableProperty]
    private bool _isFollowsEmpty;

    /// <summary>
    /// 实例.
    /// </summary>
    public static LiveRecommendDetailViewModel Instance { get; } = new();

    /// <summary>
    /// 已关注的直播间.
    /// </summary>
    public ObservableCollection<LiveItemViewModel> Follows { get; }
}
