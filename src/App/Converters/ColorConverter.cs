// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Toolkit;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace Bili.Copilot.App.Converters;

/// <summary>
/// 文本哈希到颜色的转换.
/// </summary>
internal class ColorConverter : IValueConverter
{
    /// <summary>
    /// 是否转换为笔刷.
    /// </summary>
    public bool IsBrush { get; set; }

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var color = Colors.Transparent;
        if (value is string hexColor)
        {
            color = int.TryParse(hexColor, out _)
                ? AppToolkit.HexToColor(hexColor)
                : CommunityToolkit.WinUI.Helpers.ColorHelper.ToColor(hexColor);
        }

        return IsBrush ? new SolidColorBrush(color) : (object)color;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
