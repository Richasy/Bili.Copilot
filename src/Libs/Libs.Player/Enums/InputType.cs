// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.Enums;

/// <summary>
/// 输入源类型.
/// </summary>
public enum InputType
{
    /// <summary>
    /// 文件.
    /// </summary>
    File = 0,

    /// <summary>
    /// 共享资源链接.
    /// </summary>
    UNC = 1,

    /// <summary>
    /// 种子文件.
    /// </summary>
    Torrent = 2,

    /// <summary>
    /// 网页资源.
    /// </summary>
    Web = 3,

    /// <summary>
    /// 未知.
    /// </summary>
    Unknown = 4,
}
