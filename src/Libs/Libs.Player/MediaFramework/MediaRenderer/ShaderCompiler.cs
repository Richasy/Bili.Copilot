using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Vortice.D3DCompiler;
using Vortice.Direct3D;
using Vortice.Direct3D11;

using static FlyleafLib.Utils;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaRenderer;

internal class BlobWrapper
{
    public Blob blob;
    public BlobWrapper(Blob blob) => this.blob = blob;
    public BlobWrapper() { }
}

internal static class ShaderCompiler
{
    internal static Blob VSBlob = Compile(VS, false); // TODO Embedded?

    const int MAXSIZE = 64;
    static Dictionary<string, BlobWrapper> cache = new();

    internal static ID3D11PixelShader CompilePS(ID3D11Device device, string uniqueId, string hlslSample, List<string> defines = null)
    {
        BlobWrapper bw;

        lock (cache)
        {
            if (cache.Count > MAXSIZE)
            {
                Engine.Log.Trace($"[ShaderCompiler] Clears cache");
                foreach (var bw1 in cache.Values)
                    bw1.blob.Dispose();

                cache.Clear();
            }

            cache.TryGetValue(uniqueId, out var bw2);
            if (bw2 != null)
            {
                Engine.Log.Trace($"[ShaderCompiler] Found in cache {uniqueId}");
                lock(bw2)
                    return device.CreatePixelShader(bw2.blob);
            }

            bw = new();
            Monitor.Enter(bw);
            cache.Add(uniqueId, bw);
        }

        var blob = Compile(PS_HEADER + hlslSample + PS_FOOTER, true, defines);
        bw.blob = blob;
        var ps = device.CreatePixelShader(bw.blob);
        Monitor.Exit(bw);

        Engine.Log.Trace($"[ShaderCompiler] Compiled {uniqueId}");
        return ps;
    }
    internal static Blob Compile(string hlsl, bool isPS = true, List<string> defines = null)
    {
        ShaderMacro[] definesMacro = null;

        if (defines != null)
        {
            definesMacro = new ShaderMacro[defines.Count + 1];
            for(int i=0; i<defines.Count; i++)
                definesMacro[i] = new ShaderMacro() { Name = defines[i], Definition = "" };
        }

        return Compile(Encoding.UTF8.GetBytes(hlsl), isPS, definesMacro);

    }
    internal static unsafe Blob Compile(byte[] bytes, bool isPS = true, ShaderMacro[] defines = null)
    {
        string psOrvs = isPS ? "ps" : "vs";
        Compiler.Compile(bytes, defines, null, "main", null, $"{psOrvs}_4_0_level_9_3", ShaderFlags.OptimizationLevel3, out var shaderBlob, out var psError);

        if (psError != null && psError.BufferPointer != IntPtr.Zero)
        {
            string[] errors = BytePtrToStringUTF8((byte*)psError.BufferPointer).Split('\n');

            foreach (string line in errors)
                Engine.Log.Error($"[ShaderCompile] {line}");
        }

        return shaderBlob;
    }

    //private static void CompileEmbeddedShaders() // Not Used
    //{
    //    Assembly assembly = Assembly.GetExecutingAssembly();
    //    string[] shaders = assembly.GetManifestResourceNames().Where(x => GetUrlExtention(x) == "hlsl").ToArray();

    //    foreach (string shader in shaders)
    //        using (Stream stream = assembly.GetManifestResourceStream(shader))
    //        {
    //            string shaderName = shader.Substring(0, shader.Length - 5);
    //            shaderName = shaderName.Substring(shaderName.LastIndexOf('.') + 1);

    //            byte[] bytes = new byte[stream.Length];
    //            stream.Read(bytes, 0, bytes.Length);

    //            CompileShader(bytes, shaderName);
    //        }
    //}

    //private unsafe static void CompileFileShaders()
    //{
    //    List<string> shaders = Directory.EnumerateFiles(EmbeddedShadersFolder, "*.hlsl").ToList();
    //    foreach (string shader in shaders)
    //    {
    //        string shaderName = shader.Substring(0, shader.Length - 5);
    //        shaderName = shaderName.Substring(shaderName.LastIndexOf('\\') + 1);

    //        CompileShader(File.ReadAllBytes(shader), shaderName);
    //    }
    //}

