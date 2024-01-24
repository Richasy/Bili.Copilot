// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels.Components;

/// <summary>
/// 聊天会话列表模块视图模型.
/// </summary>
public sealed partial class ChatSessionListModuleViewModel
{
    private static readonly Lazy<ChatSessionListModuleViewModel> _lazyInstance = new Lazy<ChatSessionListModuleViewModel>(() => new ChatSessionListModuleViewModel());
    private bool _isEnd;

    [ObservableProperty]
    private ChatSessionItemViewModel _selectedSession;

    [ObservableProperty]
    private bool _isEmpty;

    /// <summary>
    /// 实例.
    /// </summary>
    public static ChatSessionListModuleViewModel Instance => _lazyInstance.Value;
}
