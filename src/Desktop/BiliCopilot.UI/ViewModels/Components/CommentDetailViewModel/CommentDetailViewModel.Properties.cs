// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Comment;
using System.Collections.ObjectModel;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 评论详情视图模型.
/// </summary>
public sealed partial class CommentDetailViewModel
{
    private readonly ICommentService _service;
    private readonly ILogger<CommentDetailViewModel> _logger;
    private readonly Action<CommentDetailViewModel> _showMoreAction;

    private bool _preventLoadMore;
    private long _offset;
    private CommentItemViewModel? _replyItem;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _replyTarget;

    [ObservableProperty]
    private bool _isReplying;

    /// <summary>
    /// 列表已更新.
    /// </summary>
    public event EventHandler ListUpdated;

    /// <summary>
    /// 评论列表.
    /// </summary>
    public ObservableCollection<CommentItemViewModel> Comments { get; } = new();
}
