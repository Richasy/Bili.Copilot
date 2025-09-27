// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml.Data;

namespace BiliCopilot.UI.Converters;

internal sealed partial class PlayerPositionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double position)
        {
            // Convert position from seconds to a TimeSpan format (hh:mm:ss)
            var timeSpan = TimeSpan.FromSeconds(position);
            return $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }

        return "00:00:00"; // Default value if conversion fails or value is not a double
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is string timeString)
        {
            // Parse the string back to a TimeSpan
            if (TimeSpan.TryParse(timeString, out var timeSpan))
            {
                return timeSpan.TotalSeconds; // Return total seconds as double
            }
        }

        return 0.0; // Default value if conversion fails or value is not a valid time string
    }
}
