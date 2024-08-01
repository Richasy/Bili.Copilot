// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Microsoft.UI.Xaml.Data;

namespace BiliCopilot.UI.Converters;

internal sealed class AnimeSectionTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var sectionType = (AnimeSectionType)value;
        var lanName = sectionType switch
        {
            AnimeSectionType.Timeline => StringNames.TimeChart,
            AnimeSectionType.Bangumi => StringNames.Bangumi,
            AnimeSectionType.Domestic => StringNames.DomesticAnime,
            _ => throw new ArgumentOutOfRangeException(nameof(sectionType), sectionType, null)
        };

        return ResourceToolkit.GetLocalizedString(lanName);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
