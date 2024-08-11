// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// UGC合集详情视图模型.
/// </summary>
public sealed partial class UgcSeasonFavoriteSectionDetailViewModel
{
    private readonly IFavoriteService _service;
    private readonly ILogger<UgcSeasonFavoriteSectionDetailViewModel> _logger;

    private int _pageNumber;
    private bool _preventLoadMore;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private Uri? _cover;

    /// <summary>
    /// 列表已完成更新.
    /// </summary>
    public event EventHandler ListUpdated;

    /// <summary>
    /// 条目列表.
    /// </summary>
    public ObservableCollection<VideoItemViewModel> Items { get; } = new();
}
