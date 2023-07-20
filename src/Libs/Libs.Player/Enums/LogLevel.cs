// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.Enums;

/// <summary>
/// 表示日志级别的枚举.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// 静默级别，不输出任何日志.
    /// </summary>
    Quiet = 0x00,

    /// <summary>
    /// 错误级别，输出错误日志.
    /// </summary>
    Error = 0x10,

    /// <summary>
    /// 警告级别，输出警告日志.
    /// </summary>
    Warn = 0x20,

    /// <summary>
    /// 信息级别，输出信息日志.
    /// </summary>
    Info = 0x30,

    /// <summary>
    /// 调试级别，输出调试日志.
    /// </summary>
    Debug = 0x40,

    /// <summary>
    /// 跟踪级别，输出详细跟踪日志.
    /// </summary>
    Trace = 0x50,
}
