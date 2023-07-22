// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.Models;

/// <summary>
/// FFmpeg 过滤器.
/// </summary>
public class Filter
{
    /// <summary>
    /// FFmpeg 有效的过滤器 ID.
    /// （仅在发送命令时需要）.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// FFmpeg 有效的过滤器名称.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// FFmpeg 有效的过滤器参数.
    /// </summary>
    public string Args { get; set; }
}
