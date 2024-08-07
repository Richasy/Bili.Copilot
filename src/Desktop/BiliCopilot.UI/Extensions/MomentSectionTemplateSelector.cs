// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;

namespace BiliCopilot.UI.Extensions;

internal sealed class MomentSectionTemplateSelector : DataTemplateSelector
{
    public DataTemplate VideoTemplate { get; set; }

    public DataTemplate ComprehensiveTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is not IMomentSectionDetailViewModel)
        {
            return default;
        }

        var vm = (IMomentSectionDetailViewModel)item;
        return vm.SectionType == Models.Constants.MomentSectionType.Video ? VideoTemplate : ComprehensiveTemplate;
    }
}
