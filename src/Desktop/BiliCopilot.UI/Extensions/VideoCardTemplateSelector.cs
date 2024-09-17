// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.ViewModels.Items;

namespace BiliCopilot.UI.Extensions;

/// <summary>
/// 视频卡片模板选择器.
/// </summary>
internal sealed partial class VideoCardTemplateSelector : DataTemplateSelector
{
    public DataTemplate RecommendTemplate { get; set; }

    public DataTemplate HotTemplate { get; set; }

    public DataTemplate RankTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is VideoItemViewModel vm)
        {
            return vm.Style switch
            {
                VideoCardStyle.Recommend => RecommendTemplate,
                VideoCardStyle.Hot => HotTemplate,
                VideoCardStyle.Rank => RankTemplate,
                _ => default
            };
        }

        return default;
    }
}
