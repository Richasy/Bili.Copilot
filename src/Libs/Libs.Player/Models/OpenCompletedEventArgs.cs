// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.IO;

namespace Bili.Copilot.Libs.Player.Models;

/// <summary>
/// 表示打开完成事件参数.
/// </summary>
public sealed class OpenCompletedEventArgs : EventArgs
{
    /// <summary>
    /// 初始化 <see cref="OpenCompletedEventArgs"/> 类的新实例.
    /// </summary>
    /// <param name="url">URL.</param>
    /// <param name="ioStream">IO流.</param>
    /// <param name="error">错误信息.</param>
    /// <param name="isSubtitles">是否为字幕.</param>
    public OpenCompletedEventArgs(string url = null, Stream ioStream = null, string error = null, bool isSubtitles = false)
    {
        Url = url;
        IoStream = ioStream;
        Error = error;
        IsSubtitles = isSubtitles;
    }

    /// <summary>
    /// 获取或设置URL.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 获取或设置IO流.
    /// </summary>
    public Stream IoStream { get; set; }

    /// <summary>
    /// 获取或设置错误信息.
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// 获取是否成功.
    /// </summary>
    public bool Success => Error == null;

    /// <summary>
    /// 获取或设置是否为字幕.
    /// </summary>
    public bool IsSubtitles { get; set; }
}
