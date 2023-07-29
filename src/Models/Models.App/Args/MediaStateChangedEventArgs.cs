// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.App.Args;

/// <summary>
/// ý��״̬�����¼�����.
/// </summary>
public sealed class MediaStateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediaStateChangedEventArgs"/> class.
    /// </summary>
    public MediaStateChangedEventArgs(PlayerStatus status, string message)
    {
        Status = status;
        Message = message;
    }

    /// <summary>
    /// ������״̬.
    /// </summary>
    public PlayerStatus Status { get; set; }

    /// <summary>
    /// ��Ϣ.
    /// </summary>
    public string Message { get; set; }
}

