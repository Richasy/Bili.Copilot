// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Microsoft.UI.Xaml.Data;

namespace Bili.Copilot.App.Converters;

/// <summary>
/// 视频排序方式可读文本转换器.
/// </summary>
public class SortTypeTextConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var result = string.Empty;
        if (value is VideoSortType vst)
        {
            switch (vst)
            {
                case VideoSortType.Default:
                    result = ResourceToolkit.GetLocalizedString(StringNames.SortByDefault);
                    break;
                case VideoSortType.Newest:
                    result = ResourceToolkit.GetLocalizedString(StringNames.SortByNewest);
                    break;
                case VideoSortType.Play:
                    result = ResourceToolkit.GetLocalizedString(StringNames.SortByPlay);
                    break;
                case VideoSortType.Reply:
                    result = ResourceToolkit.GetLocalizedString(StringNames.SortByReply);
                    break;
                case VideoSortType.Danmaku:
                    result = ResourceToolkit.GetLocalizedString(StringNames.SortByDanmaku);
                    break;
                case VideoSortType.Favorite:
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
