// Copyright (c) Bili Copilot. All rights reserved.

using System;

namespace Bili.Copilot.Libs.Player.Models;

/// <summary>
/// 缓冲完成事件参数类.
/// </summary>
public class BufferingCompletedEventArgs : EventArgs
{
    /// <summary>
    /// 初始化 BufferingCompletedEventArgs 类的新实例.
    /// </summary>
    /// <param name="error">错误信息.</param>
    public BufferingCompletedEventArgs(string error)
    {
        Error = error;
        Success = Error == null;
    }

    /// <summary>
    /// 获取错误信息.
    /// </summary>
    public string Error { get; }

    /// <summary>
    /// 获取缓冲是否成功.
    /// </summary>
    public bool Success { get; }
}
