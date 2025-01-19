// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;
using System.Collections.ObjectModel;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 聊天消息区块详情视图模型.
/// </summary>
public sealed partial class ChatMessageSectionDetailViewModel
{
    private readonly IMessageService _service;
    private readonly ILogger<ChatMessageSectionDetailViewModel> _logger;

    private bool _preventLoadMore;

    [ObservableProperty]
    private string _lastMessage;

    [ObservableProperty]
    private string _lastMessageTime;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private int _unreadCount;

    [ObservableProperty]
    private bool _isSending;

    [ObservableProperty]
    private string _inputText;

    /// <summary>
    /// 消息列表.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<ChatMessageItemViewModel> _messages;

    /// <summary>
    /// 请求滚动到底部.
    /// </summary>
    public event EventHandler RequestScrollToBottom;

    /// <summary>
    /// 头像.
    /// </summary>
    public Uri? Avatar { get; init; }

    /// <summary>
    /// 用户名.
    /// </summary>
    public string UserName { get; init; }
}
