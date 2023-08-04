// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.App.Args;

/// <summary>
/// Ó¦ÓÃÌáÊ¾Í¨ÖªÊÂ¼þ²ÎÊý.
/// </summary>
public class AppTipNotification : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppTipNotification"/> class.
    /// </summary>
    /// <param name="msg">ÏûÏ¢ÄÚÈÝ.</param>
    /// <param name="type">ÏûÏ¢ÀàÐÍ.</param>
    public AppTipNotification(string msg, InfoType type = InfoType.Information)
    {
        Message = msg;
        Type = type;
    }

    /// <summary>
    /// ÏûÏ¢ÄÚÈÝ.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// ÏûÏ¢ÀàÐÍ.
    /// </summary>
    public InfoType Type { get; set; }
}

