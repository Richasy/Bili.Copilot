// Copyright (c) Bili Copilot. All rights reserved.

using System;

namespace Bili.Copilot.Libs.Player.Models;

/// <summary>
/// 表示播放停止事件的参数.
/// </summary>
public class PlaybackStoppedEventArgs : EventArgs
{
    /// <summary>
    /// 初始化 PlaybackStoppedEventArgs 类的新实例.
    /// </summary>
    /// <param name="error">停止播放时的错误信息.</param>
    public PlaybackStoppedEventArgs(string error)
    {
        Error = error;
        Success = Error == null;
    }

    /// <summary>
    /// 获取停止播放时的错误信息.
    /// </summary>
    public string Error { get; }

    /// <summary>
    /// 获取一个值，指示播放是否成功停止.
    /// </summary>
    public bool Success { get; }
}
