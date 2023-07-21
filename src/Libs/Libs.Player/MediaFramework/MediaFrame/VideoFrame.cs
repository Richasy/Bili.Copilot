// Copyright (c) Bili Copilot. All rights reserved.

using FFmpeg.AutoGen;
using Vortice.Direct3D11;
using ID3D11Texture2D = Vortice.Direct3D11.ID3D11Texture2D;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaFrame;

/// <summary>
/// 视频帧类，继承自帧基类.
/// </summary>
public unsafe class VideoFrame : FrameBase
{
    /// <summary>
    /// 获取或设置纹理数组.
    /// </summary>
    public ID3D11Texture2D[] Textures { get; set; }

    /// <summary>
    /// 获取或设置着色器资源视图数组.
    /// </summary>
    public ID3D11ShaderResourceView[] Srvs { get; set; }

    /// <summary>
    /// 获取或设置子资源索引，用于表示FFmpeg纹理的数组索引.
    /// </summary>
    public int SubResource { get; set; }

    /// <summary>
    /// 获取或设置缓冲区引用，用于告诉FFmpeg我们仍然需要它.
    /// </summary>
    public AVBufferRef* BufRef { get; set; }
}
