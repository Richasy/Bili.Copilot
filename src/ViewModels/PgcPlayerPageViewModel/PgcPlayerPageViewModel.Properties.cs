// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Data.Pgc;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// PGC 播放页面视图模型.
/// </summary>
public sealed partial class PgcPlayerPageViewModel
{
    private string _presetEpisodeId;
    private string _presetSeasonId;
    private string _presetTitle;
    private bool _needBiliPlus;
    private Action _playNextEpisodeAction;
    private Window _attachedWindow;

    [ObservableProperty]
    private bool _isReloading;

    [ObservableProperty]
    private bool _isFavoriteFolderRequesting;

    [ObservableProperty]
    private PgcPlayerView _view;

    [ObservableProperty]
    private bool _isSignedIn;

    [ObservableProperty]
    private string _playCountText;

    [ObservableProperty]
    private string _danmakuCountText;

    [ObservableProperty]
    private string _commentCountText;

    [ObservableProperty]
    private string _likeCountText;

    [ObservableProperty]
    private string _coinCountText;

    [ObservableProperty]
    private string _favoriteCountText;

    [ObservableProperty]
    private string _ratingCountText;

    [ObservableProperty]
    private bool _isLiked;

    [ObservableProperty]
    private bool _isCoined;

    [ObservableProperty]
    private bool _isFavorited;

    [ObservableProperty]
    private bool _isTracking;

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
    private bool _isShowCelebrities;

    [ObservableProperty]
    private PlayerSectionHeader _currentSection;

    [ObservableProperty]
    private EpisodeInformation _currentEpisode;

    [ObservableProperty]
    private bool _isShowSeasons;

    [ObservableProperty]
    private bool _isShowEpisodes;

    [ObservableProperty]
    private bool _isShowComments;

    [ObservableProperty]
    private bool _isShowExtras;

    [ObservableProperty]
    private bool _isSectionsEmpty;

    [ObservableProperty]
    private PlayerDetailViewModel _playerDetail;

    /// <summary>
    /// 视频收藏夹.
    /// </summary>
    public ObservableCollection<VideoFavoriteFolderSelectableViewModel> FavoriteFolders { get; }

    /// <summary>
    /// 标头.
    /// </summary>
    public ObservableCollection<PlayerSectionHeader> Sections { get; }

    /// <summary>
    /// 分集列表.
    /// </summary>
    public ObservableCollection<EpisodeItemViewModel> Episodes { get; }

    /// <summary>
    /// 剧集列表.
    /// </summary>
    public ObservableCollection<VideoIdentifierSelectableViewModel> Seasons { get; }

    /// <summary>
    /// 附加列表.
    /// </summary>
    public ObservableCollection<PgcExtraItemViewModel> Extras { get; }

    /// <summary>
    /// 参与人员.
    /// </summary>
    public ObservableCollection<UserItemViewModel> Celebrities { get; }
}
