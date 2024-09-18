// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Microsoft.UI.Xaml.Data;

namespace BiliCopilot.UI.Converters;

internal sealed partial class PlayerTypeConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var result = string.Empty;
        if (value is PlayerType type)
        {
            switch (type)
            {
                case PlayerType.Native:
                    result = ResourceToolkit.GetLocalizedString(StringNames.Native);
                    break;
                case PlayerType.External:
                    result = ResourceToolkit.GetLocalizedString(StringNames.ExternalPlayer);
                    break;
                case PlayerType.Web:
                    result = ResourceToolkit.GetLocalizedString(StringNames.Web);
                    break;
                default:
                    result = "MPV";
                    break;
            }
        }

        return result;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
