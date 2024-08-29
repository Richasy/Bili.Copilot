// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.BiliKernel.Models.Appearance;

namespace BiliCopilot.UI.Extensions;

internal sealed class MomentDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate VideoTemplate { get; set; }

    public DataTemplate ForwardTemplate { get; set; }

    public DataTemplate EpisodeTemplate { get; set; }

    public DataTemplate ImageTemplate { get; set; }

    public DataTemplate NotSupportTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is MomentItemViewModel)
        {
            return ForwardTemplate;
        }
        else if (item is VideoItemViewModel)
        {
            return VideoTemplate;
        }
        else if (item is EpisodeItemViewModel)
        {
            return EpisodeTemplate;
        }
        else if (item is IEnumerable<BiliImage>)
        {
            return ImageTemplate;
        }

        return NotSupportTemplate;
    }
}
