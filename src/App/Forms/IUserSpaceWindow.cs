// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.User;

namespace Bili.Copilot.App.Forms;

/// <summary>
/// 可以显示用户空间的窗口.
/// </summary>
public interface IUserSpaceWindow
{
    /// <summary>
    /// 显示用户空间.
    /// </summary>
    /// <param name="profile">用户信息.</param>
    void ShowUserSpace(UserProfile profile);
}
