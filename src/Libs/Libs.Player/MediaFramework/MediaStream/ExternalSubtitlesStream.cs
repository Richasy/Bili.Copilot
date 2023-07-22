// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Player.Core;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaStream;

/// <summary>
/// 外部字幕流类，继承自外部流类.
/// </summary>
public class ExternalSubtitleStream : ExternalStream
{
    /// <summary>
    /// 初始 URL.URL 可以是从原始输入转换而来的格式.
    /// </summary>
    public string DirectUrl { get; set; }

    /// <summary>
    /// 是否已转换.
    /// </summary>
    public bool Converted { get; set; }

    /// <summary>
    /// 是否已下载.
    /// </summary>
    public bool Downloaded { get; set; }

    /// <summary>
    /// 字幕语言.
    /// </summary>
    public Language Language { get; set; } = Language.Unknown;

    /// <summary>
    /// 评分，范围为 1.0-10.0（0: 未设置）.
    /// </summary>
    public float Rating { get; set; }

    /// <summary>
    /// 字幕标题.
    /// </summary>
    public string Title { get; set; }
}
