// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml.Data;
using Windows.UI;

namespace BiliCopilot.UI.Converters;

internal sealed partial class ColorTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is Color color ? color.ToString() : default;

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
