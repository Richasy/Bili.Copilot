// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.Local;

namespace Bili.Copilot.Models.App.Args;

/// <summary>
/// 播放页面导航事件参数.
/// </summary>
public sealed class PlayerPageNavigateEventArgs : EventArgs
{
    /// <summary>
    /// 播放快照.
    /// </summary>
    public PlaySnapshot Snapshot { get; set; }

    /// <summary>
    /// 关联的窗口.
    /// </summary>
    public object AttachedWindow { get; set; }
}
