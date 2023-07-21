// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Player.Enums;

namespace Bili.Copilot.Libs.Player.Misc;

public class LogHandler
{
    /// <summary>
    /// 构造函数.
    /// </summary>
    /// <param name="prefix">日志前缀.</param>
    public LogHandler(string prefix = "")
        => Prefix = prefix;

    /// <summary>
    /// 日志前缀.
    /// </summary>
    public string Prefix { get; }

    /// <summary>
    /// 记录错误日志.
    /// </summary>
    /// <param name="msg">错误消息.</param>
    public void Error(string msg)
        => Logger.Log($"{Prefix}{msg}", LogLevel.Error);

    /// <summary>
    /// 记录信息日志.
    /// </summary>
    /// <param name="msg">信息消息.</param>
    public void Info(string msg)
        => Logger.Log($"{Prefix}{msg}", LogLevel.Info);

    /// <summary>
    /// 记录警告日志.
    /// </summary>
    /// <param name="msg">警告消息.</param>
    public void Warn(string msg)
        => Logger.Log($"{Prefix}{msg}", LogLevel.Warn);

    /// <summary>
    /// 记录调试日志.
    /// </summary>
    /// <param name="msg">调试消息.</param>
    public void Debug(string msg)
        => Logger.Log($"{Prefix}{msg}", LogLevel.Debug);

    /// <summary>
    /// 记录跟踪日志.
    /// </summary>
    /// <param name="msg">跟踪消息.</param>
    public void Trace(string msg)
        => Logger.Log($"{Prefix}{msg}", LogLevel.Trace);
}
