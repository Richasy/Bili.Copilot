﻿// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Community;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 消息页面视图模型.
/// </summary>
public sealed partial class MessageDetailViewModel
{
    private static readonly Lazy<MessageDetailViewModel> _lazyInstance = new(() => new MessageDetailViewModel());
    private readonly Dictionary<MessageType, (IEnumerable<MessageInformation> Items, bool IsEnd)> _caches;

    private bool _isEnd;
    private bool _shouldClearCache;

    /// <summary>
    /// 当前选中的消息类型.
    /// </summary>
    [ObservableProperty]
    private MessageHeaderViewModel _currentType;

    /// <summary>
    /// 是否为空.
    /// </summary>
    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isInChatSession;

    [ObservableProperty]
    private double _navListColumnWidth;

    /// <summary>
    /// 实例.
    /// </summary>
    public static MessageDetailViewModel Instance => _lazyInstance.Value;

    /// <summary>
    /// 消息类型集合.
    /// </summary>
    public ObservableCollection<MessageHeaderViewModel> MessageTypes { get; }
}
