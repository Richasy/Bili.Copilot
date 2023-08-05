// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Player;
using Microsoft.UI.Xaml.Data;

namespace Bili.Copilot.App.Converters;

/// <summary>
/// 音质偏好转换器.
/// </summary>
internal sealed class PreferAudioQualityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is PreferAudio quality
            ? quality switch
            {
                PreferAudio.HighQuality => ResourceToolkit.GetLocalizedString(StringNames.HighQuality),
                PreferAudio.Near => ResourceToolkit.GetLocalizedString(StringNames.NearVideo),
                _ => ResourceToolkit.GetLocalizedString(StringNames.Standard),
            }
            : (object)string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
