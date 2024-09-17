// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Microsoft.UI.Xaml.Data;

namespace BiliCopilot.UI.Converters;

/// <summary>
/// 循环类型转换器.
/// </summary>
internal sealed partial class LoopTypeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var key = (VideoLoopType)value switch
        {
            VideoLoopType.Single => StringNames.SingleLoop,
            VideoLoopType.List => StringNames.ListLoop,
            _ => StringNames.NoLoop,
        };

        return ResourceToolkit.GetLocalizedString(key);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
