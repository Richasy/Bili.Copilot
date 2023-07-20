// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.Enums;

/// <summary>
/// FFmpeg 日志级别枚举.
/// </summary>
public enum FFmpegLogLevel
{
    /// <summary>
    /// 静默模式，不输出任何日志.
    /// </summary>
    Quiet = -0x08,

    /// <summary>
    /// 跳过重复日志.
    /// </summary>
    SkipRepeated = 0x01,

    /// <summary>
    /// 输出日志级别.
    /// </summary>
    PrintLevel = 0x02,

    /// <summary>
    /// 致命错误日志级别.
    /// </summary>
    Fatal = 0x08,

    /// <summary>
    /// 错误日志级别.
    /// </summary>
    Error = 0x10,

    /// <summary>
    /// 警告日志级别.
    /// </summary>
    Warning = 0x18,

    /// <summary>
    /// 信息日志级别.
    /// </summary>
    Info = 0x20,

    /// <summary>
    /// 详细日志级别.
    /// </summary>
    Verbose = 0x28,

    /// <summary>
    /// 调试日志级别.
    /// </summary>
    Debug = 0x30,

    /// <summary>
    /// 跟踪日志级别.
    /// </summary>
    Trace = 0x38,

    /// <summary>
    /// 最大偏移量.
    /// </summary>
    MaxOffset = 0x40,
}
