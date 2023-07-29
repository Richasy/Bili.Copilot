// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Bili.Copilot.Models.Data.Community;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 我的关注页面视图模型.
/// </summary>
public sealed partial class MyFollowsDetailViewModel
{
    private readonly Dictionary<string, IEnumerable<UserItemViewModel>> _cache;

    [ObservableProperty]
    private FollowGroup _currentGroup;

    [ObservableProperty]
    private bool _isCurrentGroupEmpty;

    [ObservableProperty]
    private string _userName;

    [ObservableProperty]
    private bool _isSwitching;

    /// <summary>
    /// 实例.
    /// </summary>
    public static MyFollowsDetailViewModel Instance { get; } = new MyFollowsDetailViewModel();

    /// <summary>
    /// 关注分组.
    /// </summary>
    public ObservableCollection<FollowGroup> Groups { get; }
}
