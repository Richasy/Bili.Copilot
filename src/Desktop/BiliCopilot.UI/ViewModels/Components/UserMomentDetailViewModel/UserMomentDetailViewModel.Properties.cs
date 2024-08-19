﻿// Copyright (c) Bili Copilot. All rights reserved.

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

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isCommentsOpened;

    [ObservableProperty]
    private string _title;

    /// <summary>
    /// 列表已完成更新.
    /// </summary>
    public event EventHandler ListUpdated;

    /// <summary>
    /// 条目列表.
    /// </summary>
    public ObservableCollection<MomentItemViewModel> Items { get; } = new();

    /// <summary>
    /// 评论模块.
    /// </summary>
    public CommentMainViewModel CommentModule { get; }
}
