// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.ComponentModel;
using System.Linq;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Authorize;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Community;
using Bili.Copilot.Models.Data.Local;
using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using Windows.System;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 视频播放页面视图模型.
/// </summary>
public sealed partial class VideoPlayerPageViewModel
{
    [RelayCommand]
    private static void SearchTag(Tag tag)
        => AppViewModel.Instance.SearchContentCommand.Execute(tag.Name);

    private VideoItemViewModel GetItemViewModel(VideoInformation information)
    {
        var vm = new VideoItemViewModel(
            information,
            vm =>
            {
                var snapshot = new PlaySnapshot(vm.Data.Identifier.Id, default, VideoType.Video);
                SetSnapshot(snapshot);
            });
        return vm;
    }

    /// <summary>
    /// 显示错误信息.
    /// </summary>
    private void DisplayException(Exception exception)
    {
        IsError = true;
        var msg = GetErrorMessage(exception);
        ErrorText = $"{ResourceToolkit.GetLocalizedString(StringNames.RequestVideoFailed)}\n{msg}";
        LogException(exception);
    }

    private void DisplayFavoriteFoldersException(Exception exception)
    {
        IsFavoriteFoldersError = true;
        var msg = GetErrorMessage(exception);
        FavoriteFoldersErrorText = $"{ResourceToolkit.GetLocalizedString(StringNames.RequestVideoFavoriteFailed)}\n{msg}";
        LogException(exception);
    }

    private void CheckSectionVisibility()
    {
        IsShowInformation = CurrentSection.Type == PlayerSectionType.VideoInformation;
        IsShowUgcSeason = CurrentSection.Type == PlayerSectionType.UgcSeason;
        IsShowRelatedVideos = CurrentSection.Type == PlayerSectionType.RelatedVideos;
        IsShowParts = CurrentSection.Type == PlayerSectionType.VideoParts;
        IsShowComments = CurrentSection.Type == PlayerSectionType.Comments;
        IsShowVideoPlaylist = CurrentSection.Type == PlayerSectionType.Playlist;
    }

    [RelayCommand]
    private void SelectSeason(VideoSeason season)
    {
        CurrentSeason = season;
        TryClear(CurrentSeasonVideos);
        if (season == null)
        {
            return;
        }

        foreach (var item in CurrentSeason.Videos)
        {
            var vm = GetItemViewModel(item);
            vm.IsSelected = vm.Data.Equals(View.Information);
            CurrentSeasonVideos.Add(vm);
        }
    }

    [RelayCommand]
    private void ChangeVideoPart(VideoIdentifier identifier)
    {
        foreach (var item in VideoParts)
        {
            item.IsSelected = item.Data.Equals(identifier);
        }

        CurrentVideoPart = VideoParts.FirstOrDefault(p => p.IsSelected).Data;
        CreatePlayNextAction();
        CreatePlayPreviousAction();

        PlayerDetail.IsInPlaylist = _playNextVideoAction != null || _playPreviousVideoAction != null;
        PlayerDetail.ChangePartCommand.Execute(identifier);
    }