    // Loads compiled blob shaders 
    //private static void LoadShaders()
    //{
    //    Assembly assembly = Assembly.GetExecutingAssembly();
    //    string[] shaders = assembly.GetManifestResourceNames().Where(x => GetUrlExtention(x) == "blob").ToArray();
    //    string tempFile = Path.GetTempFileName();

    //    foreach (string shader in shaders)
    //    {
    //        using (Stream stream = assembly.GetManifestResourceStream(shader))
    //        {
    //            var shaderName = shader.Substring(0, shader.Length - 5);
    //            shaderName = shaderName.Substring(shaderName.LastIndexOf('.') + 1);

    //            byte[] bytes = new byte[stream.Length];
    //            stream.Read(bytes, 0, bytes.Length);

    //            Dictionary<string, Blob> curShaders = shaderName.Substring(0, 2).ToLower() == "vs" ? VSShaderBlobs : PSShaderBlobs;

    //            File.WriteAllBytes(tempFile, bytes);
    //            curShaders.Add(shaderName, Compiler.ReadFileToBlob(tempFile));
    //        }
    //    }
    //}

    // Should work at least from main Samples => FlyleafPlayer (WPF Control) (WPF)
    //static string EmbeddedShadersFolder = @"..\..\..\..\..\..\FlyleafLib\MediaFramework\MediaRenderer\Shaders";
    //static Assembly ASSEMBLY        = Assembly.GetExecutingAssembly();
    //static string   SHADERS_NS      = typeof(Renderer).Namespace + ".Shaders.";

    //static byte[] GetEmbeddedShaderResource(string shaderName)
    //{
    //    using (Stream stream = ASSEMBLY.GetManifestResourceStream(SHADERS_NS + shaderName + ".hlsl"))
    //    {
    //        byte[] bytes = new byte[stream.Length];
    //        stream.Read(bytes, 0, bytes.Length);

    //        return bytes;
    //    }
    //}

    const string PS_HEADER = @"
#pragma warning( disable: 3571 )
Texture2D		Texture1		: register(t0);
Texture2D		Texture2		: register(t1);
Texture2D		Texture3		: register(t2);
Texture2D		Texture4		: register(t3);

cbuffer         Config          : register(b0)
{
    int coefsIndex;
    int hdrmethod;

    float brightness;
    float contrast;

    float g_luminance;
    float g_toneP1;
    float g_toneP2;

    float texWidth;
};

SamplerState Sampler : IMMUTABLE
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = CLAMP;
    AddressV = CLAMP;
    AddressW = CLAMP;
    ComparisonFunc = NEVER;
    MinLOD = 0;
};

#if defined(YUV)
// YUV to RGB matrix coefficients
static const float4x4 coefs[] =
{
    // Limited -> Full
    {
        // BT2020 (srcBits = 10)
        { 1.16438353, 1.16438353, 1.16438353, 0 },
        { 0, -0.187326103, 2.14177227, 0 },
        { 1.67867422, -0.650424361, 0, 0 },
        { -0.915688038, 0.347458541, -1.14814520, 1 }
    },
    
        // BT709
    {
        { 1.16438341, 1.16438341, 1.16438341, 0 },
        { 0, -0.213248596, 2.11240149, 0 },
        { 1.79274082, -0.532909214, 0, 0 },
        { -0.972944975, 0.301482648, -1.13340211, 1 }
    },
        // BT601
    {
        { 1.16438341, 1.16438341, 1.16438341, 0 },
        { 0, -0.391762286, 2.01723194, 0 },
        { 1.59602666, -0.812967658, 0, 0 },
        { -0.874202192, 0.531667829, -1.08563077, 1 },
    }
};
#endif

#if defined(HDR)
// hdrmethod enum
static const int Aces       = 1;
static const int Hable      = 2;
static const int Reinhard   = 3;

// HDR to SDR color convert (Thanks to KODI community https://github.com/thexai/xbmc)
static const float ST2084_m1 = 2610.0f / (4096.0f * 4.0f);
static const float ST2084_m2 = (2523.0f / 4096.0f) * 128.0f;
static const float ST2084_c1 = 3424.0f / 4096.0f;
static const float ST2084_c2 = (2413.0f / 4096.0f) * 32.0f;
static const float ST2084_c3 = (2392.0f / 4096.0f) * 32.0f;

static const float4x4 bt2020tobt709color =
{
    { 1.6604f, -0.1245f, -0.0181f, 0 },
    { -0.5876f, 1.1329f, -0.10057f, 0 },
    { -0.07284f, -0.0083f, 1.1187f, 0 },
    { 0, 0, 0, 0 }
};

