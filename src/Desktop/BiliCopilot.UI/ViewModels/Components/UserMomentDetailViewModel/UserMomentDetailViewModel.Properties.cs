// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Moment;
using Richasy.BiliKernel.Models.User;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 用户动态详情视图模型.
/// </summary>
public sealed partial class UserMomentDetailViewModel
{
    private readonly IMomentDiscoveryService _service;
    private readonly ILogger<UserMomentDetailViewModel> _logger;
    private bool _isVideo;
    private bool _preventLoadMore;
    private string? _offset;
    private UserProfile _user;
    private Action<MomentItemViewModel> _showCommentAction;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private ObservableCollection<MomentItemViewModel> _items = new();

    /// <summary>
    /// 列表已完成更新.
    /// </summary>
    public event EventHandler ListUpdated;
}
