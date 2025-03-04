// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using Microsoft.UI.Xaml.Data;

namespace BiliCopilot.UI.Converters;

internal sealed partial class SearchSectionIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var sectionType = (SearchSectionType)value;
        return sectionType switch
        {
            SearchSectionType.Video => FluentIcons.Common.Symbol.VideoClip,
            SearchSectionType.Anime => FluentIcons.Common.Symbol.Dust,
            SearchSectionType.Live => FluentIcons.Common.Symbol.Live,
            SearchSectionType.User => FluentIcons.Common.Symbol.People,
            SearchSectionType.Cinema => FluentIcons.Common.Symbol.MoviesAndTv,
            SearchSectionType.Article => FluentIcons.Common.Symbol.DocumentBulletList,
            _ => throw new ArgumentOutOfRangeException(nameof(sectionType), sectionType, null)
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
