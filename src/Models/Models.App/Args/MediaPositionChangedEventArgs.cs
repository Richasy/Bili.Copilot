// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.App.Args;

/// <summary>
/// ý�岥�Ž��ȱ���¼�����.
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
    /// ���Ž���.
    /// </summary>
    public TimeSpan Position { get; set; }

    /// <summary>
    /// ��Ƶʱ��.
    /// </summary>
    public TimeSpan Duration { get; set; }
}

