// Copyright (c) Bili Copilot. All rights reserved.

// 源自 https://learn.microsoft.com/zh-cn/windows/apps/develop/win2d/custom-effects#custom-effects-in-c-using-computesharp
using ComputeSharp;
using ComputeSharp.D2D1;
using ComputeSharp.D2D1.WinUI;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.UI;
using System.Numerics;
using Windows.Graphics.Effects;
using Windows.UI;

namespace Richasy.WinUI.Share.Effects;

/// <summary>
/// 图像表面模糊的效果.
/// </summary>
public sealed partial class BlurCanvasEffect : CanvasEffect
{
    private static readonly CanvasEffectNode<BorderEffect> _borderNode = new();
    private static readonly CanvasEffectNode<ScaleEffect> _scaleNode = new();
    private static readonly CanvasEffectNode<GaussianBlurEffect> _gaussianBlurNode = new();
    private static readonly CanvasEffectNode<TintEffect> _tintNode = new();
    private static readonly CanvasEffectNode<PixelShaderEffect<CrossFadeShader>> _crossFadeNode = new();
    private static readonly CanvasEffectNode<PixelShaderEffect<NoiseShader>> _noiseNode = new();
    private static readonly CanvasEffectNode<BlendEffect> _blendNode = new();

    private IGraphicsEffectSource _source;
    private float _blurAmount;
    private Color _tintColor = Colors.White;
    private int _crossfadeVerticalOffset;
    private int _crossfadeVerticalLength;
    private Rect _sourceRectangle;
    private Rect _destinationRectangle;

    /// <summary>
    /// 图像源.
    /// </summary>
    public IGraphicsEffectSource Source
    {
        get => _source;
        set => SetPropertyAndInvalidateEffectGraph(ref _source, value);
    }

    /// <summary>
    /// 模糊强度.
    /// </summary>
    public float BlurAmount
    {
        get => _blurAmount;
        set => SetPropertyAndInvalidateEffectGraph(ref _blurAmount, value);
    }

    /// <summary>
    /// 模糊表面颜色.
    /// </summary>
    public Color TintColor
    {
        get => _tintColor;
        set => SetPropertyAndInvalidateEffectGraph(ref _tintColor, value);
    }

    /// <summary>
    /// 交错偏移.
    /// </summary>
    public int CrossfadeVerticalOffset
    {
        get => _crossfadeVerticalOffset;
        set => SetPropertyAndInvalidateEffectGraph(ref _crossfadeVerticalOffset, value);
    }

    /// <summary>
    /// 交错长度.
    /// </summary>
    public int CrossfadeVerticalLength
    {
        get => _crossfadeVerticalLength;
        set => SetPropertyAndInvalidateEffectGraph(ref _crossfadeVerticalLength, value);
    }

    /// <summary>
    /// 源矩形.
    /// </summary>
    public Rect SourceRectangle
    {
        get => _sourceRectangle;
        set => SetPropertyAndInvalidateEffectGraph(ref _sourceRectangle, value);
    }

    /// <summary>
    /// 目标矩形.
    /// </summary>
    public Rect DestinationRectangle
    {
        get => _destinationRectangle;
        set => SetPropertyAndInvalidateEffectGraph(ref _destinationRectangle, value);
    }

