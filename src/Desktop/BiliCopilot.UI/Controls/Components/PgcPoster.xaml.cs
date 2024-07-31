// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Richasy.WinUI.Share.Base;
using Windows.UI;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// PGC海报控件.
/// </summary>
public sealed partial class PgcPoster : CardImageBase
{
    private const uint DefaultWidth = 256;
    private const uint DefaultHeight = 380;
    private const double DefaultCrossfadeVerticalOffsetRate = 0.66;
    private static readonly VerticalBlurCrossFadeEffect _crossFadeEffect = new();
    private CanvasImageSource? _canvasImageSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="PgcPoster"/> class.
    /// </summary>
    public PgcPoster()
    {
        InitializeComponent();
    }

    /// <inheritdoc/>
    protected override void OnCreateResources(CanvasDevice device)
    {
        if (_canvasImageSource is CanvasImageSource imageSource)
        {
            BackgroundBrush.ImageSource = default;
            imageSource.Recreate(device);
        }
        else
        {
            _canvasImageSource = new CanvasImageSource(
                resourceCreator: device,
                width: DefaultWidth,
                height: DefaultHeight,
                dpi: 96,
                CanvasAlphaMode.Premultiplied);
        }
    }

    /// <inheritdoc/>
    protected override void OnDrawImage(CanvasBitmap canvasBitmap)
    {
        var width = canvasBitmap.Size.Width;
        var height = canvasBitmap.Size.Height;
        var blurDestinationRect = new Rect(0, 0, DefaultWidth, DefaultHeight);
        var sourceRect = new Rect(0, 0, width, height);
        var color = Colors.Gray;

        DrawBlurImage(canvasBitmap, color, blurDestinationRect, sourceRect, DefaultCrossfadeVerticalOffsetRate);
    }

    /// <inheritdoc/>
    protected override void OnImageUriChangedAsync(Uri uri)
    {
        BackgroundBrush.ImageSource = default;
        base.OnImageUriChangedAsync(uri);
    }

    private void DrawBlurImage(
        CanvasBitmap canvasBitmap,
        Color backgroundColor,
        Rect destinationRect,
        Rect sourceRect,
        double crossfadeVerticalOffsetRate)
    {
        var blurAmount = 12;
        var crossfadeOffset = _canvasImageSource.ConvertDipsToPixels((float)(DefaultHeight * crossfadeVerticalOffsetRate), CanvasDpiRounding.Round);
        var crossfadeLength = _canvasImageSource.ConvertDipsToPixels(55, CanvasDpiRounding.Round);

        _crossFadeEffect.Source = canvasBitmap;
        _crossFadeEffect.BlurAmount = _canvasImageSource.ConvertDipsToPixels(blurAmount, CanvasDpiRounding.Round);
        _crossFadeEffect.CrossfadeVerticalOffset = crossfadeOffset;
        _crossFadeEffect.CrossfadeVerticalLength = crossfadeLength;
        _crossFadeEffect.Theme = App.Current.RequestedTheme;
        _crossFadeEffect.SourceRectangle = sourceRect;
        _crossFadeEffect.DestinationRectangle = destinationRect;
        _crossFadeEffect.BlurMainImage = false;

        using var ds = _canvasImageSource.CreateDrawingSession(backgroundColor);
        ds.DrawImage(_crossFadeEffect, destinationRect, destinationRect);
        BackgroundBrush.ImageSource = _canvasImageSource;
    }
}
