// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.Enums;

/// <summary>
/// 图像处理方式.
/// </summary>
public enum PixelFormatType
{
    /// <summary>
    /// 硬件加速. 利用 GPU 或其他硬件加速器来执行图像处理操作，以提高性能和效率.
    /// </summary>
    Hardware,

    /// <summary>
    /// 通用软件解码，不一定使用 ffmpeg 的 swscale 库。
    /// 具体的实现可能因平台和环境而异，可以是使用其他软件库或自定义的算法来进行图像处理。
    /// 这种方式可能会有不同的性能和效果，取决于具体的实现.
    /// </summary>
    SoftwareHandled,

    /// <summary>
    /// 使用 ffmpeg 的 swscale 库进行图像处理.
    /// </summary>
    SoftwareSws,
}
