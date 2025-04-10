// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.Base;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.ViewModels.Core;

public sealed partial class VideoSourceViewModel
{
    private const string VideoReferer = "https://www.bilibili.com";
    private const string VideoUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36 Edg/116.0.1938.69";

    private readonly IPlayerService _service;
    private readonly IRelationshipService _relationshipService;
    private readonly IFavoriteService _favoriteService;
    private readonly ILogger<VideoSourceViewModel> _logger;

    private VideoSnapshot _cachedSnapshot;
    private VideoPlayerView? _view;
    private VideoPart? _part;
    private IList<DashSegmentInformation>? _videoSegments;
    private IList<DashSegmentInformation>? _audioSegments;
    private IList<VideoInformation>? _playlist;
    private double _initialProgress;
    private double _lastPosition;
    private bool _isSeasonInitialized;

    private string _videoUrl;
    private string _audioUrl;

    public string Id { get; set; }

    [ObservableProperty]
    public partial Uri? Cover{ get; set; }

    [ObservableProperty]
    public partial string? Title{ get; set; }

    [ObservableProperty]
    public partial string? Description{ get; set; }

    [ObservableProperty]
    public partial string? UpName{ get; set; }

    [ObservableProperty]
    public partial Uri? UpAvatar{ get; set; }

    [ObservableProperty]
    public partial string? PublishRelativeTime{ get; set; }

    [ObservableProperty]
    public partial bool IsFollow{ get; set; }

    [ObservableProperty]
    public partial bool IsMyVideo{ get; set; }

    [ObservableProperty]
    public partial double PlayCount{ get; set; }

    [ObservableProperty]
    public partial double DanmakuCount{ get; set; }

    [ObservableProperty]
    public partial double CommentCount{ get; set; }

    [ObservableProperty]
    public partial double LikeCount{ get; set; }

    [ObservableProperty]
    public partial double CoinCount{ get; set; }

    [ObservableProperty]
    public partial double FavoriteCount{ get; set; }

    [ObservableProperty]
    public partial string? OnlineCountText{ get; set; }

    [ObservableProperty]
    public partial string? AvId{ get; set; }

    [ObservableProperty]
    public partial string? BvId{ get; set; }

    [ObservableProperty]
    public partial bool IsLiked{ get; set; }

    [ObservableProperty]
    public partial bool IsCoined{ get; set; }

    [ObservableProperty]
    public partial bool IsFavorited{ get; set; }

    [ObservableProperty]
    public partial bool IsCoinAlsoLike{ get; set; }

    [ObservableProperty]
    public partial bool HasNextVideo{ get; set; }

    [ObservableProperty]
    public partial string? NextVideoTip{ get; set; }

    [ObservableProperty]
    public partial bool HasPrevVideo { get; set; }

    [ObservableProperty]
    public partial string? PrevVideoTip { get; set; }

    [ObservableProperty]
    public partial bool CanVideoNavigate { get; set; }

    [ObservableProperty]
    public partial bool IsPrivatePlay{ get; set; }

    [ObservableProperty]
    public partial bool IsInteractionVideo{ get; set; }

    [ObservableProperty]
    public partial VideoLoopType CurrentLoop{ get; set; }

    [ObservableProperty]
    public partial PlayerFormatItemViewModel? SelectedFormat{ get; set; }

    [ObservableProperty]
    public partial List<PlayerFormatItemViewModel>? Formats{ get; set; }

    [ObservableProperty]
    public partial List<BiliTag>? Tags{ get; set; }

    [ObservableProperty]
    public partial List<PlayerFavoriteFolderViewModel>? FavoriteFolders{ get; set; }

    [ObservableProperty]
    public partial List<VideoLoopType>? LoopTypes{ get; set; }

    [ObservableProperty]
    public partial CommentMainViewModel? CommentSection { get; set; }

    [ObservableProperty]
    public partial VideoPlayerPartSectionDetailViewModel? PartSection { get; set; }

    [ObservableProperty]
    public partial VideoPlayerPlaylistSectionDetailViewModel? PlaylistSection { get; set; }

    [ObservableProperty]
    public partial VideoPlayerRecommendSectionDetailViewModel? RecommendSection { get; set; }

    [ObservableProperty]
    public partial VideoPlayerSeasonSectionDetailViewModel? SeasonSection { get; set; }

    [ObservableProperty]
    public partial bool IsInfoSectionPanelVisible { get; set; }

    [ObservableProperty]
    public partial bool IsCommentSectionPanelVisible { get; set; }

    [ObservableProperty]
    public partial bool IsPartSectionPanelVisible { get; set; }

    [ObservableProperty]
    public partial bool IsPlaylistSectionPanelVisible { get; set; }

    [ObservableProperty]
    public partial bool IsRecommendSectionPanelVisible { get; set; }

    [ObservableProperty]
    public partial bool IsSeasonSectionPanelVisible { get; set; }

    [ObservableProperty]
    public partial bool IsAISectionPanelVisible { get; set; }

    [ObservableProperty]
    public partial bool IsCommentSectionPanelLoaded { get; set; }

    [ObservableProperty]
    public partial bool IsPartSectionPanelLoaded { get; set; }

    [ObservableProperty]
    public partial bool IsPlaylistSectionPanelLoaded { get; set; }

    [ObservableProperty]
    public partial bool IsRecommendSectionPanelLoaded { get; set; }

    [ObservableProperty]
    public partial bool IsSeasonSectionPanelLoaded { get; set; }

    [ObservableProperty]
    public partial bool IsAISectionPanelLoaded { get; set; }

    [ObservableProperty]
    public partial string? ErrorMessage { get; set; }

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

    /// <summary>
    /// AI 视图模型.
    /// </summary>
    public AIViewModel AI { get; }
}
