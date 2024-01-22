// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.ComponentModel;
using System.Linq;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Humanizer;
using Windows.System;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// WebDav 播放器视图模型.
/// </summary>
public sealed partial class WebDavPlayerPageViewModel
{
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

    private void CheckSectionVisibility()
    {
        IsShowInformation = CurrentSection.Type == PlayerSectionType.VideoInformation;
        IsShowPlaylist = CurrentSection.Type == PlayerSectionType.Playlist;
    }

    [RelayCommand]
    private void ChangeVideo(WebDavStorageItemViewModel vm)
    {
        foreach (var item in Playlist)
        {
            item.IsSelected = item.Equals(vm);
        }

        CurrentItem = vm;
        CreatePlayNextAction();
        CreatePlayPreviousAction();
        PlayerDetail.IsInPlaylist = _playNextVideoAction != null || _playPreviousVideoAction != null;

        var videoInfo = new WebDavVideoInformation
        {
            Config = _config,
            ContentType = vm.Data.ContentType,
            Href = vm.Data.Uri,
        };

        InitializeInfo();
        PlayerDetail.SetWebDavData(videoInfo);
    }

    private void CreatePlayNextAction()
    {
        var currentIndex = Playlist.IndexOf(CurrentItem);
        PlayerDetail.CanPlayNextPart = currentIndex < Playlist.Count - 1;
        _playNextVideoAction = null;

        WebDavStorageItemViewModel nextPart = default;
        if (Sections.Any(p => p.Type == PlayerSectionType.Playlist))
        {
            var canContinue = Playlist.Count > 1 && currentIndex < Playlist.Count - 1;
            if (canContinue)
            {
                nextPart = Playlist[currentIndex + 1];
            }
        }

        if (nextPart == null)
        {
            PlayerDetail.CanPlayNextPart = false;
            return;
        }

        PlayerDetail.NextVideoTipText = nextPart.Data.DisplayName;
        PlayerDetail.NextPartText = string.Format(ResourceToolkit.GetLocalizedString(StringNames.NextPartTipTemplate), nextPart.Data.DisplayName);
        _playNextVideoAction = () =>
        {
            ChangeVideo(nextPart);
        };

        PlayerDetail.SetPlayNextAction(_playNextVideoAction);
    }

    private void CreatePlayPreviousAction()
    {
        var currentIndex = Playlist.IndexOf(CurrentItem);
        PlayerDetail.CanPlayPreviousPart = currentIndex > 0;
        _playNextVideoAction = null;

        WebDavStorageItemViewModel previousPart = default;
        if (Sections.Any(p => p.Type == PlayerSectionType.Playlist))
        {
            var canContinue = Playlist.Count > 1 && currentIndex > 0;
            if (canContinue)
            {
                previousPart = Playlist[currentIndex - 1];
            }
        }

        if (previousPart == null)
        {
            PlayerDetail.CanPlayPreviousPart = false;
            return;
        }

        PlayerDetail.PreviousPartText = string.Format(ResourceToolkit.GetLocalizedString(StringNames.PreviousPartTipTemplate), previousPart.Data.DisplayName);
        _playPreviousVideoAction = () =>
        {
            ChangeVideo(previousPart);
        };

        PlayerDetail.SetPlayPreviousAction(_playNextVideoAction);
    }

    private void ReloadMediaPlayer()
    {
        if (PlayerDetail != null)
        {
            return;
        }

        PlayerDetail = new PlayerDetailViewModel(_attachedWindow);
        PlayerDetail.MediaEnded += OnMediaEnded;
        PlayerDetail.RequestOpenInBrowser += OnRequestOpenInBrowserAsync;
        PlayerDetail.PropertyChanged += OnPlayerDetailPropertyChanged;
    }

    private void InitializeInfo()
    {
        if (CurrentItem == null)
        {
            ResetInfo();
            return;
        }

        FileName = CurrentItem.Data.DisplayName ?? "--";
        PublishTime = (CurrentItem.Data.CreationDate ?? CurrentItem.Data.LastModifiedDate ?? DateTime.Now).Humanize();
    }

    private void ResetInfo()
    {
        FileName = "--";
        PublishTime = "--";
        IsError = false;
        ErrorText = string.Empty;
    }

    private void OnMediaEnded(object sender, EventArgs e)
    {
        if (_playNextVideoAction != null)
        {
            if (IsContinuePlay)
            {
                _playNextVideoAction?.Invoke();
            }
            else
            {
                PlayerDetail.ShowNextVideoTipCommand.Execute(default);
            }

            return;
        }
    }

    private async void OnRequestOpenInBrowserAsync(object sender, EventArgs e)
    {
        var server = AppToolkit.GetWebDavServer(_config.Host, _config.Port, CurrentItem.Data.Uri);
        var uri = server + CurrentItem.Data.Uri;
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

            var media = new VideoMediaStats(info);
            Stats = media;

            _isStatsUpdated = true;
        }
    }
}
