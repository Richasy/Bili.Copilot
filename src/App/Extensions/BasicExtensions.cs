// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.App.Extensions;

/// <summary>
/// 基础扩展.
/// </summary>
public static class BasicExtension
{
    /// <summary>
    /// 将对象转换为Int32.
    /// </summary>
    /// <param name="obj">对象.</param>
    /// <returns><see cref="int"/>.</returns>
    public static int ToInt32(this object obj)
        => int.TryParse(obj?.ToString(), out var value) ? value : 0;

    /// <summary>
    /// 将对象转换为Double.
    /// </summary>
    /// <param name="obj">对象.</param>
    /// <returns><see cref="double"/>.</returns>
    public static double ToDouble(this object obj)
        => double.TryParse(obj?.ToString(), out var value) ? value : 0d;
}
