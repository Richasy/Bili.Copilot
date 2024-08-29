// Copyright (c) Bili Copilot. All rights reserved.

using System;

namespace BiliCopilot.UI.Models;

/// <summary>
/// 播放器通知.
/// </summary>
public sealed class PlayerNotification
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerNotification"/> class.
    /// </summary>
    public PlayerNotification(
        Action action,
        string message,
        int duration)
    {
        Action = action;
        Message = message;
        Duration = duration;
    }

    /// <summary>
    /// 点击通知执行的操作.
    /// </summary>
    public Action Action { get; }

    /// <summary>
    /// 通知消息.
    /// </summary>
    public string? Message { get; }

    /// <summary>
    /// 通知持续时间.
    /// </summary>
    public int Duration { get; }
}
