// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Toolkit;
using Microsoft.UI.Xaml.Data;

namespace Bili.Copilot.App.Converters;

/// <summary>
/// 时长转换为可读文本.
/// </summary>
public class DurationConverter : IValueConverter
{
    /// <summary>
    /// 是否为毫秒记录.
    /// </summary>
    public bool IsMilliseconds { get; set; }

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is int time
            ? IsMilliseconds
                ? NumberToolkit.GetDurationText(TimeSpan.FromMilliseconds(time))
                : NumberToolkit.GetDurationText(TimeSpan.FromSeconds(time))
            : (object)value.ToString();
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
