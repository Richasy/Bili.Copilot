// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 粉丝页面视图模型.
/// </summary>
public sealed partial class FansPageViewModel
{
    private readonly IRelationshipService _service;
    private readonly ILogger<FansPageViewModel> _logger;

    private int _pageNumber;

    [ObservableProperty]
    private bool _isUserLoading;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private int _totalCount;

    /// <summary>
    /// 用户列表已完成更新.
    /// </summary>
    public event EventHandler UserListUpdated;

    /// <summary>
    /// 用户列表.
    /// </summary>
    public ObservableCollection<UserItemViewModel> Users { get; } = new();
}
