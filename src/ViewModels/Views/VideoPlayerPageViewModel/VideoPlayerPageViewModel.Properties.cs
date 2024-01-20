// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Data.Community;
using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 视频播放页面视图模型.
/// </summary>
public sealed partial class VideoPlayerPageViewModel
{
    private readonly DispatcherQueue _dispatcherQueue;

    private string _presetVideoId;
    private Action _playNextVideoAction;
    private Action _playPreviousVideoAction;
    private bool _isInPrivate;
    private Window _attachedWindow;
    private bool _isStatsUpdated;

    [ObservableProperty]
    private VideoPlayerView _view;

    [ObservableProperty]
    private bool _isSignedIn;

    [ObservableProperty]
    private UserItemViewModel _author;

    [ObservableProperty]
    private bool _isCooperationVideo;

    [ObservableProperty]
    private string _publishTime;

    [ObservableProperty]
    private string _playCountText;

    [ObservableProperty]
    private string _danmakuCountText;

    [ObservableProperty]
    private string _commentCountText;

    [ObservableProperty]
    private string _watchingCountText;

    [ObservableProperty]
    private bool _isShowTags;

    [ObservableProperty]
    private string _likeCountText;

    [ObservableProperty]
    private string _coinCountText;

    [ObservableProperty]
    private string _favoriteCountText;

    [ObservableProperty]
    private bool _isLiked;

    [ObservableProperty]
    private bool _isCoined;

    [ObservableProperty]
    private bool _isFavorited;

    [ObservableProperty]
    private bool _isCoinWithLiked;

    [ObservableProperty]
    private bool _isError;

    [ObservableProperty]
    private string _errorText;

    [ObservableProperty]
    private bool _isFavoriteFoldersError;

    [ObservableProperty]
    private string _favoriteFoldersErrorText;

    [ObservableProperty]
    private bool _isVideoFixed;

    [ObservableProperty]
    private bool _isOnlyShowIndex;

    [ObservableProperty]
    private PlayerSectionHeader _currentSection;

    [ObservableProperty]
    private VideoSeason _currentSeason;

    [ObservableProperty]
    private VideoIdentifier _currentVideoPart;

    [ObservableProperty]
    private bool _isShowUgcSeason;

    [ObservableProperty]
    private bool _isShowRelatedVideos;

    [ObservableProperty]
    private bool _isShowVideoPlaylist;

    [ObservableProperty]
    private bool _isShowComments;

    [ObservableProperty]
    private bool _isShowParts;

    [ObservableProperty]
    private bool _isShowInformation;

    [ObservableProperty]
    private bool _isReloading;

    [ObservableProperty]
    private bool _isFavoriteFolderRequesting;

    [ObservableProperty]
    private PlayerDetailViewModel _playerDetail;

    [ObservableProperty]
    private CommentModuleViewModel _comments;

    [ObservableProperty]
    private VideoMediaStats _stats;

    /// <summary>
    /// 视频协作者.
    /// </summary>
    public ObservableCollection<UserItemViewModel> Collaborators { get; }

    /// <summary>
    /// 视频标签集.
    /// </summary>
    public ObservableCollection<Tag> Tags { get; }

    /// <summary>
    /// 收藏夹列表.
    /// </summary>
    public ObservableCollection<VideoFavoriteFolderSelectableViewModel> FavoriteFolders { get; }

    /// <summary>
    /// 播放时的关联区块集合.
    /// </summary>
    public ObservableCollection<PlayerSectionHeader> Sections { get; }

    /// <summary>
    /// 关联的视频集合.
    /// </summary>
    public ObservableCollection<VideoItemViewModel> RelatedVideos { get; }

    /// <summary>
    /// 视频播放列表.
    /// </summary>
    public ObservableCollection<VideoItemViewModel> VideoPlaylist { get; }

    /// <summary>
    /// 视频分集集合.
    /// </summary>
    public ObservableCollection<VideoIdentifierSelectableViewModel> VideoParts { get; }

    /// <summary>
    /// 合集集合.
    /// </summary>
    public ObservableCollection<VideoSeason> Seasons { get; }

    /// <summary>
    /// 当前合集下的视频列表.
    /// </summary>
    public ObservableCollection<VideoItemViewModel> CurrentSeasonVideos { get; set; }
}