    /// <inheritdoc/>
    protected override void BuildEffectGraph(CanvasEffectGraph effectGraph)
    {
        BorderEffect borderEffect = new() { ExtendY = Microsoft.Graphics.Canvas.CanvasEdgeBehavior.Mirror };
        ScaleEffect scaleEffect = new();
        GaussianBlurEffect gaussianBlurEffect = new() { Optimization = EffectOptimization.Speed, BorderMode = EffectBorderMode.Hard };
        TintEffect tintEffect = new();
        BlendEffect blendEffect = new() { Mode = BlendEffectMode.Luminosity };
        PremultiplyEffect noisePremultiplyEffect = new();
        PremultiplyEffect finalPremultiplyEffect = new();
        UnPremultiplyEffect unPremultiplyEffect1 = new();
        UnPremultiplyEffect unPremultiplyEffect2 = new();
        PixelShaderEffect<CrossFadeShader> crossFadeEffect = new();
        PixelShaderEffect<NoiseShader> noiseEffect = new();

        scaleEffect.Source = borderEffect;
        gaussianBlurEffect.Source = scaleEffect;
        tintEffect.Source = gaussianBlurEffect;
        noisePremultiplyEffect.Source = noiseEffect;
        blendEffect.Foreground = noisePremultiplyEffect;
        blendEffect.Background = tintEffect;
        unPremultiplyEffect1.Source = scaleEffect;
        unPremultiplyEffect2.Source = blendEffect;
        crossFadeEffect.Sources[0] = unPremultiplyEffect1;
        crossFadeEffect.Sources[1] = unPremultiplyEffect2;
        finalPremultiplyEffect.Source = crossFadeEffect;

        effectGraph.RegisterNode(_noiseNode, noiseEffect);
        effectGraph.RegisterNode(_borderNode, borderEffect);
        effectGraph.RegisterNode(_scaleNode, scaleEffect);
        effectGraph.RegisterNode(_gaussianBlurNode, gaussianBlurEffect);
        effectGraph.RegisterNode(_tintNode, tintEffect);
        effectGraph.RegisterNode(_blendNode, blendEffect);
        effectGraph.RegisterNode(_crossFadeNode, crossFadeEffect);
        effectGraph.RegisterNode(finalPremultiplyEffect);
        effectGraph.RegisterNode(unPremultiplyEffect1);
        effectGraph.RegisterNode(unPremultiplyEffect2);
    }

    /// <inheritdoc/>
    protected override void ConfigureEffectGraph(CanvasEffectGraph effectGraph)
    {
        effectGraph.GetNode(_noiseNode).ConstantBuffer = new NoiseShader(0.1f);
        effectGraph.GetNode(_borderNode).Source = _source;
        effectGraph.GetNode(_scaleNode).Scale = new Vector2((float)_destinationRectangle.Width / (float)_sourceRectangle.Width);
        effectGraph.GetNode(_gaussianBlurNode).BlurAmount = _blurAmount;
        effectGraph.GetNode(_tintNode).Color = _tintColor;
        effectGraph.GetNode(_crossFadeNode).ConstantBuffer = new CrossFadeShader(_crossfadeVerticalOffset, _crossfadeVerticalLength);
        effectGraph.SetOutputNode(_crossFadeNode);
    }
}

/// <summary>
/// 噪声着色器.
/// </summary>
[D2DInputCount(0)]
[D2DRequiresScenePosition]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader40)]
[D2DGeneratedPixelShaderDescriptor]
public readonly partial struct NoiseShader(float amount) : ID2D1PixelShader
{
    /// <inheritdoc/>
    public float4 Execute()
    {
        var position = (int2)D2D.GetScenePosition().XY;
        var hash = Hlsl.Frac(Hlsl.Sin(Hlsl.Dot(position, new float2(41, 289))) * 45758.5453f);
        var alpha = Hlsl.Lerp(0, amount, hash);
        return new(0, 0, 0, alpha);
    }
}

/// <summary>
/// 交错着色器.
/// </summary>
[D2DInputCount(2)]
[D2DInputSimple(0)]
[D2DInputSimple(1)]
[D2DInputDescription(0, D2D1Filter.MinMagMipPoint)]
[D2DInputDescription(1, D2D1Filter.MinMagMipPoint)]
[D2DPixelOptions(D2D1PixelOptions.TrivialSampling)]
[D2DRequiresScenePosition]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader40)]
[D2DGeneratedPixelShaderDescriptor]
public readonly partial struct CrossFadeShader : ID2D1PixelShader
{
    private readonly int _offsetStartY;
    private readonly int _offsetLengthY;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrossFadeShader"/> struct.
    /// </summary>
    public CrossFadeShader(int offsetStartY, int offsetLengthY)
    {
        _offsetStartY = offsetStartY;
        _offsetLengthY = offsetLengthY;
    }

    /// <inheritdoc/>
    public float4 Execute()
    {
        var offsetY = (int)D2D.GetScenePosition().Y;
        var factor = Hlsl.Saturate((offsetY - _offsetStartY) / (float)_offsetLengthY);
        var easing = Hlsl.Sin(factor * 1.57f);
        var blend = Hlsl.Lerp(D2D.GetInput(0).XYZ, D2D.GetInput(1).XYZ, easing);

        return new(blend, 1);
    }
}
