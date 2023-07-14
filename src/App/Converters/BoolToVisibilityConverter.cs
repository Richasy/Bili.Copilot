// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Bili.Copilot.App.Converters;

/// <summary>
/// <see cref="bool"/> to <see cref="Visibility"/>.
/// </summary>
internal sealed class BoolToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Whether to invert the value.
    /// </summary>
    public bool IsReverse { get; set; }

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var vis = Visibility.Visible;
        if (value is bool v)
        {
            vis = IsReverse
                ? v ? Visibility.Collapsed : Visibility.Visible
                : v ? Visibility.Visible : Visibility.Collapsed;
        }

        return vis;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
