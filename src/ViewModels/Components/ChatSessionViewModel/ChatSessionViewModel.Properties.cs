// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using Bili.Copilot.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels.Components;

/// <summary>
/// 聊天会话视图模型.
/// </summary>
public sealed partial class ChatSessionViewModel
{
    private static readonly Lazy<ChatSessionViewModel> _lazyInstance = new Lazy<ChatSessionViewModel>(() => new ChatSessionViewModel());

    [ObservableProperty]
    private UserItemViewModel _user;

    [ObservableProperty]
    private string _input;

    [ObservableProperty]
    private bool _isReloading;

    [ObservableProperty]
    private bool _isError;

    /// <summary>
    /// 请求滚动到底部.
    /// </summary>
    public event EventHandler RequestScrollToBottom;

    /// <summary>
    /// 实例.
    /// </summary>
    public static ChatSessionViewModel Instance => _lazyInstance.Value;

    /// <summary>
    /// 消息列表.
    /// </summary>
    public ObservableCollection<ChatMessageItemViewModel> Messages { get; }
}
