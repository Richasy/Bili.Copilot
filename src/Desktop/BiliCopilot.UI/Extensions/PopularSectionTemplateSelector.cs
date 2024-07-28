// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;

namespace BiliCopilot.UI.Extensions;

internal sealed class PopularSectionTemplateSelector : DataTemplateSelector
{
    public DataTemplate SectionTemplate { get; set; }

    public DataTemplate PartitionTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is PopularSectionItemViewModel)
        {
            return SectionTemplate;
        }
        else if (item is PopularRankPartitionViewModel)
        {
            return PartitionTemplate;
        }

        return default;
    }
}