    private void CreatePlayNextAction()
    {
        PlayerDetail.CanPlayNextPart = View.InteractionVideo == null
            && (VideoParts.FirstOrDefault(p => p.IsSelected).Index < VideoParts.Last().Index
                || (VideoPlaylist.Count > 0 && VideoPlaylist.Last().Data != View.Information));
        _playNextVideoAction = null;

        // 不处理互动视频.
        if (View.InteractionVideo != null)
        {
            return;
        }

        var isNewVideo = false;
        VideoIdentifier? nextPart = default;
        if (Sections.Any(p => p.Type == PlayerSectionType.VideoParts))
        {
            var canContinue = VideoParts.Count > 1 && CurrentVideoPart.Id != VideoParts.Last().Data.Id;
            if (canContinue)
            {
                var currentPart = VideoParts.First(p => p.Data.Id == CurrentVideoPart.Id);
                nextPart = VideoParts.FirstOrDefault(p => p.Index == currentPart.Index + 1)?.Data;
            }
        }
        else if (Sections.Any(p => p.Type == PlayerSectionType.Playlist))
        {
            var canContinue = VideoPlaylist.Count > 1 && !View.Information.Equals(VideoPlaylist.Last().Data);
            if (canContinue)
            {
                var currentIndex = VideoPlaylist.IndexOf(VideoPlaylist.FirstOrDefault(p => p.Data.Equals(View.Information)));
                if (currentIndex != -1)
                {
                    isNewVideo = true;
                    nextPart = VideoPlaylist[currentIndex + 1].Data.Identifier;
                }
            }
        }
        else if (Sections.Any(p => p.Type == PlayerSectionType.UgcSeason))
        {
            ClearPlaylistCommand.Execute(default);
            var currentVideo = CurrentSeasonVideos.FirstOrDefault(p => p.IsSelected);
            if (currentVideo != null)
            {
                var canContinue = CurrentSeasonVideos.Count > 1 && !CurrentSeasonVideos.Last().Equals(currentVideo);
                if (canContinue)
                {
                    var index = CurrentSeasonVideos.IndexOf(currentVideo);
                    nextPart = CurrentSeasonVideos[index + 1].Data.Identifier;
                    isNewVideo = true;
                }
            }
        }
        else if (SettingsToolkit.ReadLocalSetting(SettingNames.IsAutoPlayNextRelatedVideo, false)
            && RelatedVideos.Count > 0)
        {
            ClearPlaylistCommand.Execute(default);
            nextPart = RelatedVideos.First().Data.Identifier;
            isNewVideo = true;
        }

        if (nextPart == null)
        {
            return;
        }

        PlayerDetail.NextVideoTipText = nextPart.Value.Title;
        _playNextVideoAction = () =>
        {
            if (isNewVideo)
            {
                SetSnapshot(new PlaySnapshot(nextPart.Value.Id, default, VideoType.Video));
            }
            else
            {
                ChangeVideoPart(nextPart.Value);
            }
        };

        PlayerDetail.SetPlayNextAction(_playNextVideoAction);
    }

    private void CreatePlayPreviousAction()
    {
        PlayerDetail.CanPlayPreviousPart = View.InteractionVideo == null
            && (VideoParts.FirstOrDefault(p => p.IsSelected).Index > 0
                || (VideoPlaylist.Count > 0 && VideoPlaylist.First().Data != View.Information));
        _playPreviousVideoAction = null;

        // 不处理互动视频.
        if (View.InteractionVideo != null)
        {
            return;
        }

        var isNewVideo = false;
        VideoIdentifier? previousPart = default;
        if (Sections.Any(p => p.Type == PlayerSectionType.VideoParts))
        {
            var canContinue = VideoParts.Count > 1 && CurrentVideoPart.Id != VideoParts.First().Data.Id;
            if (canContinue)
            {
                var currentPart = VideoParts.First(p => p.Data.Id == CurrentVideoPart.Id);
                previousPart = VideoParts.FirstOrDefault(p => p.Index == currentPart.Index - 1)?.Data;
            }
        }
        else if (Sections.Any(p => p.Type == PlayerSectionType.Playlist))
        {
            var canContinue = VideoPlaylist.Count > 1 && !View.Information.Equals(VideoPlaylist.First().Data);
            if (canContinue)
            {
                var currentIndex = VideoPlaylist.IndexOf(VideoPlaylist.FirstOrDefault(p => p.Data.Equals(View.Information)));
                if (currentIndex != -1)
                {
                    isNewVideo = true;
                    previousPart = VideoPlaylist[currentIndex - 1].Data.Identifier;
                }
            }
        }
        else if (Sections.Any(p => p.Type == PlayerSectionType.UgcSeason))
        {
            ClearPlaylistCommand.Execute(default);
            var currentVideo = CurrentSeasonVideos.FirstOrDefault(p => p.IsSelected);
            if (currentVideo != null)
            {
                var canContinue = CurrentSeasonVideos.Count > 1 && !CurrentSeasonVideos.First().Equals(currentVideo);
                if (canContinue)
                {
                    var index = CurrentSeasonVideos.IndexOf(currentVideo);
                    previousPart = CurrentSeasonVideos[index - 1].Data.Identifier;
                    isNewVideo = true;
                }
            }
        }
        else if (SettingsToolkit.ReadLocalSetting(SettingNames.IsAutoPlayNextRelatedVideo, false)
            && RelatedVideos.Count > 0)
        {
            ClearPlaylistCommand.Execute(default);
            previousPart = RelatedVideos.First().Data.Identifier;
            isNewVideo = true;
        }

        if (previousPart == null)
        {
            return;
        }

        _playPreviousVideoAction = () =>
        {
            if (isNewVideo)
            {
                SetSnapshot(new PlaySnapshot(previousPart.Value.Id, default, VideoType.Video));
            }
            else
            {
                ChangeVideoPart(previousPart.Value);
            }
        };

        PlayerDetail.SetPlayPreviousAction(_playPreviousVideoAction);
    }

