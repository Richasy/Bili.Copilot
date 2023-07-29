// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.Bili;

namespace Bili.Copilot.Models.App.Args;

/// <summary>
/// ֱ������Ϣ��Ϣ.
/// </summary>
public class LiveMessageEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveMessageEventArgs"/> class.
    /// </summary>
    /// <param name="type">��Ϣ����.</param>
    /// <param name="data">���ݲ���.</param>
    public LiveMessageEventArgs(LiveMessageType type, object data)
    {
        Type = type;
        Data = data;
    }

    /// <summary>
    /// ��Ϣ����.
    /// </summary>
    public LiveMessageType Type { get; set; }

    /// <summary>
    /// ���ݲ���.
    /// </summary>
    public object Data { get; set; }
}

