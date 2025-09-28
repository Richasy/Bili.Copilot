// Copyright (c) Bili Copilot. All rights reserved.

using ComputeSharp.D2D1.WinUI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Graphics.Effects;

namespace BiliCopilot.UI.Extensions;

internal sealed partial class BasicScaleEffect : CanvasEffect
{
    /// <summary>
    /// The <see cref="BorderEffect"/> node, which receives the input image as input.
    /// </summary>
    private static readonly CanvasEffectNode<BorderEffect> BorderNode = new();

    /// <summary>
    /// The <see cref="ScaleEffect"/> node, to fit the image before blurring.
    /// </summary>
    private static readonly CanvasEffectNode<ScaleEffect> ScaleNode = new();

    /// <summary>
    /// The source <see cref="IGraphicsEffectSource"/> instance.
    /// </summary>
    private IGraphicsEffectSource _source;

    /// <summary>
    /// The source rectangle of the effect (ie. the area of the source that will be read from).
    /// </summary>
    private Rect _sourceRectangle;

    /// <summary>
    /// The destination rectangle (ie. the area that will be displayed on screen).
    /// </summary>
    private Rect _destinationRectangle;

    /// <summary>
    /// Gets or sets the source of the effect.
    /// </summary>
    public IGraphicsEffectSource Source
    {
        get => _source;
        set => SetPropertyAndInvalidateEffectGraph(ref _source, value);
    }

    /// <summary>
    /// Gets or sets the source rectangle of the effect (ie. the area of the source that will be read from).
    /// </summary>
    public Rect SourceRectangle
    {
        get => _sourceRectangle;
        set => SetPropertyAndInvalidateEffectGraph(ref _sourceRectangle, value);
    }

    /// <summary>
    /// Gets or sets the destination rectangle (ie. the area that will be displayed on screen).
    /// </summary>
    public Rect DestinationRectangle
    {
        get => _destinationRectangle;
        set => SetPropertyAndInvalidateEffectGraph(ref _destinationRectangle, value);
    }

    /// <inheritdoc/>
    protected override void BuildEffectGraph(CanvasEffectGraph effectGraph)
    {
        BorderEffect borderEffect = new() { ExtendY = CanvasEdgeBehavior.Mirror };
        ScaleEffect scaleEffect = new() { Source = borderEffect };
        effectGraph.RegisterNode(BorderNode, borderEffect);
        effectGraph.RegisterNode(ScaleNode, scaleEffect);
    }

    /// <inheritdoc/>
    protected override void ConfigureEffectGraph(CanvasEffectGraph effectGraph)
    {
        // Configure the basic properties
        effectGraph.GetNode(BorderNode).Source = _source;
        effectGraph.GetNode(ScaleNode).Scale = new Vector2((float)(_destinationRectangle.Width / _sourceRectangle.Width));
        effectGraph.SetOutputNode(ScaleNode);
    }
}
