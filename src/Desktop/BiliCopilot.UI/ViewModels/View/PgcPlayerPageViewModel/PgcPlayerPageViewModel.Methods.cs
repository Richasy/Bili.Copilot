// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Appearance;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// PGC 播放器页面视图模型.
/// </summary>
public sealed partial class PgcPlayerPageViewModel
{
    private void InitializeView(PgcPlayerView view)
    {
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

        ChangeFormatCommand.Execute(selectedFormat);
    }

    private void InitializeSections()
    {
        if (Sections?.Count > 0)
        {
            return;
        }

        var sections = new List<IPlayerSectionDetailViewModel>();

        if (_view.Episodes?.Count > 1)
        {
            sections.Add(new PgcPlayerEpisodeSectionDetailViewModel(_view.Episodes, _episode.Identifier.Id, ChangeEpisode));
        }

        if (_view.Seasons?.Count > 1)
        {
            sections.Add(new PgcPlayerSeasonSectionDetailViewModel(_view.Seasons, _view.Information.Identifier.Id));
        }

        if (sections.Count == 0)
        {
            sections.Add(_comments);
        }
        else
        {
            sections.Insert(1, _comments);
        }

        Sections = sections;
        SelectSection(Sections.First());
        SectionInitialized?.Invoke(this, EventArgs.Empty);
    }

    private void ClearView()
    {
        IsPageLoadFailed = false;
        _view = default;
        _videoSegments = default;
        _audioSegments = default;
        _initialProgress = -1;
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

        Sections = default;
        SelectedSection = default;
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

    private object? FindNextEpisode()
    {
        _ = this;
        return default;
    }

    private void InitializeNextEpisode()
    {
        var next = FindNextEpisode();
        HasNextEpisode = next is not null;
        if (next is EpisodeInformation part)
        {
            NextEpisodeTip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.PlayNextEpisodeTipTemplate), part.Identifier.Title);
        }
        else if (next is VideoInformation video)
        {
            NextEpisodeTip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.PlayNextVideoTipTemplate), video.Identifier.Title);
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

    private void CalcPlayerHeight()
    {
        if (PlayerWidth <= 0 || Player.IsFullScreen || Player.IsCompactOverlay || _episode is null)
        {
            return;
        }

        var ratio = _episode.GetExtensionIfNotNull<MediaAspectRatio>(EpisodeExtensionDataId.AspectRatio);
        if (ratio.Width == 0)
        {
            ratio = new MediaAspectRatio(16, 9);
        }

        PlayerHeight = (double)(PlayerWidth * ratio.Height / ratio.Width);
    }
}
