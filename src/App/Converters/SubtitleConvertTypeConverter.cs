// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;

namespace Bili.Copilot.App.Converters;

internal sealed class SubtitleConvertTypeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var result = string.Empty;
        if (value is SubtitleConvertType type)
        {
            switch (type)
            {
                case SubtitleConvertType.None:
                    result = ResourceToolkit.GetLocalizedString(StringNames.NoConvert);
                    break;
                case SubtitleConvertType.ToSimplifiedChinese:
                    result = ResourceToolkit.GetLocalizedString(StringNames.TC2SC);
                    break;
                case SubtitleConvertType.ToTraditionalChinese:
                    result = ResourceToolkit.GetLocalizedString(StringNames.SC2TC);
                    break;
                default:
                    break;
            }
        }

        return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
