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

    private void CheckTransportControlVisibility(PointerRoutedEventArgs args)
    {
        if (TransportControls is not FrameworkElement frameEle)
        {
            return;
        }

        DispatcherQueue.TryEnqueue(() =>
        {
            var point = args.GetCurrentPoint(this).Position;
            frameEle.Visibility = _transportControlTriggerRect.Contains(point)
                ? Visibility.Visible
                : Visibility.Collapsed;
        });
    }

    private void CheckCursorVisibility()
    {
        if (_cursorStayTime > 1.5
            && _isPointerStay
            && TransportControls is not null
            && TransportControls.Visibility != Visibility.Visible
            && !ViewModel.IsPaused)
        {
            ProtectedCursor?.Dispose();
            _cursorStayTime = 0;
        }
    }
}
