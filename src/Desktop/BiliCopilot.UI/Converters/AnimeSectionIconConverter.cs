// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using Microsoft.UI.Xaml.Data;

namespace BiliCopilot.UI.Converters;

internal sealed class AnimeSectionIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var sectionType = (AnimeSectionType)value;
        return sectionType switch
        {
            AnimeSectionType.Timeline => FluentIcons.Common.Symbol.Timeline,
            AnimeSectionType.Bangumi => FluentIcons.Common.Symbol.Cookies,
            AnimeSectionType.Domestic => FluentIcons.Common.Symbol.FastAcceleration,
            _ => throw new ArgumentOutOfRangeException(nameof(sectionType), sectionType, null)
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
