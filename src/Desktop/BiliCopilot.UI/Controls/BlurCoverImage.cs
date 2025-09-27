// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Extensions;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Richasy.WinUI.Share.Effects;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls;

public sealed partial class BlurCoverImage : ImageExBase
{
    /// <summary>
    /// <see cref="Offset"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty OffsetProperty =
        DependencyProperty.Register(nameof(Offset), typeof(double), typeof(BlurCoverImage), new PropertyMetadata(0.31d));

    public bool ShouldAppendOffsetHeight { get; set; } = true;

    private static readonly BlurCanvasEffect _blurEffect = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="BlurCoverImage"/> class.
    /// </summary>
    public BlurCoverImage()
    {
        DecodeWidth = 320;
        DecodeHeight = 420;
    }

    /// <summary>
    /// 模糊效果偏移量.
    /// </summary>
    public double Offset
    {
        get => (double)GetValue(OffsetProperty);
        set => SetValue(OffsetProperty, value);
    }

    protected override HttpClient? GetCustomHttpClient()
        => InternalHttpExtensions.ImageClient;

    /// <inheritdoc/>
    protected override void DrawImage(CanvasBitmap canvasBitmap)
    {
        var width = canvasBitmap.Size.Width;
        var height = canvasBitmap.Size.Height;
        var aspectRatio = width / height;
        var actualHeight = DecodeWidth / aspectRatio;
        if (Offset > 0 && ShouldAppendOffsetHeight)
        {
            actualHeight += actualHeight * Offset;
        }

        if (Math.Abs(DecodeHeight - actualHeight) > 1)
        {
            DecodeHeight = Math.Round(actualHeight);
            CanvasImageSource = new Microsoft.Graphics.Canvas.UI.Xaml.CanvasImageSource(
                resourceCreator: CanvasDevice.GetSharedDevice(),
                width: (float)DecodeWidth,
                height: (float)DecodeHeight,
                dpi: 96,
                CanvasAlphaMode.Premultiplied);
        }

        var destRect = new Rect(0, 0, DecodeWidth, DecodeHeight);
        var sourceRect = new Rect(0, 0, width, height);
        DrawBlurImage(canvasBitmap, destRect, sourceRect);
    }

    protected override string GetCacheSubFolder()
        => "ImageCache";

    private void DrawBlurImage(
        CanvasBitmap canvasBitmap,
        Rect destinationRect,
        Rect sourceRect)
    {
        if (destinationRect.Width <= 0 || destinationRect.Height <= 0)
        {
            return;
        }

        var crossfadeOffset = CanvasImageSource!.ConvertDipsToPixels((float)(DecodeHeight - (DecodeHeight * Offset)), CanvasDpiRounding.Round);
        var crossfadeLength = CanvasImageSource.ConvertDipsToPixels(68, CanvasDpiRounding.Round);

        _blurEffect.Source = canvasBitmap;
        _blurEffect.BlurAmount = CanvasImageSource.ConvertDipsToPixels(28, CanvasDpiRounding.Round);
        _blurEffect.CrossfadeVerticalOffset = crossfadeOffset;
        _blurEffect.CrossfadeVerticalLength = crossfadeLength;
        _blurEffect.SourceRectangle = sourceRect;
        _blurEffect.DestinationRectangle = destinationRect;
        using var ds = CanvasImageSource!.CreateDrawingSession(ClearColor);
        ds.DrawImage(_blurEffect, destinationRect, destinationRect);
    }
}
