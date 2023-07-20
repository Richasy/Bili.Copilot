// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.Enums;

/// <summary>
/// 视频渲染器.
/// </summary>
public enum VideoProcessor
{
    /// <summary>
    /// 自动切换.
    /// </summary>
    Auto,

    /// <summary>
    /// 使用 Direct3D 11 进行视频渲染.
    /// </summary>
    D3D11,

    /// <summary>
    /// 取自 https://github.com/SuRGeoNix/Flyleaf 的视频渲染器.
    /// </summary>
    Flyleaf,
}
