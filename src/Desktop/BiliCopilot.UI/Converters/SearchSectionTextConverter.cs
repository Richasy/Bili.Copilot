// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Microsoft.UI.Xaml.Data;

namespace BiliCopilot.UI.Converters;

internal sealed class SearchSectionTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var sectionType = (SearchSectionType)value;
        var lanName = sectionType switch
        {
            SearchSectionType.Video => StringNames.Video,
            SearchSectionType.Anime => StringNames.Anime,
            SearchSectionType.Live => StringNames.Live,
            SearchSectionType.Cinema => StringNames.Cinema,
            SearchSectionType.User => StringNames.User,
            SearchSectionType.Article => StringNames.Article,
            _ => throw new ArgumentOutOfRangeException(nameof(sectionType), sectionType, null)
        };

        return ResourceToolkit.GetLocalizedString(lanName);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
