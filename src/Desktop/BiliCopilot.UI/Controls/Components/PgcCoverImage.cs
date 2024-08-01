// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Richasy.WinUI.Share.Base;

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

    private static readonly VerticalBlurCrossFadeEffect _crossFadeEffect = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="PgcCoverImage"/> class.
    /// </summary>
    public PgcCoverImage()
    {
        DecodeWidth = 256;
        DecodeHeight = 380;
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
        var crossfadeLength = CanvasImageSource.ConvertDipsToPixels(55, CanvasDpiRounding.Round);

        _crossFadeEffect.Source = canvasBitmap;
        _crossFadeEffect.BlurAmount = CanvasImageSource.ConvertDipsToPixels(12, CanvasDpiRounding.Round);
        _crossFadeEffect.CrossfadeVerticalOffset = crossfadeOffset;
        _crossFadeEffect.CrossfadeVerticalLength = crossfadeLength;
        _crossFadeEffect.Theme = App.Current.RequestedTheme;
        _crossFadeEffect.SourceRectangle = sourceRect;
        _crossFadeEffect.DestinationRectangle = destinationRect;
        _crossFadeEffect.BlurMainImage = false;

        using var ds = CanvasImageSource.CreateDrawingSession(ClearColor);
        ds.DrawImage(_crossFadeEffect, destinationRect, destinationRect);
    }
}
