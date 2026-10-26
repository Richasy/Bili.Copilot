// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace BiliCopilot.UI.Converters;

/// <summary>
/// 把“是否当前章节”转为前景色：当前章节用强调文字色，其余用主要文字色.
/// </summary>
public sealed partial class ChapterForegroundConverter : IValueConverter
{
    private static readonly Brush FallbackBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var key = value is true ? "AccentTextFillColorPrimaryBrush" : "TextFillColorPrimaryBrush";
        if (Application.Current?.Resources.TryGetValue(key, out var brush) == true && brush is Brush result)
        {
            return result;
        }

        return FallbackBrush;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