float3 inversePQ(float3 x)
{
    x = pow(max(x, 0.0f), 1.0f / ST2084_m2);
    x = max(x - ST2084_c1, 0.0f) / (ST2084_c2 - ST2084_c3 * x);
    x = pow(x, 1.0f / ST2084_m1);
    return x;
}

#if defined(HLG)
float3 inverseHLG(float3 x)
{
    const float B67_a = 0.17883277f;
    const float B67_b = 0.28466892f;
    const float B67_c = 0.55991073f;
    const float B67_inv_r2 = 4.0f;
    x = (x <= 0.5f) ? x * x * B67_inv_r2 : exp((x - B67_c) / B67_a) + B67_b;
    return x;
}

float3 tranferPQ(float3 x)
{
    x = pow(x / 1000.0f, ST2084_m1);
    x = (ST2084_c1 + ST2084_c2 * x) / (1.0f + ST2084_c3 * x);
    x = pow(x, ST2084_m2);
    return x;
}

#endif
float3 aces(float3 x)
{
    const float A = 2.51f;
    const float B = 0.03f;
    const float C = 2.43f;
    const float D = 0.59f;
    const float E = 0.14f;
    return (x * (A * x + B)) / (x * (C * x + D) + E);
}

float3 hable(float3 x)
{
    const float A = 0.15f;
    const float B = 0.5f;
    const float C = 0.1f;
    const float D = 0.2f;
    const float E = 0.02f;
    const float F = 0.3f;
    return ((x * (A * x + C * B) + D * E) / (x * (A * x + B) + D * F)) - E / F;
}

static const float3 bt709coefs = { 0.2126f, 1.0f - 0.2126f - 0.0722f, 0.0722f };
float reinhard(float x)
{
    return x * (1.0f + x / (g_toneP1 * g_toneP1)) / (1.0f + x);
}
#endif

struct PSInput
{
	float4 Position : SV_POSITION;
	float2 Texture  : TEXCOORD;
};

float4 main(PSInput input) : SV_TARGET
{
	float4 color;

	// Dynamic Sampling

";

    const string PS_FOOTER = @"
    
	// YUV to RGB
#if defined(YUV)
    color = mul(color, coefs[coefsIndex]);
#endif
    
    
	// HDR
#if defined(HDR)
    // BT2020 -> BT709
    color.rgb = pow(max(0.0, color.rgb), 2.4f);
    color.rgb = max(0.0, mul(color, bt2020tobt709color).rgb);
    color.rgb = pow(color.rgb, 1.0f / 2.2f);

    if (hdrmethod == Aces)
    {
        color.rgb = inversePQ(color.rgb);
        color.rgb *= (10000.0f / g_luminance) * (2.0f / g_toneP1);
        color.rgb = aces(color.rgb);
        color.rgb *= (1.24f / g_toneP1);
        color.rgb = pow(color.rgb, 0.27f);
    }
    else if (hdrmethod == Hable)
    {
        color.rgb = inversePQ(color.rgb);
        color.rgb *= g_toneP1;
        color.rgb = hable(color.rgb * g_toneP2) / hable(g_toneP2);
        color.rgb = pow(color.rgb, 1.0f / 2.2f);
    }
    else if (hdrmethod == Reinhard)
    {
        float luma = dot(color.rgb, bt709coefs);
        color.rgb *= reinhard(luma) / luma;
    }
    #if defined(HLG)
    // HLG
    color.rgb = inverseHLG(color.rgb);
    float3 ootf_2020 = float3(0.2627f, 0.6780f, 0.0593f);
    float ootf_ys = 2000.0f * dot(ootf_2020, color.rgb);
    color.rgb *= pow(ootf_ys, 0.2f);
    color.rgb = tranferPQ(color.rgb);
    #endif
#endif

	// Contrast / Brightness / Saturate / Hue
    color *= contrast * 2.0f;
    color += brightness - 0.5f;

	return color;
}
";

    const string VS = @"
cbuffer cBuf : register(b0)
{
    matrix mat;
}

struct VSInput
{
    float4 Position : POSITION;
    float2 Texture  : TEXCOORD;
};

struct PSInput
{
    float4 Position : SV_POSITION;
    float2 Texture  : TEXCOORD;
};

PSInput main(VSInput vsi)
{
    PSInput psi;

    psi.Position = mul(vsi.Position, mat);
    psi.Texture = vsi.Texture;

    return psi;
}
";
}
