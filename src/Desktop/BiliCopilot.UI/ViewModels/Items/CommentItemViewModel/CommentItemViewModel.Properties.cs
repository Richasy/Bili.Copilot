// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Comment;
using Richasy.BiliKernel.Models.Appearance;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 评论项视图模型.
/// </summary>
public sealed partial class CommentItemViewModel
{
    private readonly ICommentService _service;
    private readonly ILogger<CommentItemViewModel> _logger;
    private readonly Action<CommentItemViewModel> _markReplyTargetAction;

    [ObservableProperty]
    private bool _isLiked;

    [ObservableProperty]
    private int _likeCount;

    /// <summary>
    /// 子评论条数.
    /// </summary>
    public int ChildCount { get; init; }

    /// <summary>
    /// 发布的相对时间.
    /// </summary>
    public string RelativeTime { get; init; }

    /// <summary>
    /// 发布的实际时间.
    /// </summary>
    public string ActualTime { get; init; }

    /// <summary>
    /// 用户头像.
    /// </summary>
    public Uri? Avatar { get; init; }

    /// <summary>
    /// 用户名.
    /// </summary>
    public string UserName { get; init; }

    /// <summary>
    /// 是否为大会员.
    /// </summary>
    public bool IsVip { get; init; }

    /// <summary>
    /// 内容.
    /// </summary>
    public EmoteText Content { get; init; }
}
