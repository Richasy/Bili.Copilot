// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.App;

namespace Bili.Copilot.App.Converters;

/// <summary>
/// 固定条目类型图标转换器.
/// </summary>
internal sealed class FixTypeIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is FixedType type)
        {
            var icon = type switch
            {
                FixedType.Pgc => FluentSymbol.Dust,
                FixedType.Live => FluentSymbol.Video,
                FixedType.Video => FluentSymbol.VideoClip,
                FixedType.User => FluentSymbol.Person,
                _ => FluentSymbol.Question,
            };

            return icon;
        }

        return FluentSymbol.Question;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
