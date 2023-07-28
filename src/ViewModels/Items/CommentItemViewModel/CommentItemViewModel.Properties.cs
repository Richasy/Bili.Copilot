// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Models.Data.Community;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 评论条目视图模型.
/// </summary>
public sealed partial class CommentItemViewModel
{
    private readonly Action<CommentItemViewModel> _showCommentDetailAction;
    private readonly Action<CommentItemViewModel> _clickAction;

    [ObservableProperty]
    private CommentInformation _data;

    [ObservableProperty]
    private bool _isLiked;

    [ObservableProperty]
    private string _likeCountText;

    [ObservableProperty]
    private string _replyCountText;

    [ObservableProperty]
    private string _publishDateText;

    [ObservableProperty]
    private bool _isUserHighlight;
}