    private void OnAuthorizeStateChanged(object sender, AuthorizeStateChangedEventArgs e)
        => IsSignedIn = e.NewState == AuthorizeState.SignedIn;

    private void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
    {
        var request = args.Request;
        var url = $"https://www.bilibili.com/video/{View.Information.AlternateId}";

        request.Data.Properties.Title = View.Information.Identifier.Title;
        request.Data.Properties.Description = View.Information.Description;
        request.Data.Properties.Thumbnail = RandomAccessStreamReference.CreateFromUri(View.Information.Identifier.Cover.GetSourceUri());
        request.Data.Properties.ContentSourceWebLink = new Uri(url);

        request.Data.SetText(View.Information.Description);
        request.Data.SetWebLink(new Uri(url));
        request.Data.SetBitmap(RandomAccessStreamReference.CreateFromUri(View.Information.Identifier.Cover.GetSourceUri()));
    }

    private void OnMediaEnded(object sender, EventArgs e)
    {
        if (_playNextVideoAction != null)
        {
            var isContinue = SettingsToolkit.ReadLocalSetting(SettingNames.IsContinuePlay, true);
            if (isContinue)
            {
                _playNextVideoAction?.Invoke();
            }
            else
            {
                PlayerDetail.ShowNextVideoTipCommand.Execute(default);
            }

            return;
        }

        var isAutoCloseWindowWhenEnded = SettingsToolkit.ReadLocalSetting(SettingNames.IsAutoCloseWindowWhenEnded, false);
        if (isAutoCloseWindowWhenEnded)
        {
            PlayerDetail.ShowAutoCloseWindowTipCommand.Execute(default);
        }
    }

    private void OnInternalPartChanged(object sender, VideoIdentifier e)
        => ChangeVideoPart(e);

    private async void OnRequestOpenInBrowserAsync(object sender, EventArgs e)
    {
        var uri = $"https://www.bilibili.com/video/av{View.Information.Identifier.Id}";
        _ = await Launcher.LaunchUriAsync(new Uri(uri));
    }

    private void OnPlayerDetailPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PlayerDetail.Status)
            && PlayerDetail.Status == PlayerStatus.Playing
            && !_isStatsUpdated
            && PlayerDetail.Player != null
            && PlayerDetail.Player.IsMediaStatsSupported)
        {
            var info = PlayerDetail.Player.GetMediaInformation();
            if (info == null)
            {
                return;
            }

            var media = new VideoMediaStats(info)
            {
                VideoUrl = PlayerDetail.GetVideoPlayUrl(),
                AudioUrl = PlayerDetail.GetAudioPlayUrl(),
            };
            Stats = media;

            _isStatsUpdated = true;
        }
    }
}
