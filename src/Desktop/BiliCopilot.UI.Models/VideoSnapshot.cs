// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.Models;

/// <summary>
/// 视频快照.
/// </summary>
public sealed class VideoSnapshot
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoSnapshot"/> class.
    /// </summary>
    public VideoSnapshot(
        VideoInformation video,
        bool isPrivate = false)
    {
        Video = video;
        IsPrivate = isPrivate;
    }

    /// <summary>
    /// 视频信息.
    /// </summary>
    public VideoInformation Video { get; }

    /// <summary>
    /// 无痕播放（不上报历史记录）.
    /// </summary>
    public bool IsPrivate { get; }
}
