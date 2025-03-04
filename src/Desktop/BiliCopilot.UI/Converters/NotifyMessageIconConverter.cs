// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml.Data;
using Richasy.BiliKernel.Models.User;

namespace BiliCopilot.UI.Converters;

internal sealed partial class NotifyMessageIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var type = (NotifyMessageType)value;
        return type switch
        {
            NotifyMessageType.Reply => FluentIcons.Common.Symbol.Comment,
            NotifyMessageType.At => FluentIcons.Common.Symbol.CallConnecting,
            NotifyMessageType.Like => FluentIcons.Common.Symbol.ThumbLike,
            _ => throw new Exception("Unknown type."),
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
