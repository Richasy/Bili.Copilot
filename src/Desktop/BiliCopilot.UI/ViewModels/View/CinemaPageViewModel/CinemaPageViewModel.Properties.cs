// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 影院页面视图模型.
/// </summary>
public sealed partial class CinemaPageViewModel
{
    private readonly IEntertainmentDiscoveryService _service;
    private readonly ILogger<CinemaPageViewModel> _logger;

    [ObservableProperty]
    private double _navColumnWidth;

    [ObservableProperty]
    private bool _isNavColumnManualHide;

    [ObservableProperty]
    private IReadOnlyCollection<EntertainmentIndexViewModel>? _sections;

    [ObservableProperty]
    private EntertainmentIndexViewModel? _selectedSection;

    /// <summary>
    /// 区块加载完成.
    /// </summary>
    public event EventHandler SectionInitialized;
}
