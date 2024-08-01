// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 动漫页面视图模型.
/// </summary>
public sealed partial class AnimePageViewModel
{
    private readonly IEntertainmentDiscoveryService _service;
    private readonly ILogger<AnimePageViewModel> _logger;

    [ObservableProperty]
    private double _navColumnWidth;

    [ObservableProperty]
    private bool _isNavColumnManualHide;

    [ObservableProperty]
    private IReadOnlyCollection<IAnimeSectionDetailViewModel>? _sections;

    [ObservableProperty]
    private IAnimeSectionDetailViewModel? _selectedSection;

    /// <summary>
    /// 区块加载完成.
    /// </summary>
    public event EventHandler SectionInitialized;
}
