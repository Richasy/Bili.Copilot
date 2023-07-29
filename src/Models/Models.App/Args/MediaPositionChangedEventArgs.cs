// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.App.Args;

/// <summary>
/// 媒体播放进度变更事件参数.
/// </summary>
public sealed class MediaPositionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediaPositionChangedEventArgs"/> class.
    /// </summary>
    public MediaPositionChangedEventArgs(TimeSpan time, TimeSpan duration)
    {
        Position = time;
        Duration = duration;
    }

    /// <summary>
    /// 播放进度.
    /// </summary>
    public TimeSpan Position { get; set; }

    /// <summary>
    /// 视频时长.
    /// </summary>
    public TimeSpan Duration { get; set; }
}

