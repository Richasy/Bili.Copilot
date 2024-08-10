// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Comment;
using Richasy.BiliKernel.Models;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 评论主视图模型.
/// </summary>
public sealed partial class CommentMainViewModel
{
    private readonly ICommentService _service;
    private readonly ILogger<CommentMainViewModel> _logger;

    private bool _preventLoadMore;
    private CommentTargetType _targetType;
    private long _offset;
    private CommentItemViewModel? _replyItem;

    [ObservableProperty]
    private CommentDetailViewModel? _topItem;

    [ObservableProperty]
    private CommentDetailViewModel? _selectedItem;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private IReadOnlyCollection<CommentSortType> _sorts;

    [ObservableProperty]
    private CommentSortType _sortType;

    [ObservableProperty]
    private string _replyTarget;

    [ObservableProperty]
    private bool _isReplying;

    /// <summary>
    /// 已初始化.
    /// </summary>
    public event EventHandler Initialized;

    /// <summary>
    /// 列表已更新.
    /// </summary>
    public event EventHandler ListUpdated;

    /// <summary>
    /// 评论列表.
    /// </summary>
    public ObservableCollection<CommentDetailViewModel> Comments { get; } = new();

    /// <summary>
    /// 标识符.
    /// </summary>
    public string Id { get; private set; }
}
