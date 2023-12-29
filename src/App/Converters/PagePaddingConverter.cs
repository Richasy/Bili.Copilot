// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.App.Converters;

internal sealed class PagePaddingConverter : IValueConverter
{
    public bool IsHeader { get; set; }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var v = (double)value;
        return IsHeader ? new Thickness(v, v - 8, v, 0) : new Thickness(v, 0, v, 0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
