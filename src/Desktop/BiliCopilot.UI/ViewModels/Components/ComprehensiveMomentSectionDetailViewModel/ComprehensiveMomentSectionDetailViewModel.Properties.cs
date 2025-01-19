// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Moment;
using System.Collections.ObjectModel;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 综合动态分区详情视图模型.
/// </summary>
public sealed partial class ComprehensiveMomentSectionDetailViewModel
{
    private readonly IMomentDiscoveryService _service;
    private readonly ILogger<VideoMomentSectionDetailViewModel> _logger;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private MomentUperSectionViewModel? _selectedUper;

    /// <summary>
    /// 已经初始化完成.
    /// </summary>
    public event EventHandler Initialized;

    /// <inheritdoc/>
    public MomentSectionType SectionType => MomentSectionType.Comprehensive;

    /// <summary>
    /// 推荐的用户列表.
    /// </summary>
    public ObservableCollection<MomentUperSectionViewModel> Upers { get; } = new();
}
