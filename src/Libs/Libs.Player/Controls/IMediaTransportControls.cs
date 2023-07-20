// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.Controls;

/// <summary>
/// 媒体传输控件的接口.
/// </summary>
public interface IMediaTransportControls
{
    /// <summary>
    /// 是否可以隐藏光标.
    /// </summary>
    /// <returns>是否可以.</returns>
    bool CanHideCursor();

    /// <summary>
    /// 当前是否为全屏.
    /// </summary>
    /// <returns>是否为全屏.</returns>
    bool IsFullScreen();

    /// <summary>
    /// 设置全屏状态.
    /// </summary>
    /// <param name="isFullScreen">全屏.</param>
    void SetFullScreenState(bool isFullScreen);

    /// <summary>
    /// 控件释放行为.
    /// </summary>
    void Disposed();
}
