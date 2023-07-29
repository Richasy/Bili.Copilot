// Copyright (c) Bili Copilot. All rights reserved.

using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 二级评论模块视图模型.
/// </summary>
public sealed partial class CommentDetailModuleViewModel
{
    private bool _isEnd;
    private CommentItemViewModel _selectedComment;

    [ObservableProperty]
    private CommentItemViewModel _rootComment;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private string _replyTip;

    [ObservableProperty]
    private string _replyText;

    [ObservableProperty]
    private bool _isSending;

    /// <summary>
    /// 请求返回到主评论视图.
    /// </summary>
    public event EventHandler RequestBackToMain;
}
