// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.App.Converters;

/// <summary>
/// Object to Boolean converter.
/// </summary>
/// <remarks>
/// Returns <c>True</c> when the object is not empty.
/// </remarks>
internal sealed class ObjectToBoolConverter : IValueConverter
{
    /// <summary>
    /// Whether to invert the result.
    /// </summary>
    /// <remarks>
    /// After inversion, return <c>True</c> when the string is empty, otherwise return <c>False</c>.
    /// </remarks>
    public bool IsReverse { get; set; }

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var result = false;
        if (value != null)
        {
            result = value is string str ? !string.IsNullOrEmpty(str) : value is not bool b || b;
        }

        if (IsReverse)
        {
            result = !result;
        }

        return result;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
