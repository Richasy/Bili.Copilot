// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Community;

namespace Bili.Copilot.App.Converters;

/// <summary>
/// 用户关系文本转换.
/// </summary>
internal sealed class RelationTextConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is UserRelationStatus status
            ? status switch
            {
                UserRelationStatus.Unfollow => ResourceToolkit.GetLocalizedString(StringNames.Follow),
                UserRelationStatus.Following => ResourceToolkit.GetLocalizedString(StringNames.Followed),
                UserRelationStatus.BeFollowed => ResourceToolkit.GetLocalizedString(StringNames.Follow),
                UserRelationStatus.Friends => ResourceToolkit.GetLocalizedString(StringNames.FollowEachOther),
                _ => "--",
            }
            : (object)"--";
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
