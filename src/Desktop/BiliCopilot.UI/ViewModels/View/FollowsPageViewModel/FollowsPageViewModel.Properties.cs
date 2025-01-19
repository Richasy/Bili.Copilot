// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.User;
using System.Collections.ObjectModel;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 关注页面视图模型.
/// </summary>
public sealed partial class FollowsPageViewModel
{
    private readonly IRelationshipService _service;
    private readonly ILogger<FollowsPageViewModel> _logger;
    private readonly Dictionary<UserGroup, List<UserItemViewModel>> _userCache = new();
    private readonly Dictionary<UserGroup, int> _offsetCache = new();

    [ObservableProperty]
    private bool _isGroupLoading;

    [ObservableProperty]
    private bool _isUserLoading;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private UserGroup _selectedGroup;

    /// <summary>
    /// 分组加载完成.
    /// </summary>
    public event EventHandler GroupInitialized;

    /// <summary>
    /// 用户列表已完成更新.
    /// </summary>
    public event EventHandler UserListUpdated;

    /// <summary>
    /// 分组.
    /// </summary>
    public ObservableCollection<UserGroup> Groups { get; } = new();

    /// <summary>
    /// 显示的用户列表.
    /// </summary>
    public ObservableCollection<UserItemViewModel> Users { get; } = new();
}
