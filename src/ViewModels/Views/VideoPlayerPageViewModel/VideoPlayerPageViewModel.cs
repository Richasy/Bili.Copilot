// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Authorize;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Community;
using Bili.Copilot.Models.Data.Local;
using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 视频播放页面视图模型.
/// </summary>
public sealed partial class VideoPlayerPageViewModel : ViewModelBase, IDisposable
{
    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPlayerPageViewModel"/> class.
    /// </summary>
    public VideoPlayerPageViewModel()
    {
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        Collaborators = new ObservableCollection<UserItemViewModel>();
        Tags = new ObservableCollection<Tag>();
        FavoriteFolders = new ObservableCollection<VideoFavoriteFolderSelectableViewModel>();
        Sections = new ObservableCollection<Models.App.Other.PlayerSectionHeader>();
        RelatedVideos = new ObservableCollection<VideoItemViewModel>();
        VideoParts = new ObservableCollection<VideoIdentifierSelectableViewModel>();
        Seasons = new ObservableCollection<VideoSeason>();
        CurrentSeasonVideos = new ObservableCollection<VideoItemViewModel>();
        VideoPlaylist = new ObservableCollection<VideoItemViewModel>();
        Comments = new CommentModuleViewModel();

        IsSignedIn = AuthorizeProvider.Instance.State == AuthorizeState.SignedIn;
        AuthorizeProvider.Instance.StateChanged += OnAuthorizeStateChanged;

        AttachIsRunningToAsyncCommand(p => IsReloading = p, ReloadCommand);
        AttachIsRunningToAsyncCommand(p => IsFavoriteFolderRequesting = p, RequestFavoriteFoldersCommand);

        AttachExceptionHandlerToAsyncCommand(DisplayException, ReloadCommand);
        AttachExceptionHandlerToAsyncCommand(DisplayFavoriteFoldersException, RequestFavoriteFoldersCommand);
    }

    /// <summary>
    /// 设置播放快照.
    /// </summary>
    public void SetSnapshot(PlaySnapshot snapshot)
    {
        ReloadMediaPlayer();
        _presetVideoId = snapshot.VideoId;
        _isInPrivate = snapshot.IsInPrivate;
        var defaultPlayMode = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultPlayerDisplayMode, PlayerDisplayMode.Default);
        PlayerDetail.DisplayMode = snapshot.DisplayMode ?? defaultPlayMode;
        _ = ReloadCommand.ExecuteAsync(null);
    }

    /// <summary>
    /// 设置关联的窗口.
    /// </summary>
    /// <param name="window">窗口实例.</param>
    public void SetWindow(object window)
        => _attachedWindow = window as Window;

    /// <summary>
    /// 设置播放列表.
    /// </summary>
    /// <param name="videos">视频信息.</param>
    /// <param name="playIndex">播放索引.</param>
    public void SetPlaylist(IEnumerable<VideoInformation> videos, int playIndex = 0)
    {
        ReloadMediaPlayer();
        TryClear(VideoPlaylist);
        foreach (var item in videos)
        {
            VideoPlaylist.Add(GetItemViewModel(item));
        }

        var current = VideoPlaylist[playIndex].Data.Identifier;
        var snapshot = new PlaySnapshot(current.Id, default, VideoType.Video);
        SetSnapshot(snapshot);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    [RelayCommand]
    private void Reset()
    {
        View = null;
        _isStatsUpdated = false;
        ResetPublisher();
        ResetOverview();
        ResetOperation();
        ResetCommunityInformation();
        ResetInterop();
        ResetSections();
    }

    [RelayCommand]
    private async Task ReloadAsync()
    {
        Reset();
        View = await PlayerProvider.GetVideoDetailAsync(_presetVideoId);

        if (View == null)
        {
            DisplayException(new Exception("信息获取失败"));
            return;
        }

        InitializePublisher();
        InitializeOverview();
        InitializeOperation();
        InitializeCommunityInformation();
        InitializeSections();
        InitializeInterop();

        PlayerDetail.SetVideoData(View, _isInPrivate);
    }

    private void Clear()
    {
        Reset();
        if (PlayerDetail != null)
        {
            PlayerDetail.MediaEnded -= OnMediaEnded;
            PlayerDetail.InternalPartChanged -= OnInternalPartChanged;
            PlayerDetail.RequestOpenInBrowser -= OnRequestOpenInBrowserAsync;
            PlayerDetail.PropertyChanged -= OnPlayerDetailPropertyChanged;
            PlayerDetail?.Dispose();
        }
    }

    private void ReloadMediaPlayer()
    {
        if (PlayerDetail != null)
        {
            return;
        }

        PlayerDetail = new PlayerDetailViewModel(_attachedWindow);
        PlayerDetail.MediaEnded += OnMediaEnded;
        PlayerDetail.InternalPartChanged += OnInternalPartChanged;
        PlayerDetail.RequestOpenInBrowser += OnRequestOpenInBrowserAsync;
        PlayerDetail.PropertyChanged += OnPlayerDetailPropertyChanged;
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Clear();
            }

            _disposedValue = true;
        }
    }

    partial void OnCurrentSectionChanged(PlayerSectionHeader value)
    {
        if (value != null)
        {
            CheckSectionVisibility();
        }
    }

    partial void OnIsOnlyShowIndexChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.IsOnlyShowIndex, value);
}
