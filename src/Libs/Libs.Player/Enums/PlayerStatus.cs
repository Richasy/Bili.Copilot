// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.Enums;

/// <summary>
/// 表示播放器的状态.
/// </summary>
public enum PlayerStatus
{
    /// <summary>
    /// 正在打开中.
    /// </summary>
    Opening,

    /// <summary>
    /// 打开失败.
    /// </summary>
    Failed,

    /// <summary>
    /// 已停止.
    /// </summary>
    Stopped,

    /// <summary>
    /// 已暂停.
    /// </summary>
    Paused,

    /// <summary>
    /// 正在播放中.
    /// </summary>
    Playing,

    /// <summary>
    /// 播放结束.
    /// </summary>
    Ended,
}
