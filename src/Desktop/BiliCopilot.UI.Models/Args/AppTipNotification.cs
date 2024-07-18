// Copyright (c) Bili Copilot. All rights reserved.

using System;
using BiliCopilot.UI.Models.Constants;

namespace BiliCopilot.UI.Models.Args;

/// <summary>
/// 应用提示通知.
/// </summary>
public class AppTipNotification : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppTipNotification"/> class.
    /// </summary>
    /// <param name="msg">消息内容.</param>
    /// <param name="type">提示类型.</param>
    /// <param name="targetWindow">目标窗口.</param>
    public AppTipNotification(string msg, InfoType type = InfoType.Information, object? targetWindow = default)
    {
        Message = msg;
        Type = type;
        TargetWindow = targetWindow;
    }

    /// <summary>
    /// 消息内容.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 提示类型.
    /// </summary>
    public InfoType Type { get; set; }

    /// <summary>
    /// 目标窗口.
    /// </summary>
    public object TargetWindow { get; set; }
}
