// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Microsoft.UI.Xaml.Data;
using Richasy.BiliKernel.Models;

namespace BiliCopilot.UI.Converters;

internal sealed partial class PgcFavoriteStatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var status = (PgcFavoriteStatus)value;
        return status switch
        {
            PgcFavoriteStatus.Want => ResourceToolkit.GetLocalizedString(StringNames.WantWatch),
            PgcFavoriteStatus.Watching => ResourceToolkit.GetLocalizedString(StringNames.Watching),
            PgcFavoriteStatus.Watched => ResourceToolkit.GetLocalizedString(StringNames.Watched),
            _ => throw new Exception("Unknown status."),
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
