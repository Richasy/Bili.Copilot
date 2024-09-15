// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Microsoft.UI.Xaml.Data;

namespace BiliCopilot.UI.Converters;

internal sealed class DisplayModeConverter : IValueConverter
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
                case PlayerDisplayMode.FullWindow:
                    result = ResourceToolkit.GetLocalizedString(StringNames.FullWindowMode);
                    break;
                case PlayerDisplayMode.CompactOverlay:
                    result = ResourceToolkit.GetLocalizedString(StringNames.CompactOverlayMode);
                    break;
                case PlayerDisplayMode.NewWindow:
                    result = ResourceToolkit.GetLocalizedString(StringNames.NewWindow);
                    break;
            }
        }

        return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
