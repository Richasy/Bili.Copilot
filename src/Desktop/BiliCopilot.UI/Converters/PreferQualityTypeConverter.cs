// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Microsoft.UI.Xaml.Data;

namespace BiliCopilot.UI.Converters;

/// <summary>
/// 偏好画质的可读文本转换器.
/// </summary>
internal sealed partial class PreferQualityTypeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is PreferQualityType quality
            ? quality switch
            {
                PreferQualityType.HD => ResourceToolkit.GetLocalizedString(StringNames.HDFirst),
                PreferQualityType.High => ResourceToolkit.GetLocalizedString(StringNames.PreferHighQuality),
                PreferQualityType.UHD => ResourceToolkit.GetLocalizedString(StringNames.UHDFirst),
                PreferQualityType.HDReady => ResourceToolkit.GetLocalizedString(StringNames.PreferHDReady),
                PreferQualityType.SD => ResourceToolkit.GetLocalizedString(StringNames.PreferSD),
                PreferQualityType.Smooth => ResourceToolkit.GetLocalizedString(StringNames.PreferSmooth),
                _ => ResourceToolkit.GetLocalizedString(StringNames.Automatic),
            }
            : string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
