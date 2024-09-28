// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Models.Constants;

/// <summary>
/// 偏好的解码模式.
/// </summary>
public enum PreferDecodeType
{
    /// <summary>
    /// 自动.
    /// </summary>
    Auto,

    /// <summary>
    /// D3D11硬解.
    /// </summary>
    D3D11,

    /// <summary>
    /// NVDEC硬解.
    /// </summary>
    NVDEC,

    /// <summary>
    /// DXVA2硬解.
    /// </summary>
    DXVA2,

    /// <summary>
    /// Vulkan硬解.
    /// </summary>
    Vulkan,
}
