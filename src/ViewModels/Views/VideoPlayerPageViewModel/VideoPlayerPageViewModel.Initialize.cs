// Copyright (c) Bili Copilot. All rights reserved.

using System.Linq;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Humanizer;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 视频播放页面视图模型.
/// </summary>
public sealed partial class VideoPlayerPageViewModel
{
    private void InitializePublisher()
    {
        IsCooperationVideo = View.Information.Collaborators != null;
        if (IsCooperationVideo)
        {
            foreach (var profile in View.Information.Collaborators)
            {
                var userVM = new UserItemViewModel(profile);
                Collaborators.Add(userVM);
            }
        }
        else
        {
            var myId = AuthorizeProvider.Instance.CurrentUserId;
            var userVM = new UserItemViewModel(View.Information.Publisher)
            {
                Relation = View.PublisherCommunityInformation?.Relation ?? Models.Constants.Community.UserRelationStatus.Unknown,
                IsRelationButtonShown = !string.IsNullOrEmpty(myId) && myId != View.Information.Publisher.User.Id,
            };
            Author = userVM;
        }
    }

    private void InitializeOverview()
    {
        PublishTime = View.Information.PublishTime.Humanize();
        WatchingCountText = "--";

        View.Tags?.ToList().ForEach(Tags.Add);
        IsShowTags = Tags.Count > 0;
    }

    private void InitializeOperation()
    {
        if (View.Operation == null)
        {
            return;
        }

        IsLiked = View.Operation.IsLiked;
        IsCoined = View.Operation.IsCoined;
        IsFavorited = View.Operation.IsFavorited;
        IsCoinWithLiked = true;
    }

    private void InitializeCommunityInformation()
    {
        var communityInfo = View.Information.CommunityInformation;
        PlayCountText = NumberToolkit.GetCountText(communityInfo.PlayCount);
        DanmakuCountText = NumberToolkit.GetCountText(communityInfo.DanmakuCount);
        CommentCountText = NumberToolkit.GetCountText(communityInfo.CommentCount);
        LikeCountText = NumberToolkit.GetCountText(communityInfo.LikeCount);
        CoinCountText = NumberToolkit.GetCountText(communityInfo.CoinCount);
        FavoriteCountText = NumberToolkit.GetCountText(communityInfo.FavoriteCount);
    }

    private void InitializeInterop()
    {
        var downloadParam = string.IsNullOrEmpty(View.Information.AlternateId)
            ? $"av{View.Information.Identifier.Id}"
            : View.Information.AlternateId;
        var downloadParts = VideoParts.Select((_, index) => index + 1).ToList();
        IsOnlyShowIndex = SettingsToolkit.ReadLocalSetting(SettingNames.IsOnlyShowIndex, false);
    }

    private void InitializeSections()
    {
        // 处理顶部标签.
        var hasVideoParts = View.SubVideos != null && View.SubVideos.Count() > 0;
        var hasSeason = View.Seasons != null && View.Seasons.Count() > 0;
        var hasRelatedVideos = View.RelatedVideos != null && View.RelatedVideos.Count() > 0;
        var hasPlaylist = VideoPlaylist.Count > 0;

        Sections.Add(new PlayerSectionHeader(PlayerSectionType.VideoInformation, ResourceToolkit.GetLocalizedString(StringNames.Information)));

        if (hasVideoParts)
        {
            // 只有分P数大于1时才提供切换功能.
            if (View.SubVideos.Count() > 1 && View.InteractionVideo == null)
            {
                Sections.Add(new PlayerSectionHeader(PlayerSectionType.VideoParts, ResourceToolkit.GetLocalizedString(StringNames.Parts)));
            }

            var subVideos = View.SubVideos.ToList();
            CurrentVideoPart = subVideos.First();
            for (var i = 0; i < subVideos.Count; i++)
            {
                var item = subVideos[i];
                var vm = new VideoIdentifierSelectableViewModel(item)
                {
                    Index = i + 1,
                    IsSelected = item.Equals(CurrentVideoPart),
                };
                VideoParts.Add(vm);
            }
        }

        if (hasPlaylist)
        {
            Sections.Add(new PlayerSectionHeader(PlayerSectionType.Playlist, ResourceToolkit.GetLocalizedString(StringNames.Playlist)));
            foreach (var item in VideoPlaylist)
            {
                item.IsSelected = item.Data.Equals(View.Information);
            }
        }

        if (hasSeason)
        {
            View.Seasons.ToList().ForEach(Seasons.Add);
            var season = Seasons.FirstOrDefault(p => p.Videos != null && p.Videos.Any(j => j.Equals(View.Information)));
            if (season != null)
            {
                // 只有确定当前合集包含正在播放的视频时才显示合集标头
                Sections.Add(new PlayerSectionHeader(PlayerSectionType.UgcSeason, ResourceToolkit.GetLocalizedString(StringNames.UgcEpisode)));
                SelectSeason(season);
            }
        }

        if (hasRelatedVideos)
        {
            Sections.Add(new PlayerSectionHeader(PlayerSectionType.RelatedVideos, ResourceToolkit.GetLocalizedString(StringNames.RelatedVideos)));
            View.RelatedVideos.ToList().ForEach(p => RelatedVideos.Add(GetItemViewModel(p)));
        }

        CreatePlayNextAction();

        // 评论区常显，但位于最后一个.
        Sections.Add(new PlayerSectionHeader(PlayerSectionType.Comments, ResourceToolkit.GetLocalizedString(StringNames.Reply)));

        Comments.SetData(View.Information.Identifier.Id, CommentType.Video);
        CurrentSection = Sections.First();
        _ = RequestOnlineCountCommand.ExecuteAsync(null);
    }
}
