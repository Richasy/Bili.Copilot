// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Microsoft.UI.Xaml.Data;

namespace BiliCopilot.UI.Converters;

/// <summary>
/// 全屏图标及文本转换器.
/// </summary>
internal sealed partial class FullScreenConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var isFullScreen = (bool)value;

        if (targetType == typeof(FluentIcons.Common.Symbol))
        {
            return isFullScreen
                ? FluentIcons.Common.Symbol.FullScreenMinimize
                : FluentIcons.Common.Symbol.FullScreenMaximize;
        }
        else
        {
            return isFullScreen
                ? ResourceToolkit.GetLocalizedString(StringNames.ExitFullScreen)
                : ResourceToolkit.GetLocalizedString(StringNames.EnterFullScreen);
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
