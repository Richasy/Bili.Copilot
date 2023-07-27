// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Models.Constants.Community;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Bili.Copilot.App.Converters;

/// <summary>
/// 关系按钮样式转换器.
/// </summary>
internal sealed class RelationButtonStyleConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is UserRelationStatus status)
        {
            return status == UserRelationStatus.Unfollow || status == UserRelationStatus.BeFollowed
                ? App.Current.Resources["AccentButtonStyle"] as Style
                : App.Current.Resources["DefaultButtonStyle"] as Style;
        }

        return null;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
