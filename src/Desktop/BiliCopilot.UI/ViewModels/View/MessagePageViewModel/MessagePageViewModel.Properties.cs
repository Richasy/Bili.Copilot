// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 消息页面视图模型.
/// </summary>
public sealed partial class MessagePageViewModel
{
    private readonly IMessageService _service;
    private readonly ILogger<MessagePageViewModel> _logger;

    [ObservableProperty]
    private IMessageSectionDetailViewModel _currentSection;

    [ObservableProperty]
    private bool _isLoading;

    /// <summary>
    /// 分区初始化完成.
    /// </summary>
    public event EventHandler SectionInitialized;

    /// <summary>
    /// 分区列表.
    /// </summary>
    public ObservableCollection<IMessageSectionDetailViewModel> Sections { get; } = new();
}
