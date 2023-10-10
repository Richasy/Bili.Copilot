// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Globalization;
using System.Reflection;
using Bili.Copilot.Libs.Toolkit;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

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
                : ToColor(hexColor);
        }

        return IsBrush ? new SolidColorBrush(color) : color;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();

    private static Color ToColor(string colorString)
    {
        if (string.IsNullOrEmpty(colorString))
        {
            ThrowArgumentException();
        }

        if (colorString[0] == '#')
        {
            switch (colorString.Length)
            {
                case 9:
                    {
                        var cuint = System.Convert.ToUInt32(colorString.Substring(1), 16);
                        var a = (byte)(cuint >> 24);
                        var r = (byte)((cuint >> 16) & 0xff);
                        var g = (byte)((cuint >> 8) & 0xff);
                        var b = (byte)(cuint & 0xff);

                        return Color.FromArgb(a, r, g, b);
                    }

                case 7:
                    {
                        var cuint = System.Convert.ToUInt32(colorString.Substring(1), 16);
                        var r = (byte)((cuint >> 16) & 0xff);
                        var g = (byte)((cuint >> 8) & 0xff);
                        var b = (byte)(cuint & 0xff);

                        return Color.FromArgb(255, r, g, b);
                    }

                case 5:
                    {
                        var cuint = System.Convert.ToUInt16(colorString.Substring(1), 16);
                        var a = (byte)(cuint >> 12);
                        var r = (byte)((cuint >> 8) & 0xf);
                        var g = (byte)((cuint >> 4) & 0xf);
                        var b = (byte)(cuint & 0xf);
                        a = (byte)((a << 4) | a);
                        r = (byte)((r << 4) | r);
                        g = (byte)((g << 4) | g);
                        b = (byte)((b << 4) | b);

                        return Color.FromArgb(a, r, g, b);
                    }

                case 4:
                    {
                        var cuint = System.Convert.ToUInt16(colorString.Substring(1), 16);
                        var r = (byte)((cuint >> 8) & 0xf);
                        var g = (byte)((cuint >> 4) & 0xf);
                        var b = (byte)(cuint & 0xf);
                        r = (byte)((r << 4) | r);
                        g = (byte)((g << 4) | g);
                        b = (byte)((b << 4) | b);

                        return Color.FromArgb(255, r, g, b);
                    }

                default:
                    return ThrowFormatException();
            }
        }

        if (colorString.Length > 3 && colorString[0] == 's' && colorString[1] == 'c' && colorString[2] == '#')
        {
            var values = colorString.Split(',');

            if (values.Length == 4)
            {
                var scA = double.Parse(values[0].Substring(3), CultureInfo.InvariantCulture);
                var scR = double.Parse(values[1], CultureInfo.InvariantCulture);
                var scG = double.Parse(values[2], CultureInfo.InvariantCulture);
                var scB = double.Parse(values[3], CultureInfo.InvariantCulture);

                return Color.FromArgb((byte)(scA * 255), (byte)(scR * 255), (byte)(scG * 255), (byte)(scB * 255));
            }

            if (values.Length == 3)
            {
                var scR = double.Parse(values[0].Substring(3), CultureInfo.InvariantCulture);
                var scG = double.Parse(values[1], CultureInfo.InvariantCulture);
                var scB = double.Parse(values[2], CultureInfo.InvariantCulture);

                return Color.FromArgb(255, (byte)(scR * 255), (byte)(scG * 255), (byte)(scB * 255));
            }

            return ThrowFormatException();
        }

        var prop = typeof(Colors).GetTypeInfo().GetDeclaredProperty(colorString);

        return prop != null ? (Color)prop.GetValue(null) : ThrowFormatException();

        static void ThrowArgumentException() => throw new ArgumentException("The parameter \"colorString\" must not be null or empty.");
        static Color ThrowFormatException() => throw new FormatException("The parameter \"colorString\" is not a recognized Color format.");
    }
}
