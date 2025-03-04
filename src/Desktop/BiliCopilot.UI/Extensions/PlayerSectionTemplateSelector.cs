// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;

namespace BiliCopilot.UI.Extensions;

internal sealed partial class PlayerSectionTemplateSelector : DataTemplateSelector
{
    public DataTemplate CommentTemplate { get; set; }

    public DataTemplate RecommendTemplate { get; set; }

    public DataTemplate UgcSeasonTemplate { get; set; }

    public DataTemplate PartTemplate { get; set; }

    public DataTemplate PlaylistTemplate { get; set; }

    public DataTemplate EpisodeTemplate { get; set; }

    public DataTemplate SeasonTemplate { get; set; }

    public DataTemplate InfoTemplate { get; set; }

    public DataTemplate AITemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is CommentMainViewModel)
        {
            return CommentTemplate;
        }
        else if (item is VideoPlayerRecommendSectionDetailViewModel)
        {
            return RecommendTemplate;
        }
        else if (item is VideoPlayerPartSectionDetailViewModel)
        {
            return PartTemplate;
        }
        else if (item is VideoPlayerSeasonSectionDetailViewModel)
        {
            return UgcSeasonTemplate;
        }
        else if (item is VideoPlayerPlaylistSectionDetailViewModel)
        {
            return PlaylistTemplate;
        }
        else if (item is PgcPlayerEpisodeSectionDetailViewModel)
        {
            return EpisodeTemplate;
        }
        else if (item is PgcPlayerSeasonSectionDetailViewModel)
        {
            return SeasonTemplate;
        }
        else if (item is VideoPlayerInfoSectionDetailViewModel or PgcPlayerInfoSectionDetailViewModel)
        {
            return InfoTemplate;
        }
        else if (item is VideoPlayerAISectionDetailViewModel)
        {
            return AITemplate;
        }

        return default;
    }
}
