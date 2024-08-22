// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 原始图片.
/// </summary>
public sealed class SourceImage : ImageExBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoCoverImage"/> class.
    /// </summary>
    public SourceImage()
    {
    }

    /// <inheritdoc/>
    protected override void DrawImage(CanvasBitmap canvasBitmap)
    {
        var bitmapWidth = canvasBitmap.SizeInPixels.Width;
        var bitmapHeight = canvasBitmap.SizeInPixels.Height;
        DecodeWidth = bitmapWidth / 2;
        DecodeHeight = bitmapHeight / 2;
        var sourceRect = new Rect(0, 0, bitmapWidth, bitmapHeight);
        var destRect = new Rect(0, 0, DecodeWidth, DecodeHeight);
        var cropRect = GetCenterCropRect(destRect, sourceRect);
        using var drawingSession = CanvasImageSource.CreateDrawingSession(ClearColor);
        drawingSession.DrawImage(canvasBitmap, destRect, sourceRect);
    }
}
