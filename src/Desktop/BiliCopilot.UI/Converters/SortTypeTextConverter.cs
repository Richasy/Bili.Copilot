// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Microsoft.UI.Xaml.Data;
using Richasy.BiliKernel.Models;

namespace BiliCopilot.UI.Converters;

/// <summary>
/// 视频排序方式可读文本转换器.
/// </summary>
public class SortTypeTextConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var result = string.Empty;
        if (value is PartitionVideoSortType vst)
        {
            switch (vst)
            {
                case PartitionVideoSortType.Default:
                    result = ResourceToolkit.GetLocalizedString(StringNames.SortByDefault);
                    break;
                case PartitionVideoSortType.Newest:
                    result = ResourceToolkit.GetLocalizedString(StringNames.SortByNewest);
                    break;
                case PartitionVideoSortType.Play:
                    result = ResourceToolkit.GetLocalizedString(StringNames.SortByPlay);
                    break;
                case PartitionVideoSortType.Reply:
                    result = ResourceToolkit.GetLocalizedString(StringNames.SortByReply);
                    break;
                case PartitionVideoSortType.Danmaku:
                    result = ResourceToolkit.GetLocalizedString(StringNames.SortByDanmaku);
                    break;
                case PartitionVideoSortType.Favorite:
                    result = ResourceToolkit.GetLocalizedString(StringNames.SortByFavorite);
                    break;
                default:
                    break;
            }
        }
        else if (value is ArticleSortType ast)
        {
            switch (ast)
            {
                case ArticleSortType.Default:
                    result = ResourceToolkit.GetLocalizedString(StringNames.SortByDefault);
                    break;
                case ArticleSortType.Newest:
                    result = ResourceToolkit.GetLocalizedString(StringNames.SortByNewest);
                    break;
                case ArticleSortType.Read:
                    result = ResourceToolkit.GetLocalizedString(StringNames.SortByRead);
                    break;
                case ArticleSortType.Reply:
                    result = ResourceToolkit.GetLocalizedString(StringNames.SortByReply);
                    break;
                case ArticleSortType.Like:
                    result = ResourceToolkit.GetLocalizedString(StringNames.SortByLike);
                    break;
                case ArticleSortType.Favorite:
                    result = ResourceToolkit.GetLocalizedString(StringNames.SortByFavorite);
                    break;
                default:
                    break;
            }
        }

        return result;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
