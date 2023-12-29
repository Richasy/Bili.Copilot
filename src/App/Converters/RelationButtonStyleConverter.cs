// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.Community;

namespace Bili.Copilot.App.Converters;

/// <summary>
/// 关系按钮样式转换器.
/// </summary>
internal sealed class RelationButtonStyleConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is UserRelationStatus status
            ? status is UserRelationStatus.Unfollow or UserRelationStatus.BeFollowed
                ? App.Current.Resources["AccentButtonStyle"] as Style
                : App.Current.Resources["DefaultButtonStyle"] as Style
            : (object)null;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
