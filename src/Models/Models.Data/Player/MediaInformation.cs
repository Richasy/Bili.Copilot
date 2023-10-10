// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.Data.Player;

/// <summary>
/// 媒体信息.
/// </summary>
public sealed class MediaInformation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediaInformation"/> class.
    /// </summary>
    /// <param name="minBufferTime">最低缓冲时间.</param>
    /// <param name="videoSegments">不同清晰度的视频列表.</param>
    /// <param name="audioSegments">不同码率的音频列表.</param>
    /// <param name="formats">格式列表.</param>
    public MediaInformation(
        double minBufferTime,
        List<SegmentInformation> videoSegments,
        List<SegmentInformation> audioSegments,
        List<FormatInformation> formats)
    {
        MinBufferTime = minBufferTime;
        VideoSegments = videoSegments;
        AudioSegments = audioSegments;
        Formats = formats;
    }

    /// <summary>
    /// 最低缓冲时间.
    /// </summary>
    public double MinBufferTime { get; }

    /// <summary>
    /// 不同清晰度的视频列表.
    /// </summary>
    public List<SegmentInformation> VideoSegments { get; }

    /// <summary>
    /// 不同码率的音频列表.
    /// </summary>
    public List<SegmentInformation> AudioSegments { get; }

    /// <summary>
    /// 格式列表.
    /// </summary>
    public List<FormatInformation> Formats { get; }
}
