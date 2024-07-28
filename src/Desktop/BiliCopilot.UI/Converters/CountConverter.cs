// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Microsoft.UI.Xaml.Data;

namespace BiliCopilot.UI.Converters;

internal sealed class CountConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var count = System.Convert.ToInt32(value);
        if (count < 0)
        {
            return string.Empty;
        }

        if (count >= 100_000_000)
        {
            var unit = ResourceToolkit.GetLocalizedString(StringNames.Billion);
            return Math.Round(count / 100_000_000d, 2) + unit;
        }
        else if (count >= 10_000)
        {
            var unit = ResourceToolkit.GetLocalizedString(StringNames.TenThousands);
            return Math.Round(count / 10_000d, 2) + unit;
        }

        return count.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
