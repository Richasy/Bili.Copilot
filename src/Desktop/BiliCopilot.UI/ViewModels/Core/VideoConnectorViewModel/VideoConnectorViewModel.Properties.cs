// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Richasy.BiliKernel.Models.Base;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.ViewModels.Core;

public sealed partial class VideoConnectorViewModel
{
    public event EventHandler<PlaylistInitializedEventArgs> PlaylistInitialized;
    public event EventHandler<MediaSnapshot> NewMediaRequest;
    public event EventHandler RequestOpenExtraPanel;
    public event EventHandler<PlayerInformationUpdatedEventArgs> PropertiesUpdated;

    private readonly CommentMainViewModel _comments;
    private MediaSnapshot _snapshot;
    internal VideoPlayerView _view;
    internal VideoPart? _part;

    /// <summary>
    /// 区块加载完成.
    /// </summary>
    public event EventHandler SectionInitialized;

    [ObservableProperty]
    public partial Uri? Cover { get; set; }

    [ObservableProperty]
    public partial string? Title { get; set; }

    [ObservableProperty]
    public partial string? Subtitle { get; set; }

    [ObservableProperty]
    public partial string? Description { get; set; }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial bool IsMyVideo { get; set; }

    [ObservableProperty]
    public partial string? ContainerId { get; set; }

    [ObservableProperty]
    public partial string? UpName { get; set; }

    [ObservableProperty]
    public partial Uri? UpAvatar { get; set; }

    [ObservableProperty]
    public partial string? PublishRelativeTime { get; set; }

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
    public partial string? OnlineCountText { get; set; }

    [ObservableProperty]
    public partial string? AvId { get; set; }

    [ObservableProperty]
    public partial string? BvId { get; set; }

    [ObservableProperty]
    public partial bool IsLiked { get; set; }

    [ObservableProperty]
    public partial bool IsCoined { get; set; }

    [ObservableProperty]
    public partial bool IsFavorited { get; set; }

    [ObservableProperty]
    public partial bool IsCoinAlsoLike { get; set; }

    [ObservableProperty]
    public partial bool IsPrivatePlay { get; set; }

    [ObservableProperty]
    public partial bool IsInteractionVideo { get; set; }

    [ObservableProperty]
    public partial List<BiliTag>? Tags { get; set; }

    [ObservableProperty]
    public partial List<PlayerFavoriteFolderViewModel>? FavoriteFolders { get; set; }

    [ObservableProperty]
    public partial IPlayerSectionDetailViewModel? SelectedSection { get; set; }

    [ObservableProperty]
    public partial List<IPlayerSectionDetailViewModel>? Sections { get; set; }

    [ObservableProperty]
    public partial bool IsAISectionPanelVisible { get; set; }

    [ObservableProperty]
    public partial bool IsAISectionPanelLoaded { get; set; }

    /// <summary>
    /// 下载视图模型.
    /// </summary>
    public DownloadViewModel Downloader { get; }

    /// <summary>
    /// AI 视图模型.
    /// </summary>
    public AIViewModel AI { get; }

    /// <summary>
    /// 播放视图模型.
    /// </summary>
    public PlayerViewModel Parent { get; set; }
}
