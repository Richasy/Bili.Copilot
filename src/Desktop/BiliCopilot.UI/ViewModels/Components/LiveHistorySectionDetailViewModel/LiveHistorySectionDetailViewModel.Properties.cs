// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models;
using System.Collections.ObjectModel;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 直播历史记录分区详情视图模型.
/// </summary>
public sealed partial class LiveHistorySectionDetailViewModel
{
    private readonly IViewHistoryService _service;
    private readonly ILogger<LiveHistorySectionDetailViewModel> _logger;

    private bool _isPreventLoadMore;
    private long _offset;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isLoading;

    /// <summary>
    /// 列表已完成更新.
    /// </summary>
    public event EventHandler ListUpdated;

    /// <inheritdoc/>
    public ViewHistoryTabType Type => ViewHistoryTabType.Live;

    /// <summary>
    /// 条目列表.
    /// </summary>
    public ObservableCollection<LiveItemViewModel> Items { get; } = new();
}
