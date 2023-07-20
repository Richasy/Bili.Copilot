// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.Enums;

/// <summary>
/// 数据复制操作的状态.
/// </summary>
/// <remarks>
/// ZeroCopy 是一种视频处理技术，它旨在最小化数据在内存之间的复制操作，从而提高视频处理的效率和性能.
/// </remarks>
public enum ZeroCopyState
{
    /// <summary>
    /// 自动.
    /// </summary>
    Auto,

    /// <summary>
    /// 启用.
    /// </summary>
    Enabled,

    /// <summary>
    /// 关闭.
    /// </summary>
    Disabled,
}
