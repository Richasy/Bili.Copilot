// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.Graphics.Canvas;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 视频封面图片.
/// </summary>
public sealed class VideoCoverImage : ImageExBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoCoverImage"/> class.
    /// </summary>
    public VideoCoverImage()
    {
        DecodeWidth = 336;
        DecodeHeight = 189;
    }

    /// <inheritdoc/>
    protected override void DrawImage(CanvasBitmap canvasBitmap)
    {
        var bitmapWidth = canvasBitmap.SizeInPixels.Width;
        var bitmapHeight = canvasBitmap.SizeInPixels.Height;
        var sourceRect = new Rect(0, 0, bitmapWidth, bitmapHeight);
        var destRect = new Rect(0, 0, DecodeWidth, DecodeHeight);
        var cropRect = GetCenterCropRect(destRect, sourceRect);
        using var drawingSession = CanvasImageSource.CreateDrawingSession(ClearColor);
        drawingSession.DrawImage(canvasBitmap, destRect, cropRect);
    }
}
