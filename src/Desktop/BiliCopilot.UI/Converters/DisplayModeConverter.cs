// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Microsoft.UI.Xaml.Data;

namespace BiliCopilot.UI.Converters;

internal sealed partial class DisplayModeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var result = string.Empty;
        if (value is PlayerDisplayMode mode)
        {
            switch (mode)
            {
                case PlayerDisplayMode.Default:
                    result = ResourceToolkit.GetLocalizedString(StringNames.Default);
                    break;
                case PlayerDisplayMode.FullScreen:
                    result = ResourceToolkit.GetLocalizedString(StringNames.FullScreenMode);
                    break;
                case PlayerDisplayMode.CompactOverlay:
                    result = ResourceToolkit.GetLocalizedString(StringNames.CompactOverlayMode);
                    break;
            }
        }

        return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
