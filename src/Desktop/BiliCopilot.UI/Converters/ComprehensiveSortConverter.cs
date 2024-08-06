// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Microsoft.UI.Xaml.Data;
using Richasy.BiliKernel.Models;

namespace BiliCopilot.UI.Converters;

/// <summary>
/// 综合排序转换器.
/// </summary>
internal sealed class ComprehensiveSortConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var sort = (ComprehensiveSearchSortType)value;
        return sort switch
        {
            ComprehensiveSearchSortType.Default => ResourceToolkit.GetLocalizedString(StringNames.SortByDefault),
            ComprehensiveSearchSortType.Danmaku => ResourceToolkit.GetLocalizedString(StringNames.SortByDanmaku),
            ComprehensiveSearchSortType.Play => ResourceToolkit.GetLocalizedString(StringNames.SortByPlay),
            ComprehensiveSearchSortType.Newest => ResourceToolkit.GetLocalizedString(StringNames.SortByNewest),
            _ => string.Empty,
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
