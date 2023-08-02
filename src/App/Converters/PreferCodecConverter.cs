// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Microsoft.UI.Xaml.Data;

namespace Bili.Copilot.App.Converters;

/// <summary>
/// 偏好解码模式到可读文本转换器.
/// </summary>
public class PreferCodecConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var result = string.Empty;
        if (value is PreferCodec codec)
        {
            switch (codec)
            {
                case PreferCodec.H265:
                    result = ResourceToolkit.GetLocalizedString(StringNames.H265);
                    break;
                case PreferCodec.H264:
                    result = ResourceToolkit.GetLocalizedString(StringNames.H264);
                    break;
                case PreferCodec.Av1:
                    result = ResourceToolkit.GetLocalizedString(StringNames.Av1);
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
