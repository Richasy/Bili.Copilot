// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.Player;

namespace Bili.Copilot.Models.App.Args;

/// <summary>
/// WebDav字幕列表改变事件参数.
/// </summary>
public sealed class WebDavSubtitleListChangedEventArgs : EventArgs
{
    /// <summary>
    /// 字幕列表.
    /// </summary>
    public List<SubtitleMeta> Subtitles { get; set; }

    /// <summary>
    /// 选中的字幕.
    /// </summary>
    public SubtitleMeta SelectedMeta { get; set; }
}
