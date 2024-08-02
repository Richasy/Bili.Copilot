// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;

namespace BiliCopilot.UI.Extensions;

internal sealed class AnimePageControlTemplateSelector : DataTemplateSelector
{
    public DataTemplate TimelineTemplate { get; set; }

    public DataTemplate BangumiTemplate { get; set; }

    public DataTemplate DomesticTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is AnimeTimelineViewModel)
        {
            return TimelineTemplate;
        }
        else if (item is IAnimeSectionDetailViewModel vm)
        {
            return vm.SectionType == Models.Constants.AnimeSectionType.Bangumi ? BangumiTemplate : DomesticTemplate;
        }

        return base.SelectTemplateCore(item, container);
    }
}
