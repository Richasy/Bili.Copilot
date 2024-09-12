// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;

namespace BiliCopilot.UI.Converters;

/// <summary>
/// 将链接转换为位图.
/// </summary>
internal sealed class UriToBitmapConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is string url
            ? new BitmapImage(new Uri(url))
            : value is null
                ? default
                : new BitmapImage((Uri)value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
