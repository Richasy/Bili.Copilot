// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Player;
using Microsoft.UI.Xaml.Data;

namespace Bili.Copilot.App.Converters;

internal sealed class PlayerTypeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var result = string.Empty;
        if (value is PlayerType decode)
        {
            switch (decode)
            {
                case PlayerType.Native:
                    result = ResourceToolkit.GetLocalizedString(StringNames.NativePlayer);
                    break;
                case PlayerType.FFmpeg:
                    result = ResourceToolkit.GetLocalizedString(StringNames.FFmpegPlayer);
                    break;
                default:
                    break;
            }
        }

        return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
