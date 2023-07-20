// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Bili.Copilot.Libs.Player.Models;

/// <summary>
/// 表示一个宽高比的结构体.
/// </summary>
public struct AspectRatio
{
    /// <summary>
    /// 保持宽高比.
    /// </summary>
    public static readonly AspectRatio Keep = new(-1, 1);

    /// <summary>
    /// 填充宽高比.
    /// </summary>
    public static readonly AspectRatio Fill = new(-2, 1);

    /// <summary>
    /// 自定义宽高比.
    /// </summary>
    public static readonly AspectRatio Custom = new(-3, 1);

    /// <summary>
    /// 无效的宽高比.
    /// </summary>
    public static readonly AspectRatio Invalid = new(-999, 1);

    /// <summary>
    /// 支持的宽高比列表.
    /// </summary>
    public static readonly List<AspectRatio> AspectRatios = new()
    {
        Keep,
        Fill,
        Custom,
        new AspectRatio(1, 1),
        new AspectRatio(4, 3),
        new AspectRatio(16, 9),
        new AspectRatio(16, 10),
        new AspectRatio(2.35f, 1),
    };

    /// <summary>
    /// 使用指定的宽度和高度创建 AspectRatio 对象.
    /// </summary>
    /// <param name="value">宽度.</param>
    public AspectRatio(float value)
        : this(value, 1)
    {
    }

    /// <summary>
    /// 使用指定的宽度和高度创建 AspectRatio 对象.
    /// </summary>
    /// <param name="num">宽度.</param>
    /// <param name="den">高度.</param>
    public AspectRatio(float num, float den)
    {
        Num = num;
        Den = den;
    }

    /// <summary>
    /// 使用指定的字符串创建 AspectRatio 对象.
    /// </summary>
    /// <param name="value">字符串表示的宽高比.</param>
    public AspectRatio(string value)
    {
        Num = Invalid.Num;
        Den = Invalid.Den;
        FromString(value);
    }

    /// <summary>
    /// 获取或设置宽度.
    /// </summary>
    public float Num { get; set; }

    /// <summary>
    /// 获取或设置高度.
    /// </summary>
    public float Den { get; set; }

    /// <summary>
    /// 获取或设置宽高比的值.
    /// </summary>
    public float Value
    {
        readonly get => Num / Den;
        set
        {
            Num = value;
            Den = 1;
        }
    }

    /// <summary>
    /// 获取或设置宽高比的字符串表示.
    /// </summary>
    public string ValueStr
    {
        readonly get => ToString();
        set => FromString(value);
    }

    /// <summary>
    /// 隐式转换为 <see cref="AspectRatio"/> 类型.
    /// </summary>
    /// <param name="value">要转换的字符串.</param>
    /// <returns>转换后的 <see cref="AspectRatio"/> 对象.</returns>
    public static implicit operator AspectRatio(string value) => new(value);

    /// <summary>
    /// 判断两个 AspectRatio 对象是否相等.
    /// </summary>
    /// <param name="a">要比较的第一个对象.</param>
    /// <param name="b">要比较的第二个对象.</param>
    /// <returns>如果相等，则为 true；否则为 false.</returns>
    public static bool operator ==(AspectRatio a, AspectRatio b)
        => a.Equals(b);

    /// <summary>
    /// 判断两个 AspectRatio 对象是否不相等.
    /// </summary>
    /// <param name="a">要比较的第一个对象.</param>
    /// <param name="b">要比较的第二个对象.</param>
    /// <returns>如果不相等，则为 true；否则为 false.</returns>
    public static bool operator !=(AspectRatio a, AspectRatio b) => !(a == b);

    /// <summary>
    /// 判断当前对象是否与指定对象相等.
    /// </summary>
    /// <param name="obj">要比较的对象.</param>
    /// <returns>如果相等，则为 true；否则为 false.</returns>
    public override readonly bool Equals(object obj)
        => obj != null && GetType().Equals(obj.GetType()) && Num == ((AspectRatio)obj).Num && Den == ((AspectRatio)obj).Den;

    /// <inheritdoc/>
    public override readonly int GetHashCode() => (int)(Value * 1000);

    /// <inheritdoc/>
    public override readonly string ToString()
        => this == Keep ? "Keep" : (this == Fill ? "Fill" : (this == Custom ? "Custom" : (this == Invalid ? "Invalid" : $"{Num}:{Den}")));

    /// <summary>
    /// 从字符串解析并设置宽高比.
    /// </summary>
    /// <param name="value">要解析的字符串.</param>
    public void FromString(string value)
    {
        if (value == "Keep")
        {
            Num = Keep.Num;
            Den = Keep.Den;
            return;
        }
        else if (value == "Fill")
        {
            Num = Fill.Num;
            Den = Fill.Den;
            return;
        }
        else if (value == "Custom")
        {
            Num = Custom.Num;
            Den = Custom.Den;
            return;
        }
        else if (value == "Invalid")
        {
            Num = Invalid.Num;
            Den = Invalid.Den;
            return;
        }

        var newValue = value.ToString().Replace(',', '.');

        if (Regex.IsMatch(newValue.ToString(), @"^\s*[0-9\.]+\s*[:/]\s*[0-9\.]+\s*$"))
        {
            var values = newValue.ToString().Split(':');
            if (values.Length < 2)
            {
                values = newValue.ToString().Split('/');
            }

            Num = float.Parse(values[0], NumberStyles.Any, CultureInfo.InvariantCulture);
            Den = float.Parse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture);
        }
        else if (float.TryParse(newValue.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        {
            Num = result;
            Den = 1;
        }
        else
        {
            Num = Invalid.Num;
            Den = Invalid.Den;
        }
    }
}
