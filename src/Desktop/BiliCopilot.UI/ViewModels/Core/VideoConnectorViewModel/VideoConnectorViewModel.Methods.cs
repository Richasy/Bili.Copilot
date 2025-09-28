// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using Humanizer;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.ViewModels.Core;

public sealed partial class VideoConnectorViewModel
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
            sections.Add(new VideoPlayerPartSectionDetailViewModel(_view.Parts, _part.Identifier.Id, default));
        }

        if (_snapshot.Playlist is not null)
        {
            sections.Insert(0, new VideoPlayerPlaylistSectionDetailViewModel(this, _snapshot.Playlist, AvId));
        }

        if (_view.Recommends is not null)
        {
            sections.Add(new VideoPlayerRecommendSectionDetailViewModel(this, _view.Recommends));
        }

        sections.Add(_comments);
        sections.Add(new VideoPlayerAISectionDetailViewModel(AI));

        Sections = sections;
        SelectSection(Sections[0]);
        SectionInitialized?.Invoke(this, EventArgs.Empty);
    }

    private void ClearView()
    {
        _view = default;
        _part = default;
        Tags = default;
        UpAvatar = default;
        IsFollow = false;
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
        SelectedSection = default;
        Sections?.Clear();
    }

    private object? FindPrevVideo()
    {
        // 1. 先检查分P列表中是否有下一个视频.
        if (Sections.OfType<VideoPlayerPartSectionDetailViewModel>().FirstOrDefault() is VideoPlayerPartSectionDetailViewModel partSection)
        {
            var index = partSection.Parts.ToList().IndexOf(_part);
            if (index > 0)
            {
                return partSection.Parts[index - 1];
            }
        }

        // 2. 检查播放列表中是否有下一个视频.
        if (_snapshot.Playlist is not null)
        {
            var index = _snapshot.Playlist.Select(p => p.Video).ToList().IndexOf(_snapshot.Video);
            if (index > 0)
            {
                return _snapshot.Playlist[index - 1];
            }

            if (SettingsToolkit.ReadLocalSetting(SettingNames.EndWithPlaylist, true))
            {
                return default;
            }
        }

        // 3. 检查合集中是否有上一个视频.
        if (Sections.OfType<VideoPlayerSeasonSectionDetailViewModel>().FirstOrDefault() is VideoPlayerSeasonSectionDetailViewModel seasonSection)
        {
            var selectedItem = seasonSection.Items.Find(p => p.IsSelected);
            var index = seasonSection.Items.ToList().IndexOf(selectedItem);
            if (index > 0)
            {
                return seasonSection.Items[index - 1].Data;
            }
        }

        return default;
    }

    private object? FindNextVideo()
    {
        // 1. 先检查分P列表中是否有下一个视频.
        if (Sections.OfType<VideoPlayerPartSectionDetailViewModel>().FirstOrDefault() is VideoPlayerPartSectionDetailViewModel partSection)
        {
            var index = partSection.Parts.ToList().IndexOf(_part);
            if (index < partSection.Parts.Count - 1)
            {
                return partSection.Parts[index + 1];
            }
        }

        // 2. 检查播放列表中是否有下一个视频.
        if (_snapshot.Playlist is not null)
        {
            var index = _snapshot.Playlist.ConvertAll(p => p.Video).IndexOf(_snapshot.Video);
            if (index < _snapshot.Playlist.Count - 1)
            {
                return _snapshot.Playlist[index + 1];
            }

            if (SettingsToolkit.ReadLocalSetting(SettingNames.EndWithPlaylist, true))
            {
                return default;
            }
        }

        // 3. 检查合集中是否有下一个视频.
        if (Sections.OfType<VideoPlayerSeasonSectionDetailViewModel>().FirstOrDefault() is VideoPlayerSeasonSectionDetailViewModel seasonSection)
        {
            var selectedItem = seasonSection.Items.Find(p => p.IsSelected);
            var index = seasonSection.Items.ToList().IndexOf(selectedItem);
            if (index < seasonSection.Items.Count - 1)
            {
                return seasonSection.Items[index + 1].Data;
            }
        }

        // 4. 检查推荐视频中是否有下一个视频.
        var isAutoPlayRecommendVideo = SettingsToolkit.ReadLocalSetting(SettingNames.AutoPlayNextRecommendVideo, false);
        if (isAutoPlayRecommendVideo && Sections.OfType<VideoPlayerRecommendSectionDetailViewModel>().FirstOrDefault() is VideoPlayerRecommendSectionDetailViewModel recommendSection)
        {
            return recommendSection.Items[0].Data;
        }

        return default;
    }

    private string GetWebLink()
        => $"https://www.bilibili.com/video/av{_view.Information.Identifier.Id}";
}
