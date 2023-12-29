// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Player;

namespace Bili.Copilot.App.Converters;

/// <summary>
/// 偏好画质的可读文本转换器.
/// </summary>
internal sealed class PreferQualityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is PreferQuality quality
            ? quality switch
            {
                PreferQuality.HDFirst => ResourceToolkit.GetLocalizedString(StringNames.HDFirst),
                PreferQuality.HighQuality => ResourceToolkit.GetLocalizedString(StringNames.PreferHighQuality),
                _ => ResourceToolkit.GetLocalizedString(StringNames.Automatic),
            }
            : (object)string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
