// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.Enums;

/// <summary>
/// 线程状态.
/// </summary>
public enum ThreadStatus
{
    /// <summary>
    /// 正在打开线程.
    /// </summary>
    Opening,

    /// <summary>
    /// 正在停止线程.
    /// </summary>
    Stopping,

    /// <summary>
    /// 线程已停止.
    /// </summary>
    Stopped,

    /// <summary>
    /// 正在暂停线程.
    /// </summary>
    Pausing,

    /// <summary>
    /// 线程已暂停.
    /// </summary>
    Paused,

    /// <summary>
    /// 线程正在运行.
    /// </summary>
    Running,

    /// <summary>
    /// 队列已满.
    /// </summary>
    QueueFull,

    /// <summary>
    /// 队列为空.
    /// </summary>
    QueueEmpty,

    /// <summary>
    /// 正在排空队列.
    /// </summary>
    Draining,

    /// <summary>
    /// 线程已结束.
    /// </summary>
    Ended,
}
