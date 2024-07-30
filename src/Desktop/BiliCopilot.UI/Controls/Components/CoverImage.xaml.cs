// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 封面图片.
/// </summary>
public sealed partial class CoverImage : CardImageBase
{
    private const uint DefaultWidth = 400;
    private const uint DefaultHeight = 240;
    private CanvasImageSource? _canvasImageSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoverImage"/> class.
    /// </summary>
    public CoverImage()
    {
        InitializeComponent();
    }

    /// <inheritdoc/>
    protected override void OnCreateResources(CanvasDevice device)
    {
        if (_canvasImageSource is CanvasImageSource canvasImageSource)
        {
            BackgroundBrush.ImageSource = default;
            canvasImageSource.Recreate(device);
        }
        else
        {
            _canvasImageSource = new CanvasImageSource(
                resourceCreator: device,
                width: DefaultWidth,
                height: DefaultHeight,
                dpi: 96,
                CanvasAlphaMode.Ignore);
        }
    }

    /// <inheritdoc/>
    protected override void OnDrawImage(CanvasBitmap canvasBitmap)
    {
        var widthInPixel = canvasBitmap.SizeInPixels.Width;
        var heightInPixel = canvasBitmap.SizeInPixels.Height;
        var imageRatio = widthInPixel / (float)heightInPixel;
        var aspectRatio = 16f / 9f;
        var diff = Math.Abs(imageRatio - aspectRatio);
        var threshold = 0.005f;
        var imageFillContainer = Math.Abs(diff / imageRatio) < threshold || Math.Abs(diff / aspectRatio) < threshold;

        using var drawingSession = _canvasImageSource.CreateDrawingSession(Colors.Black);
        if (imageFillContainer)
        {
            drawingSession.DrawImage(
                canvasBitmap,
                destinationRectangle: new Rect(0, 0, DefaultWidth, DefaultHeight),
                sourceRectangle: new Rect(0, 0, widthInPixel, heightInPixel));
        }
        else
        {
            var backgroundDestinationRect = new Rect(0, 0, DefaultWidth, DefaultHeight);
            var backgroundSourceRect = new Rect(0, 0, widthInPixel, heightInPixel);
            var clipRect = GetClipRect(backgroundDestinationRect, backgroundSourceRect);

            var scaledWidthInPixels = DefaultWidth;
            var scaledHeightInPixels = DefaultWidth / widthInPixel * heightInPixel;

            if (scaledHeightInPixels > DefaultHeight)
            {
                scaledWidthInPixels = DefaultHeight / heightInPixel * widthInPixel;
                scaledHeightInPixels = DefaultHeight;
            }

            var horizontalOverlayOffset = Math.Max(0, DefaultWidth - scaledWidthInPixels) / 2;
            var verticalOverlayOffset = Math.Max(0, DefaultHeight - scaledHeightInPixels) / 2;
            drawingSession.DrawImage(
                canvasBitmap,
                destinationRectangle: new Rect(horizontalOverlayOffset, verticalOverlayOffset, scaledWidthInPixels, scaledHeightInPixels),
                sourceRectangle: new Rect(0, 0, widthInPixel, heightInPixel));
        }

        BackgroundBrush.ImageSource = _canvasImageSource;
    }

    /// <inheritdoc/>
    protected override void OnImageUriChangedAsync(Uri uri)
    {
        BackgroundBrush.ImageSource = default;
        base.OnImageUriChangedAsync(uri);
    }
}
