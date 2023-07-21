// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.Enums;

/// <summary>
/// HDR 转 SDR 方法.
/// </summary>
public enum HdrToSdrMethod : int
{
    /// <summary>
    /// 不做处理.
    /// </summary>
    None = 0,

    /// <summary>
    /// Academy Color Encoding System.<br/>
    /// ACES是一种广泛使用的颜色编码系统，它提供了一种标准化的方法来处理高动态范围图像.
    /// ACES算法通过将HDR图像的亮度范围映射到SDR范围内，并保留尽可能多的细节和色彩信息来实现转换.
    /// </summary>
    Aces = 1,

    /// <summary>
    /// Hable算法使用了一种非线性映射函数，将HDR图像的亮度范围映射到SDR范围内.
    /// 在保留细节和色彩信息的同时，还能够提供更好的对比度和色彩饱和度.
    /// </summary>
    Hable = 2,

    /// <summary>
    /// Reinhard算法使用了一种全局色调映射函数，将HDR图像的亮度范围映射到SDR范围内.
    /// Reinhard算法通过调整图像的曝光和对比度来实现转换.
    /// </summary>
    Reinhard = 3,
}
