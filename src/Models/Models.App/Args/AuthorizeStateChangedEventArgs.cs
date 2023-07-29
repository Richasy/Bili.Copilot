// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.Authorize;

namespace Bili.Copilot.Models.App.Args;

/// <summary>
/// ��Ȩ��֤���¼�����.
/// </summary>
public class AuthorizeStateChangedEventArgs : EventArgs
{
    /// <summary>
    /// ����һ�� <see cref="AuthorizeStateChangedEventArgs"/> ���͵�ʵ��.
    /// </summary>
    /// <param name="oldState">ǰһ��<see cref="AuthorizeState"/>.</param>
    /// <param name="newState">��ǰ��<see cref="AuthorizeState"/>.</param>
    public AuthorizeStateChangedEventArgs(AuthorizeState oldState, AuthorizeState newState)
    {
        OldState = oldState;
        NewState = newState;
    }

    /// <summary>
    /// ��ȡǰһ����Ȩ״̬.
    /// </summary>
    public AuthorizeState OldState { get; private set; }

    /// <summary>
    /// ��ȡ�µ���Ȩ״̬.
    /// </summary>
    public AuthorizeState NewState { get; private set; }
}

