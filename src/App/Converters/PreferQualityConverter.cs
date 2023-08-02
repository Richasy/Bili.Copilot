// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Player;
using Microsoft.UI.Xaml.Data;

namespace Bili.Copilot.App.Converters;

/// <summary>
/// 偏好画质的可读文本转换器.
/// </summary>
internal sealed class PreferQualityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is PreferQuality quality)
        {
            return quality switch
            {
                PreferQuality.HDFirst => ResourceToolkit.GetLocalizedString(StringNames.HDFirst),
                PreferQuality.HighQuality => ResourceToolkit.GetLocalizedString(StringNames.PreferHighQuality),
                _ => ResourceToolkit.GetLocalizedString(StringNames.Automatic),
            };
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
