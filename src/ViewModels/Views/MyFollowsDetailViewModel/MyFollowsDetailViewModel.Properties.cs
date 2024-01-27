// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Bili.Copilot.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 我的关注页面视图模型.
/// </summary>
public sealed partial class MyFollowsDetailViewModel
{
    private static readonly Lazy<MyFollowsDetailViewModel> _lazyInstance = new(() => new MyFollowsDetailViewModel());
    private readonly Dictionary<string, IEnumerable<UserItemViewModel>> _cache;

    [ObservableProperty]
    private FollowGroupViewModel _currentGroup;

    [ObservableProperty]
    private bool _isCurrentGroupEmpty;

    [ObservableProperty]
    private string _userName;

    [ObservableProperty]
    private bool _isSwitching;

    /// <summary>
    /// 实例.
    /// </summary>
    public static MyFollowsDetailViewModel Instance => _lazyInstance.Value;

    /// <summary>
    /// 关注分组.
    /// </summary>
    public ObservableCollection<FollowGroupViewModel> Groups { get; }
}
