// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// PGC 播放器页面视图模型.
/// </summary>
public sealed partial class PgcPlayerPageViewModel
{
    private readonly IPlayerService _service;
    private readonly IEntertainmentDiscoveryService _discoveryService;
    private readonly IFavoriteService _favoriteService;
    private readonly ILogger<PgcPlayerPageViewModel> _logger;
    private readonly CommentMainViewModel _comments;

    private CancellationTokenSource _pageLoadCancellationTokenSource;
    private CancellationTokenSource _dashLoadCancellationTokenSource;

    private PgcPlayerView? _view;
    private EpisodeInformation? _episode;
    private IList<DashSegmentInformation>? _videoSegments;
    private IList<DashSegmentInformation>? _audioSegments;
    private int _initialProgress;
    private EntertainmentType _type;

    [ObservableProperty]
    private bool _isPageLoading;

    [ObservableProperty]
    private bool _isPageLoadFailed;

    [ObservableProperty]
    private bool _isMediaLoading;

    [ObservableProperty]
    private bool _isMediaLoadFailed;

    [ObservableProperty]
    private Uri? _cover;

    [ObservableProperty]
    private string _seasonTitle;

    [ObservableProperty]
    private string _episodeTitle;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private string _alias;

    [ObservableProperty]
    private bool _isFollow;

    [ObservableProperty]
    private double _playerWidth;

    [ObservableProperty]
    private double _playerHeight;

    [ObservableProperty]
    private double _playCount;

    [ObservableProperty]
    private double _danmakuCount;

    [ObservableProperty]
    private double _commentCount;

    [ObservableProperty]
    private double _likeCount;

    [ObservableProperty]
    private double _coinCount;

    [ObservableProperty]
    private double _favoriteCount;

    [ObservableProperty]
    private double _followCount;

    [ObservableProperty]
    private string _onlineCountText;

    [ObservableProperty]
    private string _seasonId;

    [ObservableProperty]
    private string _episodeId;

    [ObservableProperty]
    private bool _isLiked;

    [ObservableProperty]
    private bool _isCoined;

    [ObservableProperty]
    private bool _isFavorited;

    [ObservableProperty]
    private bool _isCoinAlsoLike;

    [ObservableProperty]
    private bool _hasNextEpisode;

    [ObservableProperty]
    private string _nextEpisodeTip;

    [ObservableProperty]
    private PlayerFormatItemViewModel? _selectedFormat;

    [ObservableProperty]
    private IPlayerSectionDetailViewModel? _selectedSection;

    [ObservableProperty]
    private IReadOnlyCollection<PlayerFormatItemViewModel>? _formats;

    [ObservableProperty]
    private IReadOnlyCollection<PlayerFavoriteFolderViewModel>? _favoriteFolders;

    [ObservableProperty]
    private IReadOnlyCollection<IPlayerSectionDetailViewModel>? _sections;

    /// <summary>
    /// 视图数据加载完成.
    /// </summary>
    public event EventHandler ViewInitialized;

    /// <summary>
    /// 区块加载完成.
    /// </summary>
    public event EventHandler SectionInitialized;

    /// <summary>
    /// 播放器视图模型.
    /// </summary>
    public PlayerViewModel Player { get; }

    /// <summary>
    /// 弹幕视图模型.
    /// </summary>
    public DanmakuViewModel Danmaku { get; }

    /// <summary>
    /// 字幕视图模型.
    /// </summary>
    public SubtitleViewModel Subtitle { get; }
}
