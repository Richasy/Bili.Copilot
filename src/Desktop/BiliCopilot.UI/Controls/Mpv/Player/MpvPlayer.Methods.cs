// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml.Input;
using OpenTK.Graphics.OpenGL;

namespace BiliCopilot.UI.Controls.Mpv;

/// <summary>
/// 播放器.
/// </summary>
public sealed partial class MpvPlayer
{
    private void Render()
    {
        if (ViewModel?.Player?.Client?.IsInitialized is not true || ViewModel?.Player?.IsDisposed is true)
        {
            return;
        }

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        ViewModel.Player.RenderGL((int)(ActualWidth * _renderControl.ScaleX), (int)(ActualHeight * _renderControl.ScaleY), _renderControl.GetBufferHandle());
    }

    private void MeasureTransportTriggerRect()
    {
        if (TransportControls is FrameworkElement frameEle)
        {
            var width = frameEle.ActualWidth;
            var height = frameEle.ActualHeight;

            // 测量 frameEle 相对于 MpvPlayer 的位置.
            var extraWidth = (ActualWidth - width) / 2;
            var transform = frameEle.TransformToVisual(this).TransformPoint(new Point(0, 0));
            var actualX = transform.X - extraWidth >= 0 ? transform.X - extraWidth : 0;
            var actualY = transform.Y - height >= 0 ? transform.Y - height : 0;
            var actualWidth = width + (extraWidth * 2);
            var actualHeight = height * 3;
            if (actualWidth >= ActualWidth)
            {
                actualWidth = ActualWidth;
                actualX = 0;
            }

            if (actualHeight >= ActualHeight)
            {
                actualHeight = ActualHeight;
                actualY = 0;
            }

            _transportControlTriggerRect = new Rect(actualX, actualY, actualWidth, actualHeight);
        }
    }

    private void ArrangeSubtitleSize()
    {
        if (SubtitleControls is null)
        {
            return;
        }

        const int initialWidth = 1280;
        const int initialFontSize = 28;
        const int initialPaddingLeft = 20;
        const int initialPaddingTop = 8;

        // 根据实际宽度调整字幕大小，线性增长，每增加100px，字体大小增加3，横向边距增加8，纵向边距增加6.
        var width = ActualWidth;
        SubtitleControls.FontSize = Math.Max(12, (double)(initialFontSize + ((width - initialWidth) / 100 * 1.5)));
        var horizontalPadding = Math.Max(8, initialPaddingLeft + ((width - initialWidth) / 100 * 2.5));
        var verticalPadding = Math.Max(6, initialPaddingTop + ((width - initialWidth) / 100 * 1));
        SubtitleControls.Padding = new Thickness(horizontalPadding, verticalPadding, horizontalPadding, verticalPadding);
    }

    private void CheckTransportControlVisibility(PointerRoutedEventArgs args)
    {
        if (TransportControls is null || ViewModel.IsPlayerDataLoading || ViewModel.IsPlayerInitializing)
        {
            return;
        }

        DispatcherQueue.TryEnqueue(() =>
        {
            var point = args.GetCurrentPoint(this).Position;
            TransportControls.Visibility = _transportControlTriggerRect.Contains(point) || ViewModel.IsPaused
                ? Visibility.Visible
                : Visibility.Collapsed;
        });
    }
}
