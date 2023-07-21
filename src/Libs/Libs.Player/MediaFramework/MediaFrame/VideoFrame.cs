using FFmpeg.AutoGen;

using Vortice.Direct3D11;

using ID3D11Texture2D = Vortice.Direct3D11.ID3D11Texture2D;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaFrame;

public unsafe class VideoFrame : FrameBase
{
    public ID3D11Texture2D[]            textures;       // Planes
    public ID3D11ShaderResourceView[]   srvs;           // Views

    // Zero-Copy
    public int                          subresource;    // FFmpeg texture's array index
    public AVBufferRef*                 bufRef;         // Lets ffmpeg to know that we still need it
}
