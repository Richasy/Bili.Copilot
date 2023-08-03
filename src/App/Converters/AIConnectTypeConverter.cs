// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Microsoft.UI.Xaml.Data;

namespace Bili.Copilot.App.Converters;

internal sealed class AIConnectTypeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is AIConnectType type
            ? type switch
            {
                AIConnectType.Protocol => ResourceToolkit.GetLocalizedString(StringNames.ProtocolLaunch),
                AIConnectType.AppService => ResourceToolkit.GetLocalizedString(StringNames.AppService),
                _ => throw new NotImplementedException(),
            }
            : (object)string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
