// Copyright (c) Bili Copilot. All rights reserved.

using System.Linq;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// PGC 播放页面视图模型.
/// </summary>
public sealed partial class PgcPlayerPageViewModel
{
    private void InitializeOverview()
    {
        var actors = View.Information.Celebrities;
        if (actors != null)
        {
            foreach (var item in actors)
            {
                var vm = new UserItemViewModel(item);
                Celebrities.Add(vm);
            }
        }

        IsShowCelebrities = Celebrities.Count > 0;
    }

    private void InitializeOperation()
    {
        IsTracking = View.Information.IsTracking;
        IsCoinWithLiked = true;
        ReloadCommunityInformationCommand.ExecuteAsync(null);
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

        if (communityInfo.Score > 0)
        {
            RatingCountText = NumberToolkit.GetCountText(View.Information.RatingCount) + ResourceToolkit.GetLocalizedString(StringNames.PeopleCount);
        }
    }

    private void InitializeInterop()
        => IsOnlyShowIndex = SettingsToolkit.ReadLocalSetting(SettingNames.IsOnlyShowIndex, false);

    private void InitializeSections()
    {
        // 处理顶部标签.
        var hasEpisodes = View.Episodes != null && View.Episodes.Any();
        var hasSeason = View.Seasons != null && View.Seasons.Any();
        var hasExtras = View.Extras != null && View.Extras.Count > 0;
        var isShowExtraSection = false;

        if (hasEpisodes)
        {
            CurrentEpisode = View.Episodes.FirstOrDefault(p => p.Identifier.Id == _presetEpisodeId);
        }

        if (CurrentEpisode == null && hasExtras)
        {
            CurrentEpisode = View.Extras.SelectMany(p => p.Value).FirstOrDefault(p => p.Identifier.Id == _presetEpisodeId);
            isShowExtraSection = CurrentEpisode != null;
        }

        if (CurrentEpisode == null)
        {
            if (hasEpisodes)
            {
                CurrentEpisode = View.Episodes.First();
            }
            else if (hasExtras)
            {
                CurrentEpisode = View.Extras.First().Value.FirstOrDefault();
            }
        }

        if (hasEpisodes)
        {
            // 只有分集数大于1时才提供切换功能.
            if (View.Episodes.Count() > 1)
            {
                Sections.Add(new PlayerSectionHeader(PlayerSectionType.Episodes, ResourceToolkit.GetLocalizedString(StringNames.Episodes)));
            }

            var subVideos = View.Episodes.ToList();

            for (var i = 0; i < subVideos.Count; i++)
            {
                var item = subVideos[i];
                var vm = new EpisodeItemViewModel(item)
                {
                    IsSelected = item.Equals(CurrentEpisode),
                };
                Episodes.Add(vm);
            }
        }

        if (hasSeason)
        {
            Sections.Add(new PlayerSectionHeader(PlayerSectionType.Seasons, ResourceToolkit.GetLocalizedString(StringNames.Seasons)));
            var seasons = View.Seasons.ToList();
            for (var i = 0; i < seasons.Count; i++)
            {
                var item = seasons[i];
                var vm = new VideoIdentifierSelectableViewModel(item)
                {
                    Index = i + 1,
                    IsSelected = item.Id == View.Information.Identifier.Id,
                };
                Seasons.Add(vm);
            }
        }

        if (hasExtras)
        {
            Sections.Add(new PlayerSectionHeader(PlayerSectionType.Extras, ResourceToolkit.GetLocalizedString(StringNames.Sections)));
            var currentId = CurrentEpisode == null ? string.Empty : CurrentEpisode.Identifier.Id;
            foreach (var item in View.Extras)
            {
                var vm = new PgcExtraItemViewModel(item.Key, item.Value, currentId);
                Extras.Add(vm);
            }
        }

        if (CurrentEpisode != null)
        {
            Sections.Add(new PlayerSectionHeader(PlayerSectionType.Comments, ResourceToolkit.GetLocalizedString(StringNames.Reply)));

            // CommentPageViewModel.SetData(CurrentEpisode.VideoId, CommentType.Video);
        }

        CreatePlayNextAction();
        IsSectionsEmpty = Sections.Count == 0;
        CurrentSection = isShowExtraSection
            ? Sections.FirstOrDefault(p => p.Type == PlayerSectionType.Extras)
            : Sections.FirstOrDefault();
    }
}
