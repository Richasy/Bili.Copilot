// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using Humanizer;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 视频播放器页面视图模型.
/// </summary>
public sealed partial class VideoPlayerPageViewModel
{
    private void InitializeView(VideoPlayerView view)
    {
        _view = view;
        Cover = view.Information.Identifier.Cover.SourceUri;
        Title = view.Information.Identifier.Title;
        AvId = view.Information.Identifier.Id;
        BvId = view.Information.BvId;
        IsMyVideo = _view.Information.Publisher.User.Id == this.Get<IBiliTokenResolver>().GetToken().UserId.ToString();
        UpAvatar = view.Information.Publisher.User.Avatar.Uri;
        UpName = view.Information.Publisher.User.Name;
        var relativeTime = DateTimeOffset.Now - view.Information.PublishTime > TimeSpan.FromDays(90) ? view.Information.PublishTime!.Value.ToString("yyyy-MM-dd HH:mm:ss") : view.Information.PublishTime.Humanize(default, new System.Globalization.CultureInfo("zh-CN"));
        PublishRelativeTime = string.Format(ResourceToolkit.GetLocalizedString(StringNames.AuthorPublishTime), relativeTime);
        Description = view.Information.GetExtensionIfNotNull<string>(VideoExtensionDataId.Description);
        IsFollow = view.OwnerCommunity.Relation != Richasy.BiliKernel.Models.User.UserRelationStatus.Unfollow && view.OwnerCommunity.Relation != Richasy.BiliKernel.Models.User.UserRelationStatus.Unknown;
        IsInteractionVideo = view.IsInteractiveVideo;
        Tags = view.Tags.ToList();
        InitializeCommunityInformation(view.Information.CommunityInformation);
        IsLiked = view.Operation.IsLiked;
        IsCoined = view.Operation.IsCoined;
        IsFavorited = view.Operation.IsFavorited;
        IsCoinAlsoLike = true;
        Player.Title = Title;
        CalcPlayerHeight();
    }

    private void InitializeCommunityInformation(VideoCommunityInformation info)
    {
        PlayCount = info.PlayCount ?? 0;
        DanmakuCount = info.DanmakuCount ?? 0;
        CommentCount = info.CommentCount ?? 0;
        LikeCount = info.LikeCount ?? 0;
        CoinCount = info.CoinCount ?? 0;
        FavoriteCount = info.FavoriteCount ?? 0;
    }

    private void InitializeDash(DashMediaInformation info)
    {
        _videoSegments = info.Videos;
        _audioSegments = info.Audios;

        // 这里针对 MPV 额外过滤掉杜比视界画质，目前使用 libmpv 暂时还不能正确处理色域映射.
        Downloader.Clear();
        var isMpvPlayer = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerType, PlayerType.Island) == PlayerType.Mpv;
        var formats = isMpvPlayer ? info.Formats.Where(p => p.Quality != 126) : info.Formats;
        Formats = formats.Select(p => new PlayerFormatItemViewModel(p)).ToList();

        // 用户个人视频无需会员即可观看最高画质.
        if (IsMyVideo)
        {
            foreach (var format in Formats)
            {
                format.IsEnabled = true;
            }
        }

        if (_view == default)
        {
            return;
        }

        var preferFormatSetting = SettingsToolkit.ReadLocalSetting(SettingNames.PreferQuality, PreferQualityType.Auto);
        var availableFormats = Formats.Where(p => p.IsEnabled).ToList();
        var partIndex = _view.Parts?.IndexOf(_part) ?? 0;

