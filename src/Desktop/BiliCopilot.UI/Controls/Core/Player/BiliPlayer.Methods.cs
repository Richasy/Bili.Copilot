// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Input;

namespace BiliCopilot.UI.Controls.Core;

/// <summary>
/// 播放器.
/// </summary>
public sealed partial class BiliPlayer
{
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
        const int initialPaddingLeft = 16;
        const int initialPaddingTop = 12;

        // 根据实际宽度调整字幕大小，线性增长，每增加100px，字体大小增加3，横向边距增加8，纵向边距增加6.
        var width = ActualWidth;
        SubtitleControls.FontSize = Math.Max(12, (double)(initialFontSize + ((width - initialWidth) / 100 * 1.5)));
        var horizontalPadding = Math.Max(8, initialPaddingLeft + ((width - initialWidth) / 100 * 2.5));
        var verticalPadding = Math.Max(8, initialPaddingTop + ((width - initialWidth) / 100 * 1));
        SubtitleControls.Padding = new Thickness(horizontalPadding, verticalPadding, horizontalPadding, verticalPadding);
    }

    private void CheckTransportControlVisibility(PointerRoutedEventArgs args)
    {
        if (TransportControls is null || ViewModel.IsPlayerDataLoading || ViewModel.IsPlayerInitializing || ViewModel.IsExternalPlayer)
        {
            return;
        }

        DispatcherQueue.TryEnqueue(() =>
        {
            var point = args.GetCurrentPoint(this).Position;
            TransportControls.Visibility = _transportControlTriggerRect.Contains(point) || ViewModel.IsPaused
                ? Visibility.Visible
                : Visibility.Collapsed;
            ViewModel.CheckBottomProgressVisibility(TransportControls.Visibility == Visibility.Collapsed);
        });
    }

    private void OnCursorTimerTick(object? sender, object e)
    {
        _cursorStayTime += 0.5;

        if (_cursorStayTime >= 2
            && TransportControls is not null
            && TransportControls.Visibility == Visibility.Collapsed
            && !ViewModel.IsPaused
            && !ViewModel.IsPlayerDataLoading)
        {
            if (!_isCursorDisposed)
            {
                ProtectedCursor?.Dispose();
                _isCursorDisposed = true;
            }

            _cursorStayTime = 0;
        }
    }

    private void RestoreCursor()
    {
        _cursorStayTime = 0;
        if (_isCursorDisposed)
        {
            ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
            _isCursorDisposed = false;
        }
    }
}
