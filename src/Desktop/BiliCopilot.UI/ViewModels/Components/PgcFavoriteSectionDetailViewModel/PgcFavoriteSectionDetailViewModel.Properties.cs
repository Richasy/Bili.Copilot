// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models;
using System.Collections.ObjectModel;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 追番分区详情视图模型.
/// </summary>
public sealed partial class PgcFavoriteSectionDetailViewModel
{
    private readonly IFavoriteService _service;
    private readonly ILogger<PgcFavoriteSectionDetailViewModel> _logger;
    private readonly Dictionary<PgcFavoriteStatus, List<SeasonItemViewModel>> _cache = new();
    private readonly Dictionary<PgcFavoriteStatus, (int, bool)> _offsetCache = new();

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private PgcFavoriteStatus _currentStatus;

    [ObservableProperty]
    private List<PgcFavoriteStatus> _statusList;

    /// <summary>
    /// 列表已完成更新.
    /// </summary>
    public event EventHandler ListUpdated;

    /// <summary>
    /// 条目列表.
    /// </summary>
    public ObservableCollection<SeasonItemViewModel> Items { get; } = new();

    /// <summary>
    /// 类型.
    /// </summary>
    public PgcFavoriteType Type { get; }
}
