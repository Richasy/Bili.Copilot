// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.Enums;

/// <summary>
/// 色彩空间.
/// </summary>
public enum ColorSpace : int
{
    /// <summary>
    /// 不指定色彩空间.
    /// </summary>
    None = 0,

    /// <summary>
    /// 即 SDTV，适用于标清视频。它的色域范围较小，色彩精度较低，主要用于传输和显示标准分辨率的视频.
    /// </summary>
    BT601 = 1,

    /// <summary>
    /// 即 HDTV，适用于高清视频。相比于BT601，BT709的色域范围更广，色彩精度更高，能够提供更丰富、更真实的色彩表现.
    /// </summary>
    BT709 = 2,

    /// <summary>
    /// 超高清电视（UHDTV）的色彩空间，适用于4K和8K分辨率的视频。它的色域范围更大，色彩精度更高，能够呈现更广阔、更细腻的色彩.
    /// </summary>
    BT2020 = 3,
}
