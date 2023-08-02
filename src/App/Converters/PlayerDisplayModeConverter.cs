// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Microsoft.UI.Xaml.Data;

namespace Bili.Copilot.App.Converters;

/// <summary>
/// 播放器显示模式到可读文本的转换器.
/// </summary>
public class PlayerDisplayModeConverter : IValueConverter
{
    /// <inheritdoc/>
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
                default:
                    break;
            }
        }

        return result;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
