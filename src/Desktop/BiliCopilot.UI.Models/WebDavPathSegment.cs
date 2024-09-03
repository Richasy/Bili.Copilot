// Copyright (c) Bili Copilot. All rights reserved.

using System;

namespace BiliCopilot.UI.Models;

/// <summary>
/// WebDav 路径片段.
/// </summary>
public sealed class WebDavPathSegment
{
    /// <summary>
    /// 显示名称.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 路径.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// 是否为根目录.
    /// </summary>
    public bool IsRoot => Path == "/";

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is WebDavPathSegment segment && Path == segment.Path;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Path);
}
