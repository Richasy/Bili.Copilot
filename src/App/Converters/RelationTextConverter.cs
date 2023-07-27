// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Community;
using Microsoft.UI.Xaml.Data;

namespace Bili.Copilot.App.Converters;

/// <summary>
/// 用户关系文本转换.
/// </summary>
internal sealed class RelationTextConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is UserRelationStatus status)
        {
            return status switch
            {
                UserRelationStatus.Unfollow => ResourceToolkit.GetLocalizedString(StringNames.Follow),
                UserRelationStatus.Following => ResourceToolkit.GetLocalizedString(StringNames.Followed),
                UserRelationStatus.BeFollowed => ResourceToolkit.GetLocalizedString(StringNames.Follow),
                UserRelationStatus.Friends => ResourceToolkit.GetLocalizedString(StringNames.FollowEachOther),
                _ => "--",
            };
        }

        return "--";
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
