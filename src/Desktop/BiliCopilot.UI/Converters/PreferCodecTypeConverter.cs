// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Microsoft.UI.Xaml.Data;

namespace BiliCopilot.UI.Converters;

/// <summary>
/// 偏好解码模式到可读文本转换器.
/// </summary>
internal sealed partial class PreferCodecTypeConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var result = string.Empty;
        if (value is PreferCodecType codec)
        {
            switch (codec)
            {
                case PreferCodecType.H265:
                    result = ResourceToolkit.GetLocalizedString(StringNames.H265);
                    break;
                case PreferCodecType.H264:
                    result = ResourceToolkit.GetLocalizedString(StringNames.H264);
                    break;
                case PreferCodecType.Av1:
                    result = ResourceToolkit.GetLocalizedString(StringNames.Av1);
                    break;
            }
        }

        return result;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
