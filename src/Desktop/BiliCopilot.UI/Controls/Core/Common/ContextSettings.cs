// Copyright (c) Bili Copilot. All rights reserved.

using System.Diagnostics.CodeAnalysis;
using Silk.NET.Windowing;

namespace BiliCopilot.UI.Controls.Core.Common;

/// <summary>
/// 上下文设置.
/// </summary>
public sealed class ContextSettings
{
    /// <summary>
    /// 主版本.
    /// </summary>
    public int MajorVersion { get; set; } = 3;

    /// <summary>
    /// 次版本.
    /// </summary>
    public int MinorVersion { get; set; } = 3;

    /// <summary>
    /// 上下文标志.
    /// </summary>
    public ContextFlags GraphicsContextFlags { get; set; } = ContextFlags.ForwardCompatible;

    /// <summary>
    /// 图形配置.
    /// </summary>
    public ContextProfile GraphicsProfile { get; set; } = ContextProfile.Compatability;

    /// <summary>
    /// 是否使用默认上下文.
    /// </summary>
    /// <returns>结果.</returns>
    public static bool WouldResultInSameContext([NotNull] ContextSettings a, [NotNull] ContextSettings b)
    {
        if (a.MajorVersion != b.MajorVersion)
        {
            return false;
        }

        if (a.MinorVersion != b.MinorVersion)
        {
            return false;
        }

        if (a.GraphicsProfile != b.GraphicsProfile)
        {
            return false;
        }

        if (a.GraphicsContextFlags != b.GraphicsContextFlags)
        {
            return false;
        }

        return true;
    }
}
