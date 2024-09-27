// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;

namespace BiliCopilot.UI.Forms;

/// <summary>
/// 支持全屏切换的窗口.
/// </summary>
public interface IPlayerHostWindow
{
    /// <summary>
    /// 进入全屏.
    /// </summary>
    void EnterPlayerHostMode(PlayerDisplayMode mode);

    /// <summary>
    /// 退出全屏.
    /// </summary>
    void ExitPlayerHostMode();
}
