// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// PGC 源视图模型.
/// </summary>
public sealed partial class PgcSourceViewModel
{
    private const string VideoReferer = "https://www.bilibili.com";
    private const string VideoUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36 Edg/116.0.1938.69";

    private readonly IPlayerService _service;
    private readonly IEntertainmentDiscoveryService _discoveryService;
    private readonly IFavoriteService _favoriteService;
    private readonly ILogger<PgcSourceViewModel> _logger;

    private MediaIdentifier? _cachedMedia;
    private PgcPlayerView? _view;
    private EpisodeInformation? _episode;
    private IList<DashSegmentInformation>? _videoSegments;
    private IList<DashSegmentInformation>? _audioSegments;
    private double _initialProgress;
    private double _lastPosition;
    private int? _injectedProgress;
    private EntertainmentType _type;
    private bool _isSeasonInitialized;

    private string _videoUrl;
    private string _audioUrl;

    public string Id { get; set; }

    [ObservableProperty]
    public partial Uri? Cover { get; set; }

    [ObservableProperty]
    public partial string? SeasonTitle { get; set; }

    [ObservableProperty]
    public partial string? EpisodeTitle { get; set; }

    [ObservableProperty]
    public partial string? Description { get; set; }

    [ObservableProperty]
    public partial string? Alias { get; set; }

    [ObservableProperty]
    public partial bool IsFollow { get; set; }

    [ObservableProperty]
    public partial double PlayCount { get; set; }

    [ObservableProperty]
    public partial double DanmakuCount { get; set; }

    [ObservableProperty]
    public partial double CommentCount { get; set; }

    [ObservableProperty]
    public partial double LikeCount { get; set; }

    [ObservableProperty]
    public partial double CoinCount { get; set; }

    [ObservableProperty]
    public partial double FavoriteCount { get; set; }

    [ObservableProperty]
    public partial double FollowCount { get; set; }

    [ObservableProperty]
    public partial string? OnlineCountText { get; set; }

    [ObservableProperty]
    public partial string? SeasonId { get; set; }

    [ObservableProperty]
    public partial string? EpisodeId { get; set; }

    [ObservableProperty]
    public partial bool IsLiked { get; set; }

    [ObservableProperty]
    public partial bool IsCoined { get; set; }

    [ObservableProperty]
    public partial bool IsFavorited { get; set; }

    [ObservableProperty]
    public partial bool IsCoinAlsoLike { get; set; }

    [ObservableProperty]
    public partial bool HasNextEpisode { get; set; }

    [ObservableProperty]
    public partial string? NextEpisodeTip { get; set; }

    [ObservableProperty]
    public partial bool HasPrevEpisode { get; set; }

    [ObservableProperty]
    public partial string? PrevEpisodeTip { get; set; }

    [ObservableProperty]
    public partial bool CanEpisodeNavigate { get; set; }

    [ObservableProperty]
    public partial string? ErrorMessage { get; set; }

    [ObservableProperty]
    public partial PlayerFormatItemViewModel? SelectedFormat { get; set; }

    [ObservableProperty]
    public partial List<PlayerFormatItemViewModel>? Formats { get; set; }

    [ObservableProperty]
    public partial List<PlayerFavoriteFolderViewModel>? FavoriteFolders { get; set; }

    [ObservableProperty]
    public partial CommentMainViewModel? CommentSection { get; set; }

    [ObservableProperty]
    public partial PgcPlayerEpisodeSectionDetailViewModel? EpisodeSection { get; set; }

    [ObservableProperty]
    public partial PgcPlayerSeasonSectionDetailViewModel? SeasonSection { get; set; }

    [ObservableProperty]
    public partial bool IsInfoSectionPanelVisible { get; set; }

    [ObservableProperty]
    public partial bool IsCommentSectionPanelVisible { get; set; }

    [ObservableProperty]
    public partial bool IsEpisodeSectionPanelVisible { get; set; }

    [ObservableProperty]
    public partial bool IsSeasonSectionPanelVisible { get; set; }

    [ObservableProperty]
    public partial bool IsCommentSectionPanelLoaded { get; set; }

    [ObservableProperty]
    public partial bool IsEpisodeSectionPanelLoaded { get; set; }

    [ObservableProperty]
    public partial bool IsSeasonSectionPanelLoaded { get; set; }

    /// <summary>
    /// 弹幕视图模型.
    /// </summary>
    public DanmakuViewModel Danmaku { get; }

    /// <summary>
    /// 字幕视图模型.
    /// </summary>
    public SubtitleViewModel Subtitle { get; }

    /// <summary>
    /// 下载视图模型.
    /// </summary>
    public DownloadViewModel Downloader { get; }
}
