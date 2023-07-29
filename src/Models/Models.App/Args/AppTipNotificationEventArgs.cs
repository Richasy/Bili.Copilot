// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.App.Args;

/// <summary>
/// Ӧ����ʾ֪ͨ�¼�����.
/// </summary>
public class AppTipNotificationEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppTipNotificationEventArgs"/> class.
    /// </summary>
    /// <param name="msg">��Ϣ����.</param>
    /// <param name="type">��Ϣ����.</param>
    public AppTipNotificationEventArgs(string msg, InfoType type = InfoType.Information)
    {
        Message = msg;
        Type = type;
    }

    /// <summary>
    /// ��Ϣ����.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// ��Ϣ����.
    /// </summary>
    public InfoType Type { get; set; }
}

