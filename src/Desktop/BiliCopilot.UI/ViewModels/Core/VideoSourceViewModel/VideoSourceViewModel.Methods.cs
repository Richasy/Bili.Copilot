// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using Humanizer;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.ViewModels.Core;

public sealed partial class VideoSourceViewModel
{
    private void InitializeView(VideoPlayerView view)
    {
        if (view is null)
        {
            return;
        }

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
        Tags = [.. view.Tags];
        InitializeCommunityInformation(view.Information.CommunityInformation);
        IsLiked = view.Operation.IsLiked;
        IsCoined = view.Operation.IsCoined;
        IsFavorited = view.Operation.IsFavorited;
        IsCoinAlsoLike = true;
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

        Downloader.Clear();
        Formats = [.. info.Formats.Select(p => new PlayerFormatItemViewModel(p))];

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

        if (selectedFormat is null)
        {
            var maxQuality = availableFormats.Max(p => p.Data.Quality);
            selectedFormat = availableFormats.Find(p => p.Data.Quality == maxQuality);
        }

        ChangeFormat(selectedFormat);
    }

    private void InitializeSections()
    {
        if (_isSeasonInitialized)
        {
            return;
        }

        SeasonSection = _view.Seasons is not null ? new VideoPlayerSeasonSectionDetailViewModel(this, _view.Seasons, AvId) : default;
        PartSection = _view.Parts?.Count > 1 ? new VideoPlayerPartSectionDetailViewModel(_view.Parts, _part.Identifier.Id, ChangePartCommand.Execute) : default;
        PlaylistSection = _playlist is not null ? new VideoPlayerPlaylistSectionDetailViewModel(this, _playlist, AvId) : default;
        RecommendSection = _view.Recommends is not null ? new VideoPlayerRecommendSectionDetailViewModel(_view.Recommends) : default;

        _isSeasonInitialized = true;
        SectionInitialized?.Invoke(this, EventArgs.Empty);
    }

    private void ClearView()
    {
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
    }

    private VideoPart? FindInitialPart(string? initialPartId = default)
    {
        if (_view?.Parts?.Count == 1)
        {
            return _view.Parts[0];
        }

        VideoPart? part = default;
        if (!string.IsNullOrEmpty(initialPartId))
        {
            part = _view!.Parts.FirstOrDefault(p => p.Identifier.Id == initialPartId);
        }

        if (part == null)
        {
            var historyPartId = _view!.Progress?.Cid;
            var autoLoadHistory = SettingsToolkit.ReadLocalSetting(SettingNames.AutoLoadHistory, true);
            if (!string.IsNullOrEmpty(historyPartId) && autoLoadHistory)
            {
                part = _view.Parts.FirstOrDefault(p => p.Identifier.Id == historyPartId);
            }
        }

        return part ?? _view!.Parts.FirstOrDefault();
    }

    private void LoadInitialProgress(int? progress = default)
    {
        _initialProgress = 0;
        if (progress is not null)
        {
            _initialProgress = progress.Value;
        }
        else if (_view?.Progress is not null)
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

        // 1. 先检查分P列表中是否有下一个视频.
        if (PartSection != null)
        {
            var index = PartSection.Parts.ToList().IndexOf(_part);
            if (index < PartSection.Parts.Count - 1)
            {
                return PartSection.Parts[index + 1];
            }

            if (_playlist is null && isListLoop && SeasonSection is null)
            {
                return PartSection.Parts[0];
            }
        }

        // 2. 检查播放列表中是否有下一个视频.
        if (_playlist is not null)
        {
            var index = _playlist.ToList().IndexOf(_view.Information);
            if (index < _playlist.Count - 1)
            {
                return _playlist[index + 1];
            }

            if (isListLoop)
            {
                return _playlist[0];
            }
            else if (SettingsToolkit.ReadLocalSetting(SettingNames.EndWithPlaylist, true))
            {
                return default;
            }
        }

        // 3. 检查合集中是否有下一个视频.
        if (SeasonSection != null)
        {
            var index = SeasonSection.Items.ToList().IndexOf(SeasonSection.SelectedItem);
            if (index < SeasonSection.Items.Count - 1)
            {
                return SeasonSection.Items[index + 1].Data;
            }
            else if (isListLoop)
            {
                return SeasonSection.Items[0].Data;
            }
        }

        // 4. 检查推荐视频中是否有下一个视频.
        var isAutoPlayRecommendVideo = SettingsToolkit.ReadLocalSetting(SettingNames.AutoPlayNextRecommendVideo, false);
        if (isAutoPlayRecommendVideo && RecommendSection != null)
        {
            return RecommendSection.Items[0].Data;
        }

        return default;
    }

    private void InitializeNextVideo()
    {
        if (_view is null || !_isSeasonInitialized)
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
        if (PartSection is not null
           || _playlist is not null
           || SeasonSection is not null)
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

    private string GetWebLink()
        => $"https://www.bilibili.com/video/av{_view.Information.Identifier.Id}";
}
