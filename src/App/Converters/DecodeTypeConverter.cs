// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;

namespace Bili.Copilot.App.Converters;

internal sealed class DecodeTypeConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var result = string.Empty;
        if (value is DecodeType decode)
        {
            switch (decode)
            {
                case DecodeType.HardwareDecode:
                    result = ResourceToolkit.GetLocalizedString(StringNames.HardwareDecode);
                    break;
                case DecodeType.SoftwareDecode:
                    result = ResourceToolkit.GetLocalizedString(StringNames.SoftwareDecode);
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
