// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.BiliKernel.Models;

namespace BiliCopilot.UI.Extensions;

internal sealed class HistorySectionTemplateSelector : DataTemplateSelector
{
    public DataTemplate VideoTemplate { get; set; }

    public DataTemplate LiveTemplate { get; set; }

    public DataTemplate ArticleTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is IHistorySectionDetailViewModel vm)
        {
            return vm.Type switch
            {
                ViewHistoryTabType.Video => VideoTemplate,
                ViewHistoryTabType.Live => LiveTemplate,
                ViewHistoryTabType.Article => ArticleTemplate,
                _ => throw new ArgumentOutOfRangeException(nameof(vm.Type), vm.Type, null)
            };
        }

        return default;
    }
}