        // 下载可以选择所有清晰度.
        Downloader.InitializeMetas(
            GetWebLink(),
            info.Formats.AsReadOnly(),
            _view.Parts?.Count > 1 ? _view.Parts.AsReadOnly() : default,
            partIndex + 1);
        PlayerFormatItemViewModel? selectedFormat = default;
        if (preferFormatSetting == PreferQualityType.Auto)
        {
            var lastSelectedFormat = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedVideoQuality, 0);
            selectedFormat = availableFormats.Find(p => p.Data.Quality == lastSelectedFormat);
        }
        else if (preferFormatSetting == PreferQualityType.UHD)
        {
            selectedFormat = availableFormats.Find(p => p.Data.Quality == 120);
        }
        else if (preferFormatSetting == PreferQualityType.HD)
        {
            var hdFormats = availableFormats.Where(p => p.Data.Quality == 116 || p.Data.Quality == 80).ToList();
            selectedFormat = hdFormats.OrderByDescending(p => p.Data.Quality).FirstOrDefault();
        }
        else if (preferFormatSetting == PreferQualityType.HDReady)
        {
            selectedFormat = availableFormats.Find(p => p.Data.Quality == 64);
        }
        else if (preferFormatSetting == PreferQualityType.SD)
        {
            selectedFormat = availableFormats.Find(p => p.Data.Quality == 32);
        }
        else if (preferFormatSetting == PreferQualityType.Smooth)
        {
            selectedFormat = availableFormats.Find(p => p.Data.Quality == 16);
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

        var sections = new List<IPlayerSectionDetailViewModel>
        {
            new VideoPlayerInfoSectionDetailViewModel(this),
        };

        if (_view.Seasons is not null)
        {
            sections.Insert(0, new VideoPlayerSeasonSectionDetailViewModel(this, _view.Seasons, AvId));
        }

        if (_view.Parts?.Count > 1)
        {
            sections.Add(new VideoPlayerPartSectionDetailViewModel(_view.Parts, _part.Identifier.Id, ChangePart));
        }

        if (_playlist is not null)
        {
            sections.Insert(0, new VideoPlayerPlaylistSectionDetailViewModel(this, _playlist, AvId));
        }

        if (_view.Recommends is not null)
        {
            sections.Add(new VideoPlayerRecommendSectionDetailViewModel(_view.Recommends));
        }

        sections.Add(_comments);
        sections.Add(new VideoPlayerAISectionDetailViewModel(AI));

        Sections = sections;
        SelectSection(Sections.First());
        SectionInitialized?.Invoke(this, EventArgs.Empty);
    }

    private void ClearView()
    {
        IsPageLoadFailed = false;
        _view = default;
        _part = default;
        _videoSegments = default;
        _audioSegments = default;
        _initialProgress = -1;
        Tags = default;
        UpAvatar = default;
        IsFollow = false;
        IsMyVideo = false;
        PlayCount = 0;
        DanmakuCount = 0;
        CommentCount = 0;
        LikeCount = 0;
        CoinCount = 0;
        FavoriteCount = 0;
        IsLiked = false;
        IsCoined = false;
        IsFavorited = false;
        IsCoinAlsoLike = true;
        AvId = default;
        BvId = default;
        FavoriteFolders = default;
        SelectedFormat = default;
        Sections?.Clear();
    }

    private void CalcPlayerHeight()
    {
        if (PlayerWidth <= 0 || _view?.AspectRatio is null || Player.IsFullScreen || Player.IsCompactOverlay)
        {
            return;
        }

        PlayerHeight = (double)(PlayerWidth * _view.AspectRatio.Value.Height / _view.AspectRatio.Value.Width);
    }

    private VideoPart? FindInitialPart(string? initialPartId = default)
    {
        if (_view.Parts?.Count == 1)
        {
            return _view.Parts.First();
        }

        VideoPart? part = default;
        if (!string.IsNullOrEmpty(initialPartId))
        {
            part = _view.Parts.FirstOrDefault(p => p.Identifier.Id == initialPartId);
        }

        if (part == null)
        {
            var historyPartId = _view.Progress?.Cid;
            var autoLoadHistory = SettingsToolkit.ReadLocalSetting(SettingNames.AutoLoadHistory, true);
            if (!string.IsNullOrEmpty(historyPartId) && autoLoadHistory)
            {
                part = _view.Parts.FirstOrDefault(p => p.Identifier.Id == historyPartId);
            }
        }

        return part ?? _view.Parts.FirstOrDefault();
    }

    private void LoadInitialProgress(int? progress = default)
    {
        _initialProgress = 0;
        if (progress is not null)
        {
            _initialProgress = progress.Value;
        }
        else if (_view.Progress is not null)
        {
            var p = Convert.ToInt32(_view.Progress.Progress);
            if (p < _view.Information.Duration - 5)
            {
                _initialProgress = p;
            }
        }
    }

    private object? FindNextVideo()
    {
        var isListLoop = CurrentLoop == VideoLoopType.List;
        var hasUgcSeason = Sections.OfType<VideoPlayerSeasonSectionDetailViewModel>().Any();

        // 1. 先检查分P列表中是否有下一个视频.
        if (Sections.OfType<VideoPlayerPartSectionDetailViewModel>().FirstOrDefault() is VideoPlayerPartSectionDetailViewModel partSection)
        {
            var index = partSection.Parts.ToList().IndexOf(_part);
            if (index < partSection.Parts.Count - 1)
            {
                return partSection.Parts.ElementAt(index + 1);
            }

            if (_playlist is null && isListLoop && !hasUgcSeason)
            {
                return partSection.Parts.First();
            }
        }

        // 2. 检查播放列表中是否有下一个视频.
        if (_playlist is not null)
        {
            var index = _playlist.ToList().IndexOf(_view.Information);
            if (index < _playlist.Count - 1)
            {
                return _playlist.ElementAt(index + 1);
            }

            if (isListLoop)
            {
                return _playlist.First();
            }
            else if (SettingsToolkit.ReadLocalSetting(SettingNames.EndWithPlaylist, true))
            {
                return default;
            }
        }

        // 3. 检查合集中是否有下一个视频.
        if (Sections.OfType<VideoPlayerSeasonSectionDetailViewModel>().FirstOrDefault() is VideoPlayerSeasonSectionDetailViewModel seasonSection)
        {
            var index = seasonSection.Items.ToList().IndexOf(seasonSection.SelectedItem);
            if (index < seasonSection.Items.Count - 1)
            {
                return seasonSection.Items.ElementAt(index + 1).Data;
            }
            else if (isListLoop)
            {
                return seasonSection.Items.First().Data;
            }
        }

        // 4. 检查推荐视频中是否有下一个视频.
        var isAutoPlayRecommendVideo = SettingsToolkit.ReadLocalSetting(SettingNames.AutoPlayNextRecommendVideo, false);
        if (isAutoPlayRecommendVideo && Sections.OfType<VideoPlayerRecommendSectionDetailViewModel>().FirstOrDefault() is VideoPlayerRecommendSectionDetailViewModel recommendSection)
        {
            return recommendSection.Items.First().Data;
        }

        return default;
    }

    private void InitializeNextVideo()
    {
        if (_view is null || Sections.Count == 0)
        {
            return;
        }

        var next = FindNextVideo();
        HasNextVideo = next is not null;
        if (next is VideoPart part)
        {
            NextVideoTip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.PlayNextPartTipTemplate), part.Identifier.Title);
        }
        else if (next is VideoInformation video)
        {
            NextVideoTip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.PlayNextVideoTipTemplate), video.Identifier.Title);
        }
    }

    private void ReloadPart()
    {
        if (_part is null)
        {
            return;
        }

        if (Player.Position > 0)
        {
            _initialProgress = Player.Position;
        }

        InitializeDashMediaCommand.Execute(_part);
    }

    private void SyncDownloadAndSubtitle()
    {
        var isAISubtitleFiltered = SettingsToolkit.ReadLocalSetting(SettingNames.FilterAISubtitle, true);
        Downloader.HasAvailableSubtitle = Subtitle.IsAvailable && (isAISubtitleFiltered ? Subtitle.Metas.Any(p => !p.IsAI) : true);
    }

    private void InitializeLoops()
    {
        var currentType = CurrentLoop;
        var loops = new List<VideoLoopType> { VideoLoopType.None, VideoLoopType.Single };
        if (Sections.OfType<VideoPlayerPartSectionDetailViewModel>().Any()
           || _playlist is not null
           || Sections.OfType<VideoPlayerSeasonSectionDetailViewModel>().Any())
        {
            loops.Add(VideoLoopType.List);
        }

        if (!loops.Contains(CurrentLoop))
        {
            CurrentLoop = VideoLoopType.None;
        }

        LoopTypes = loops;
        CurrentLoop = VideoLoopType.None;
        CurrentLoop = currentType;
    }
}
