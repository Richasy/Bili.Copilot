// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.Models;

/// <summary>
/// 视频快照.
/// </summary>
public sealed class MediaSnapshot(
        MediaIdentifier video,
        MediaType type,
        bool isPrivate = false)
{
    /// <summary>
    /// 视频信息.
    /// </summary>
    public MediaIdentifier Media { get; } = video;

    /// <summary>
    /// 视频类型.
    /// </summary>
    public MediaType Type { get; } = type;

    /// <summary>
    /// 无痕播放（不上报历史记录）.
    /// </summary>
    public bool IsPrivate { get; } = isPrivate;
}
