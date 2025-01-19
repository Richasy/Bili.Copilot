// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;
using System.Collections.ObjectModel;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 收藏页面视图模型.
/// </summary>
public sealed partial class FavoritesPageViewModel
{
    private readonly IFavoriteService _service;
    private readonly ILogger<FavoritesPageViewModel> _logger;

    [ObservableProperty]
    private IFavoriteSectionDetailViewModel _currentSection;

    [ObservableProperty]
    private bool _isLoading;

    /// <summary>
    /// 分区初始化完成.
    /// </summary>
    public event EventHandler SectionInitialized;

    /// <summary>
    /// 分区列表.
    /// </summary>
    public ObservableCollection<IFavoriteSectionDetailViewModel> Sections { get; } = new();
}
