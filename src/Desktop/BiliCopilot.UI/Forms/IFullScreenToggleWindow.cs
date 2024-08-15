// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Forms;

/// <summary>
/// 支持全屏切换的窗口.
/// </summary>
public interface IPlayerHostWindow
{
    /// <summary>
    /// 进入全屏.
    /// </summary>
    void EnterPlayerHostMode();

    /// <summary>
    /// 退出全屏.
    /// </summary>
    void ExitPlayerHostMode();
}
