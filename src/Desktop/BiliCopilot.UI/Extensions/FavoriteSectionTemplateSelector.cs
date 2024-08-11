// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;

namespace BiliCopilot.UI.Extensions;

internal sealed class FavoriteSectionTemplateSelector : DataTemplateSelector
{
    public DataTemplate PgcTemplate { get; set; }

    public DataTemplate VideoTempalte { get; set; }

    public DataTemplate CollectTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is PgcFavoriteSectionDetailViewModel)
        {
            return PgcTemplate;
        }

        return default;
    }
}
