// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 历史记录页面视图模型.
/// </summary>
public sealed partial class HistoryPageViewModel
{
    private readonly IViewHistoryService _service;
    private readonly ILogger<HistoryPageViewModel> _logger;

    [ObservableProperty]
    private IHistorySectionDetailViewModel _selectedSection;

    [ObservableProperty]
    private IReadOnlyCollection<IHistorySectionDetailViewModel> _sections;

    /// <summary>
    /// 初始化完成.
    /// </summary>
    public event EventHandler SectionInitialized;
}
