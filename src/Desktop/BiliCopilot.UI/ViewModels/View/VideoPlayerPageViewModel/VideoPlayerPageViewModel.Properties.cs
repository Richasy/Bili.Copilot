// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.Base;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 视频播放器页面视图模型.
/// </summary>
public sealed partial class VideoPlayerPageViewModel
{
    private readonly IPlayerService _service;
    private readonly IRelationshipService _relationshipService;
    private readonly IFavoriteService _favoriteService;
    private readonly ILogger<VideoPlayerPageViewModel> _logger;
    private readonly CommentMainViewModel _comments;

    private CancellationTokenSource _pageLoadCancellationTokenSource;
    private CancellationTokenSource _dashLoadCancellationTokenSource;

    private VideoPlayerView? _view;
    private VideoPart? _part;
    private IList<DashSegmentInformation>? _videoSegments;
    private IList<DashSegmentInformation>? _audioSegments;
    private IList<VideoInformation>? _playlist;
    private int _initialProgress;

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
    private string _title;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private string _upName;

    [ObservableProperty]
    private Uri? _upAvatar;

    [ObservableProperty]
    private string _publishRelativeTime;

    [ObservableProperty]
    private bool _isFollow;

    [ObservableProperty]
    private bool _isMyVideo;

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
    private string _onlineCountText;

    [ObservableProperty]
    private string _avId;

    [ObservableProperty]
    private string _bvId;

    [ObservableProperty]
    private bool _isLiked;

    [ObservableProperty]
    private bool _isCoined;

    [ObservableProperty]
    private bool _isFavorited;

    [ObservableProperty]
    private bool _isCoinAlsoLike;

    [ObservableProperty]
    private bool _hasNextVideo;

    [ObservableProperty]
    private string _nextVideoTip;

    [ObservableProperty]
    private bool _isPrivatePlay;

    [ObservableProperty]
    private bool _isInteractionVideo;

    [ObservableProperty]
    private PlayerFormatItemViewModel? _selectedFormat;

    [ObservableProperty]
    private IPlayerSectionDetailViewModel? _selectedSection;

    [ObservableProperty]
    private IReadOnlyCollection<PlayerFormatItemViewModel>? _formats;

    [ObservableProperty]
    private IReadOnlyCollection<BiliTag>? _tags;

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
    /// 弹幕视图模型.
    /// </summary>
    public DanmakuViewModel Danmaku { get; }

    /// <summary>
    /// 字幕视图模型.
    /// </summary>
    public SubtitleViewModel Subtitle { get; }
}
