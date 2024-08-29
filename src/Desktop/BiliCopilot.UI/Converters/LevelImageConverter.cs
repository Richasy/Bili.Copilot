// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;

namespace BiliCopilot.UI.Converters;

internal sealed class LevelImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => new BitmapImage(new Uri($"ms-appx:///Assets/Level/level_{(int?)value ?? 0}.png"));

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
