// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Constants.Bili;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 主评论模块视图模型.
/// </summary>
public sealed partial class CommentMainModuleViewModel
{
    private bool _isEnd;
    private CommentType _commentType;
    private string _targetId;
    private CommentItemViewModel _selectedComment;

    [ObservableProperty]
    private CommentItemViewModel _topComment;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private CommentSortHeader _currentSort;

    [ObservableProperty]
    private string _replyTip;

    [ObservableProperty]
    private string _replyText;

    [ObservableProperty]
    private bool _isSending;

    /// <summary>
    /// 请求显示评论详情.
    /// </summary>
    public event EventHandler<CommentItemViewModel> RequestShowDetail;

    /// <summary>
    /// 排序方式集合.
    /// </summary>
    public ObservableCollection<CommentSortHeader> SortCollection { get; }
}
