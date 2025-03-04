// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;

namespace BiliCopilot.UI.Extensions;

internal sealed partial class AnimePageControlTemplateSelector : DataTemplateSelector
{
    public DataTemplate TimelineTemplate { get; set; }

    public DataTemplate IndexTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is IPgcSectionDetailViewModel vm)
        {
            return vm.SectionType == Models.Constants.PgcSectionType.Timeline ? TimelineTemplate : IndexTemplate;
        }

        return base.SelectTemplateCore(item, container);
    }
}
