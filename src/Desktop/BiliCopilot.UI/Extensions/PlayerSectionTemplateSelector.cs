// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;

namespace BiliCopilot.UI.Extensions;

internal sealed class PlayerSectionTemplateSelector : DataTemplateSelector
{
    public DataTemplate CommentTemplate { get; set; }

    public DataTemplate RecommendTemplate { get; set; }

    public DataTemplate UgcSeasonTemplate { get; set; }

    public DataTemplate PartTemplate { get; set; }

    public DataTemplate PlaylistTemplate { get; set; }

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

        return default;
    }
}
