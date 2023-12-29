// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;

namespace Bili.Copilot.App.Converters;

/// <summary>
/// 追番/追剧文本转换.
/// </summary>
public class PgcFollowTextConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var result = string.Empty;
        if (value is bool isFollow)
        {
            result = isFollow ?
                ResourceToolkit.GetLocalizedString(StringNames.PgcFollowing) :
                ResourceToolkit.GetLocalizedString(StringNames.PgcNotFollow);
        }

        return result;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
