// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml.Data;

namespace BiliCopilot.UI.Converters;

internal sealed partial class SearchSuggestRegionIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var id = value as string;
        return id switch
        {
            "space" => FluentIcons.Common.Symbol.Person,
            "history" => FluentIcons.Common.Symbol.History,
            _ => FluentIcons.Common.Symbol.Search,
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
