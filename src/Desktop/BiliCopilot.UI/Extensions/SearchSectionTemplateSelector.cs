// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.ViewModels.Items;

namespace BiliCopilot.UI.Extensions;

internal sealed class SearchSectionTemplateSelector : DataTemplateSelector
{
    public DataTemplate VideoTemplate { get; set; }

    public DataTemplate PgcTemplate { get; set; }

    public DataTemplate LiveTemplate { get; set; }

    public DataTemplate UserTemplate { get; set; }

    public DataTemplate ArticleTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is ISearchSectionDetailViewModel searchSectionDetailViewModel)
        {
            return searchSectionDetailViewModel.SectionType switch
            {
                SearchSectionType.Video => VideoTemplate,
                SearchSectionType.Anime or SearchSectionType.Cinema => PgcTemplate,
                SearchSectionType.Live => LiveTemplate,
                SearchSectionType.User => UserTemplate,
                SearchSectionType.Article => ArticleTemplate,
                _ => throw new ArgumentOutOfRangeException(nameof(searchSectionDetailViewModel.SectionType), searchSectionDetailViewModel.SectionType, null)
            };
        }

        return default;
    }
}
