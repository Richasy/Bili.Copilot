// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;

namespace Bili.Copilot.App.Converters;

/// <summary>
/// 用户等级转换器，将等级转化为对应的图片.
/// </summary>
internal sealed class UserLevelConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
        => new BitmapImage(new Uri($"ms-appx:///Assets/Level/level_{value}.png"));

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
