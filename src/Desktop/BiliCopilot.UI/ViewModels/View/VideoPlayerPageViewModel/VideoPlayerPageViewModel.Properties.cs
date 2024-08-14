// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading;
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
    private readonly ILogger<VideoPlayerPageViewModel> _logger;

    private CancellationTokenSource _pageLoadCancellationTokenSource;
    private CancellationTokenSource _dashLoadCancellationTokenSource;

    private VideoPlayerView? _view;
    private IList<DashSegmentInformation>? _videoSegments;
    private IList<DashSegmentInformation>? _audioSegments;

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
    private PlayerFormatItemViewModel? _selectedFormat;

    [ObservableProperty]
    private IReadOnlyCollection<PlayerFormatItemViewModel>? _formats;

    [ObservableProperty]
    private IReadOnlyCollection<BiliTag>? _tags;

    /// <summary>
    /// 播放器视图模型.
    /// </summary>
    public PlayerViewModel Player { get; }
}
