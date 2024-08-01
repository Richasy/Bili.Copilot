// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Richasy.WinUI.Share.Base;
using Richasy.WinUI.Share.Effects;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// PGC 封面图片.
/// </summary>
public sealed class PgcCoverImage : ImageExBase
{
    /// <summary>
    /// <see cref="BlurRatio"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty BlurRatioProperty =
        DependencyProperty.Register(nameof(BlurRatio), typeof(double), typeof(PgcCoverImage), new PropertyMetadata(0.33));

    private static readonly BlurCanvasEffect _blurEffect = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="PgcCoverImage"/> class.
    /// </summary>
    public PgcCoverImage()
    {
        DecodeWidth = 320;
        DecodeHeight = 460;
    }

    /// <summary>
    /// 底部模糊比例.
    /// </summary>
    public double BlurRatio
    {
        get => (double)GetValue(BlurRatioProperty);
        set => SetValue(BlurRatioProperty, value);
    }

    /// <inheritdoc/>
    protected override void DrawImage(CanvasBitmap canvasBitmap)
    {
        var width = canvasBitmap.Size.Width;
        var height = canvasBitmap.Size.Height;
        var destRect = new Rect(0, 0, DecodeWidth, DecodeHeight);
        var sourceRect = new Rect(0, 0, width, height);
        DrawBlurImage(canvasBitmap, destRect, sourceRect);
    }

    private void DrawBlurImage(
        CanvasBitmap canvasBitmap,
        Rect destinationRect,
        Rect sourceRect)
    {
        var crossfadeOffset = CanvasImageSource.ConvertDipsToPixels((float)(DecodeHeight - (DecodeHeight * BlurRatio)), CanvasDpiRounding.Round);
        var crossfadeLength = CanvasImageSource.ConvertDipsToPixels(60, CanvasDpiRounding.Round);

        _blurEffect.Source = canvasBitmap;
        _blurEffect.BlurAmount = CanvasImageSource.ConvertDipsToPixels(20, CanvasDpiRounding.Round);
        _blurEffect.CrossfadeVerticalOffset = crossfadeOffset;
        _blurEffect.CrossfadeVerticalLength = crossfadeLength;
        _blurEffect.SourceRectangle = sourceRect;
        _blurEffect.DestinationRectangle = destinationRect;

        using var ds = CanvasImageSource.CreateDrawingSession(ClearColor);
        ds.DrawImage(_blurEffect, destinationRect, destinationRect);
    }
}
