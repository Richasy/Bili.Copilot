// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Moment;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 我的动态页面视图模型.
/// </summary>
public sealed partial class MyMomentsPageViewModel
{
    private readonly IMomentDiscoveryService _service;
    private readonly ILogger<MyMomentsPageViewModel> _logger;
    private bool _preventLoadMore;
    private string? _offset;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isLoading;

    /// <summary>
    /// 列表已完成更新.
    /// </summary>
    public event EventHandler ListUpdated;

    /// <summary>
    /// 条目列表.
    /// </summary>
    public ObservableCollection<MomentItemViewModel> Items { get; } = new();
}
