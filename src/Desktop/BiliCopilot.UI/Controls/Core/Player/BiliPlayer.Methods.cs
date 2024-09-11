// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
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
            SetTransportVisibility(_transportControlTriggerRect.Contains(point) || ViewModel.IsPaused);
        });
    }

    private void SetTransportVisibility(bool isVisible)
    {
        if (TransportControls is null)
        {
            return;
        }

        TransportControls.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        ViewModel.CheckBottomProgressVisibility(TransportControls.Visibility == Visibility.Collapsed);
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

    private void OnInteractionControlManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        _manipulationVolume = 0;
        _manipulationProgress = 0;
        _manipulationDeltaX = 0;
        _manipulationDeltaY = 0;
        _manipulationType = PlayerManipulationType.None;

        if (_manipulationBeforeIsPlay && ViewModel.IsPaused && !ViewModel.IsBuffering)
        {
            ViewModel.TogglePlayPauseCommand.Execute(default);
        }

        _manipulationBeforeIsPlay = false;
    }

    private void OnInteractionControlManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        if (ViewModel.IsBuffering)
        {
            return;
        }

        _manipulationDeltaX += e.Delta.Translation.X;
        _manipulationDeltaY -= e.Delta.Translation.Y;
        if (Math.Abs(_manipulationDeltaX) > 15 || Math.Abs(_manipulationDeltaY) > 15)
        {
            if (_manipulationType == PlayerManipulationType.None)
            {
                var isVolume = Math.Abs(_manipulationDeltaY) > Math.Abs(_manipulationDeltaX);
                _manipulationType = isVolume ? PlayerManipulationType.Volume : PlayerManipulationType.Progress;
            }

            if (_manipulationType == PlayerManipulationType.Volume)
            {
                var volume = _manipulationVolume + (_manipulationDeltaY / 2.0);
                if (volume > 100)
                {
                    volume = 100;
                }
                else if (volume < 0)
                {
                    volume = 0;
                }

                ViewModel.SetVolumeCommand.Execute(Convert.ToInt32(volume));
            }
            else
            {
                var progress = _manipulationProgress + (_manipulationDeltaX * _manipulationUnitLength);
                if (progress > ViewModel.Duration)
                {
                    progress = ViewModel.Duration;
                }
                else if (progress < 0)
                {
                    progress = 0;
                }

                ViewModel.SeekCommand.Execute(progress);
            }
        }
    }

    private void OnInteractionControlManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
    {
        if (ViewModel.IsPlayerDataLoading || ViewModel.IsBuffering || ViewModel.IsFailed)
        {
            return;
        }

        _manipulationProgress = ViewModel.Position;
        _manipulationVolume = ViewModel.Volume;

        // 点击事件先于手势事件，当该事件触发时，可能已经切换了播放状态.
        _manipulationBeforeIsPlay = ViewModel.IsPaused;
        if (ViewModel.Duration > 0)
        {
            // 获取单位像素对应的时长
            var unit = ViewModel.Duration / ActualWidth;
            _manipulationUnitLength = unit / 1.5;
        }
    }

    private void CheckPointerType(Pointer pointer)
        => _isTouch = pointer.PointerDeviceType == PointerDeviceType.Touch;
}
