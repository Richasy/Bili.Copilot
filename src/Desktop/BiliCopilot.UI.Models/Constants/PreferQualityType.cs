// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Models.Constants;

/// <summary>
/// 偏好视频质量类型.
/// </summary>
public enum PreferQualityType
{
    /// <summary>
    /// 最高画质.
    /// </summary>
    High,

    /// <summary>
    /// 4K 优先.
    /// </summary>
    FourK,

    /// <summary>
    /// 高清（1080P）优先.
    /// </summary>
    HD,

    /// <summary>
    /// 自动（延续上一次播放设置）.
    /// </summary>
    Auto,
}
