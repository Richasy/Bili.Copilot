// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml.Data;

namespace BiliCopilot.UI.Converters;

internal sealed class PgcFavoriteIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var type = (Richasy.BiliKernel.Models.PgcFavoriteType)value;
        return type switch
        {
            Richasy.BiliKernel.Models.PgcFavoriteType.Anime => FluentIcons.Common.Symbol.Dust,
            Richasy.BiliKernel.Models.PgcFavoriteType.Cinema => FluentIcons.Common.Symbol.MoviesAndTv,
            _ => throw new Exception("Unknown type."),
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
