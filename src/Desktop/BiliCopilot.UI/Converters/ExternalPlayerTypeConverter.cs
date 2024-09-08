// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using Microsoft.UI.Xaml.Data;

namespace BiliCopilot.UI.Converters;

internal sealed class ExternalPlayerTypeConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var result = string.Empty;
        if (value is ExternalPlayerType type)
        {
            switch (type)
            {
                case ExternalPlayerType.Mpv:
                    result = "MPV";
                    break;
                case ExternalPlayerType.MpvNet:
                    result = "MPV.NET";
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
