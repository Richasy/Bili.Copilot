// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// PGC 源视图模型.
/// </summary>
public sealed partial class PgcSourceViewModel
{
    private void InitializeView(PgcPlayerView view)
    {
        if (view is null)
        {
            return;
        }

        _view = view;
        _type = view.Information.GetExtensionIfNotNull<EntertainmentType>(SeasonExtensionDataId.PgcType);
        Cover = view.Information.Identifier.Cover.SourceUri;
        SeasonTitle = view.Information.Identifier.Title;
        Description = view.Information.GetExtensionIfNotNull<string>(SeasonExtensionDataId.Description);
        Alias = view.Information.GetExtensionIfNotNull<string>(SeasonExtensionDataId.Alias);
        if (string.IsNullOrEmpty(Alias))
        {
            Alias = view.Information.GetExtensionIfNotNull<string>(SeasonExtensionDataId.Subtitle);
        }

        SeasonId = view.Information.Identifier.Id;
        IsFollow = view.Information.IsTracking ?? false;
        PlayCount = view.Information.CommunityInformation.PlayCount ?? 0;
        DanmakuCount = view.Information.CommunityInformation.DanmakuCount ?? 0;
        CommentCount = view.Information.CommunityInformation.CommentCount ?? 0;
        LikeCount = view.Information.CommunityInformation.LikeCount ?? 0;
        CoinCount = view.Information.CommunityInformation.CoinCount ?? 0;
        FavoriteCount = view.Information.CommunityInformation.FavoriteCount ?? 0;
        FollowCount = view.Information.CommunityInformation.TrackCount ?? 0;
    }

    private void InitializeDash(DashMediaInformation info)
    {
        _videoSegments = info.Videos;
        _audioSegments = info.Audios;
        Downloader.Clear();
        Formats = info.Formats.Select(p => new PlayerFormatItemViewModel(p)).ToList();

        var preferFormatSetting = SettingsToolkit.ReadLocalSetting(SettingNames.PreferQuality, PreferQualityType.Auto);
        var availableFormats = Formats.Where(p => p.IsEnabled).ToList();
        PlayerFormatItemViewModel? selectedFormat = default;
        if (preferFormatSetting == PreferQualityType.Auto)
        {
            var lastSelectedFormat = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedPgcQuality, 0);
            selectedFormat = availableFormats.Find(p => p.Data.Quality == lastSelectedFormat);
        }
        else if (preferFormatSetting == PreferQualityType.UHD)
        {
            selectedFormat = availableFormats.Find(p => p.Data.Quality == 120);
        }
        else if (preferFormatSetting == PreferQualityType.HD)
        {
            selectedFormat = availableFormats.Find(p => p.Data.Quality == 116 || p.Data.Quality == 80);
        }

        if (selectedFormat is null)
        {
            var maxQuality = availableFormats.Max(p => p.Data.Quality);
            selectedFormat = availableFormats.Find(p => p.Data.Quality == maxQuality);
        }

        var episodeIndex = _view.Episodes.IndexOf(_episode);
        Downloader.InitializeMetas(
            GetEpisodeUrl(),
            GetSeasonUrl(),
            info.Formats.AsReadOnly(),
            _view.Episodes?.Count > 1 ? _view.Episodes.AsReadOnly() : default,
            episodeIndex);

        ChangeFormatCommand.Execute(selectedFormat);
    }

    private void InitializeSections()
    {
        if (_isSeasonInitialized)
        {
            return;
        }

        EpisodeSection = _view.Episodes?.Count > 1 ? new PgcPlayerEpisodeSectionDetailViewModel(_view.Episodes, _episode.Identifier.Id, ChangeEpisode) : null;
        SeasonSection = _view.Seasons?.Count > 1 ? new PgcPlayerSeasonSectionDetailViewModel(_view.Seasons, _view.Information.Identifier.Id) : null;
        _isSeasonInitialized = true;
    }

    private void ClearView()
    {
        ErrorMessage = string.Empty;
        _view = default;
        _videoSegments = default;
        _audioSegments = default;
        _initialProgress = 0;
        Cover = default;
        SeasonTitle = default;
        IsFollow = false;
        PlayCount = 0;
        DanmakuCount = 0;
        CommentCount = 0;
        LikeCount = 0;
        CoinCount = 0;
        FavoriteCount = 0;
        FollowCount = 0;
        IsLiked = false;
        IsCoined = false;
        IsFavorited = false;
        IsCoinAlsoLike = true;
        FavoriteFolders = default;
        HasNextEpisode = false;

        Formats = default;
        SelectedFormat = default;
    }

    private EpisodeInformation? FindInitialEpisode(string? initialEpisodeId)
    {
        EpisodeInformation? playEpisode = default;
        if (!string.IsNullOrEmpty(initialEpisodeId))
        {
            playEpisode = _view.Episodes.FirstOrDefault(p => p.Identifier.Id == initialEpisodeId);
        }

        if (playEpisode == null)
        {
            var historyEpisodeId = _view.Progress?.Cid;
            var autoLoadHistory = SettingsToolkit.ReadLocalSetting(SettingNames.AutoLoadHistory, true);
            if (!string.IsNullOrEmpty(historyEpisodeId) && autoLoadHistory)
            {
                playEpisode = _view.Episodes.FirstOrDefault(p => p.Identifier.Id == historyEpisodeId);
            }
        }

        return playEpisode ?? _view.Episodes.FirstOrDefault();
    }

    private object? FindPrevEpisode()
    {
        if (EpisodeSection is null)
        {
            return default;
        }

        var index = EpisodeSection.Episodes.ConvertAll(p => p.Data).IndexOf(_episode);
        if (index > 0)
        {
            return EpisodeSection.Episodes[index - 1].Data;
        }

        return default;
    }

    private object? FindNextEpisode()
    {
        if (EpisodeSection is null)
        {
            return default;
        }

        var index = EpisodeSection.Episodes.ConvertAll(p => p.Data).IndexOf(_episode);
        if (index < EpisodeSection.Episodes.Count - 1)
        {
            return EpisodeSection.Episodes[index + 1].Data;
        }

        return default;
    }

    private void InitializeEpisodeNavigation()
    {
        if (_view is null || !_isSeasonInitialized)
        {
            return;
        }

        var next = FindNextEpisode();
        var prev = FindPrevEpisode();
        HasNextEpisode = next is not null;
        HasPrevEpisode = prev is not null;
        if (next is EpisodeInformation part)
        {
            NextEpisodeTip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.PlayNextEpisodeTipTemplate), part.Identifier.Title);
        }
        else if (next is VideoInformation video)
        {
            NextEpisodeTip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.PlayNextVideoTipTemplate), video.Identifier.Title);
        }

        if (prev is EpisodeInformation part2)
        {
            PrevEpisodeTip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.PlayPrevEpisodeTipTemplate), part2.Identifier.Title);
        }
        else if (prev is VideoInformation video2)
        {
            PrevEpisodeTip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.PlayPrevVideoTipTemplate), video2.Identifier.Title);
        }
    }

    private void LoadInitialProgress(int? progress = default)
    {
        _initialProgress = 0;
        if (progress is not null)
        {
            _initialProgress = progress.Value;
        }
        else if (_view.Progress is not null && _view.Progress.Cid == _episode.Identifier.Id)
        {
            var p = Convert.ToInt32(_view.Progress.Progress);
            if (p < _episode.Duration - 5)
            {
                _initialProgress = p;
            }
        }
    }

    private void ReloadEpisode()
    {
        if (_episode is null)
        {
            return;
        }

        ChangeEpisode(_episode);
    }
}
