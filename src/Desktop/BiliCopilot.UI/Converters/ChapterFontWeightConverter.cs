// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Data;

namespace BiliCopilot.UI.Converters;

/// <summary>
/// 把“是否当前章节”转为字重：当前章节加粗，其余常规.
/// </summary>
public sealed partial class ChapterFontWeightConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is true ? FontWeights.SemiBold : FontWeights.Normal;

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
