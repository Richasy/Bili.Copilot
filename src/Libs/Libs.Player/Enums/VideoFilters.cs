// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.Enums;

/// <summary>
/// 视频滤镜.
/// </summary>
public enum VideoFilters
{
    /// <summary>
    /// 调整视频的亮度级别.
    /// </summary>
    Brightness = 0x01,

    /// <summary>
    /// 调整视频的对比度，即图像中亮度差异的程度.
    /// </summary>
    Contrast = 0x02,

    /// <summary>
    /// 调整视频的色调，即图像中颜色的整体偏移.
    /// </summary>
    Hue = 0x04,

    /// <summary>
    /// 调整视频的饱和度，即图像中颜色的鲜艳程度.
    /// </summary>
    Saturation = 0x08,

    /// <summary>
    /// 减少视频中的噪点，提高图像的清晰度.
    /// </summary>
    NoiseReduction = 0x10,

    /// <summary>
    /// 增强视频中的边缘，使图像更加清晰锐利.
    /// </summary>
    EdgeEnhancement = 0x20,

    /// <summary>
    /// 对视频进行变形缩放，改变图像的宽高比.
    /// </summary>
    AnamorphicScaling = 0x40,

    /// <summary>
    /// 调整视频的立体声效果，改变声音的空间感.
    /// </summary>
    StereoAdjustment = 0x80,
}
