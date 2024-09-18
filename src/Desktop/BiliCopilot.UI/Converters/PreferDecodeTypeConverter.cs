// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Microsoft.UI.Xaml.Data;

namespace BiliCopilot.UI.Converters;

internal sealed partial class PreferDecodeTypeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var decode = (PreferDecodeType)value;
        return decode switch
        {
            PreferDecodeType.Software => ResourceToolkit.GetLocalizedString(StringNames.SoftwareDecode),
            PreferDecodeType.D3D11 => ResourceToolkit.GetLocalizedString(StringNames.D3D11Decode),
            PreferDecodeType.NVDEC => ResourceToolkit.GetLocalizedString(StringNames.NVDECDecode),
            PreferDecodeType.DXVA2 => ResourceToolkit.GetLocalizedString(StringNames.DXVA2Decode),
            _ => string.Empty,
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
