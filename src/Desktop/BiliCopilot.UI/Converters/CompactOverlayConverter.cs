// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Microsoft.UI.Xaml.Data;

namespace BiliCopilot.UI.Converters;

/// <summary>
/// 全屏图标及文本转换器.
/// </summary>
internal sealed class CompactOverlayConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var isCompact = (bool)value;

        if (targetType == typeof(FluentIcons.Common.Symbol))
        {
            return isCompact
                ? FluentIcons.Common.Symbol.ContractDownLeft
                : FluentIcons.Common.Symbol.ContractUpRight;
        }
        else
        {
            return isCompact
                ? ResourceToolkit.GetLocalizedString(StringNames.ExitCompactOverlay)
                : ResourceToolkit.GetLocalizedString(StringNames.EnterCompactOverlay);
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
