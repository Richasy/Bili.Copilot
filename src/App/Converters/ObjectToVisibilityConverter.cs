// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Bili.Copilot.App.Converters;

/// <summary>
/// Object to visibility converter. Returns Collapsed when the object is empty.
/// </summary>
internal sealed class ObjectToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Whether to invert the result.
    /// </summary>
    public bool IsReverse { get; set; }

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var isShow = true;
        if (value == null)
        {
            isShow = false;
        }
        else if (value is string str)
        {
            isShow = !string.IsNullOrEmpty(str);
        }
        else if (value is int numInt)
        {
            isShow = numInt <= 0;
        }
        else if (value is double numDouble)
        {
            isShow = numDouble <= 0;
        }

        if (IsReverse)
        {
            isShow = !isShow;
        }

        return isShow ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
