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
using Bili.Copilot.Models.Data.Local;
using Bili.Copilot.Models.Data.Pgc;
using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.Input;
using Windows.System;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// PGC 播放页面视图模型.
/// </summary>
public sealed partial class PgcPlayerPageViewModel
{
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
        IsShowInformation = CurrentSection.Type == PlayerSectionType.PgcInformation;
        IsShowSeasons = CurrentSection.Type == PlayerSectionType.Seasons;
        IsShowEpisodes = CurrentSection.Type == PlayerSectionType.Episodes;
        IsShowExtras = CurrentSection.Type == PlayerSectionType.Extras;
        IsShowComments = CurrentSection.Type == PlayerSectionType.Comments;
    }

    [RelayCommand]
    private void ChangeSeason(SeasonInformation season)
        => SetSnapshot(new PlaySnapshot(default, season.Identifier.Id, VideoType.Pgc));

    [RelayCommand]
    private void ChangeEpisode(EpisodeInformation episode)
    {
        PlayerDetail.Player?.Stop();
        CurrentEpisode = episode;
        foreach (var item in Episodes)
        {
            item.IsSelected = episode.Identifier.Id == item.Data.Identifier.Id;
        }

        if (Extras.Count > 0)
        {
            foreach (var extra in Extras)
            {
                foreach (var item in extra.Episodes)
                {
                    item.IsSelected = episode.Identifier.Id == item.Data.Identifier.Id;
                }
            }
        }

        _ = ReloadCommunityInformationCommand.ExecuteAsync(null);
        PlayerDetail.SetPgcData(View, CurrentEpisode);

        CreatePlayNextAction();
        CreatePlayPreviousAction();
        PlayerDetail.IsInPlaylist = _playNextEpisodeAction != null || _playPreviousEpisodeAction != null;
    }

    private void CreatePlayNextAction()
    {
        _playNextEpisodeAction = null;

        // 当前分集为空时不处理.
        if (CurrentEpisode == null)
        {
            return;
        }

        PlayerDetail.CanPlayNextPart = CurrentEpisode.IsPreviewVideo
            ? !Extras.LastOrDefault()?.Equals(CurrentEpisode) ?? false
            : !Episodes.LastOrDefault()?.Equals(CurrentEpisode) ?? false;

        EpisodeInformation nextPart = default;
        var isPreview = CurrentEpisode.IsPreviewVideo;
        if (!isPreview && Sections.Any(p => p.Type == PlayerSectionType.Episodes))
        {
            var canContinue = Episodes.Count > 1 && CurrentEpisode != Episodes.Last().Data;
            if (canContinue)
            {
                nextPart = Episodes.FirstOrDefault(p => p.Data.Index == CurrentEpisode.Index + 1)?.Data;
            }
        }
        else if (isPreview && Sections.Any(p => p.Type == PlayerSectionType.Extras))
        {
            var extras = Extras.SelectMany(p => p.Episodes).ToList();
            var index = extras.IndexOf(extras.FirstOrDefault(p => p.Equals(CurrentEpisode)));
            var canContinue = index != -1 && extras.Count > 1 && CurrentEpisode != extras.Last().Data;
            if (canContinue)
            {
                nextPart = extras[index + 1].Data;
            }
        }

        if (nextPart == null)
        {
            PlayerDetail.CanPlayNextPart = false;
            return;
        }

        PlayerDetail.NextVideoTipText = nextPart.Identifier.Title;
        PlayerDetail.NextPartText = string.Format(ResourceToolkit.GetLocalizedString(StringNames.NextPartTipTemplate), nextPart.Identifier.Title);
        _playNextEpisodeAction = () =>
        {
            ChangeEpisode(nextPart);
        };

        PlayerDetail.SetPlayNextAction(_playNextEpisodeAction);
    }

    private void CreatePlayPreviousAction()
    {
        _playPreviousEpisodeAction = null;

        // 当前分集为空时不处理.
        if (CurrentEpisode == null)
        {
            return;
        }

        PlayerDetail.CanPlayPreviousPart = CurrentEpisode.IsPreviewVideo
            ? !Extras.FirstOrDefault()?.Equals(CurrentEpisode) ?? false
            : !Episodes.FirstOrDefault()?.Equals(CurrentEpisode) ?? false;

        EpisodeInformation previousPart = default;
        var isPreview = CurrentEpisode.IsPreviewVideo;
        if (!isPreview && Sections.Any(p => p.Type == PlayerSectionType.Episodes))
        {
            var canContinue = Episodes.Count > 1 && CurrentEpisode != Episodes.First().Data;
            if (canContinue)
            {
                previousPart = Episodes.FirstOrDefault(p => p.Data.Index == CurrentEpisode.Index - 1)?.Data;
            }
        }
        else if (isPreview && Sections.Any(p => p.Type == PlayerSectionType.Extras))
        {
            var extras = Extras.SelectMany(p => p.Episodes).ToList();
            var index = extras.IndexOf(extras.FirstOrDefault(p => p.Equals(CurrentEpisode)));
            var canContinue = index != -1 && extras.Count > 1 && CurrentEpisode != extras.First().Data;
            if (canContinue)
            {
                previousPart = extras[index - 1].Data;
            }
        }

        if (previousPart == null)
        {
            PlayerDetail.CanPlayPreviousPart = false;
            return;
        }

        PlayerDetail.PreviousPartText = string.Format(ResourceToolkit.GetLocalizedString(StringNames.PreviousPartTipTemplate), previousPart.Identifier.Title);
        _playPreviousEpisodeAction = () =>
        {
            ChangeEpisode(previousPart);
        };

        PlayerDetail.SetPlayPreviousAction(_playPreviousEpisodeAction);
    }

    private void OnAuthorizeStateChanged(object sender, AuthorizeStateChangedEventArgs e)
        => IsSignedIn = e.NewState == AuthorizeState.SignedIn;

    private void OnMediaEnded(object sender, EventArgs e)
    {
        if (_playNextEpisodeAction == null)
        {
            var isAutoCloseWindowWhenEnded = SettingsToolkit.ReadLocalSetting(SettingNames.IsAutoCloseWindowWhenEnded, false);
            if (isAutoCloseWindowWhenEnded)
            {
                PlayerDetail.ShowAutoCloseWindowTipCommand.Execute(default);
            }

            return;
        }

        var isContinue = SettingsToolkit.ReadLocalSetting(SettingNames.IsContinuePlay, true);
        if (isContinue)
        {
            _playNextEpisodeAction?.Invoke();
        }
        else
        {
            PlayerDetail.ShowNextVideoTipCommand.Execute(default);
        }
    }

    private void OnInternalPartChanged(object sender, VideoIdentifier e)
    {
        var episode = Episodes.FirstOrDefault(p => p.Data.Identifier.Id == e.Id);
        if (episode == null && Extras.Count > 0)
        {
            episode = Extras.SelectMany(p => p.Episodes).FirstOrDefault(p => p.Data.Identifier.Id == e.Id);
        }

        if (episode == null)
        {
            return;
        }

        ChangeEpisodeCommand.Execute(episode.Data);
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IsOnlyShowIndex))
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.IsOnlyShowIndex, IsOnlyShowIndex);
        }
    }

    partial void OnCurrentSectionChanged(PlayerSectionHeader value)
    {
        if (value != null)
        {
            CheckSectionVisibility();
        }
    }

    private async void OnRequestOpenInBrowserAsync(object sender, EventArgs e)
    {
        var uri = !string.IsNullOrEmpty(_presetEpisodeId)
            ? $"https://www.bilibili.com/bangumi/play/ep{_presetEpisodeId}"
            : $"https://www.bilibili.com/bangumi/play/ss{_presetSeasonId}";
        _ = await Launcher.LaunchUriAsync(new Uri(uri));
    }

    private void OnPlayerDetailPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PlayerDetail.Status)
            && PlayerDetail.Status == PlayerStatus.Playing
            && PlayerDetail.Player != null
            && !PlayerDetail.Player.IsStatsUpdated
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
        }
    }
}
