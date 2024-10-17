// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Forms;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
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
        if (_transportControl is FrameworkElement frameEle)
        {
            var width = frameEle.ActualWidth;
            var height = frameEle.ActualHeight;

            // 测量 frameEle 相对于 MpvPlayer 的位置.
            var transform = frameEle.TransformToVisual(_overlayContainer).TransformPoint(new Point(0, 0));

            _transportControlTriggerRect = new Rect((int)transform.X, (int)transform.Y, (int)width, (int)height);
        }
    }

    private void ArrangeSubtitleSize()
    {
        if (_subtitlePresenter is null)
        {
            return;
        }

        const int initialWidth = 1280;
        const int initialFontSize = 28;
        const int initialPaddingLeft = 16;
        const int initialPaddingTop = 12;

        // 根据实际宽度调整字幕大小，线性增长，每增加100px，字体大小增加3，横向边距增加8，纵向边距增加6.
        var width = ActualWidth;
        _subtitlePresenter.FontSize = Math.Max(12, (double)(initialFontSize + ((width - initialWidth) / 100 * 1.5)));
        var horizontalPadding = Math.Max(8, initialPaddingLeft + ((width - initialWidth) / 100 * 2.5));
        var verticalPadding = Math.Max(8, initialPaddingTop + ((width - initialWidth) / 100 * 1));
        _subtitlePresenter.Padding = new Thickness(horizontalPadding, verticalPadding, horizontalPadding, verticalPadding);
    }

    private void CheckTransportControlVisibility(PointerRoutedEventArgs? args = default)
    {
        var isManual = SettingsToolkit.ReadLocalSetting(SettingNames.MTCBehavior, MTCBehavior.Automatic) == MTCBehavior.Manual;
        if (isManual
            || _transportControl is null
            || ViewModel.IsPlayerDataLoading
            || ViewModel.IsPlayerInitializing
            || ViewModel.IsExternalPlayer)
        {
            return;
        }

        DispatcherQueue.TryEnqueue(() =>
        {
            if (args is not null)
            {
                _lastPointerPoint = args.GetCurrentPoint(_overlayContainer).Position;
            }

            if (_lastPointerPoint is null)
            {
                SetTransportVisibility(false);
                return;
            }

            var isInStayArea = _transportControlTriggerRect.Contains(_lastPointerPoint ?? new(0, 0));
            var shouldShow = isInStayArea;
            if (!isInStayArea)
            {
                if (ViewModel.IsPaused)
                {
                    shouldShow = true;
                }
                else if (_mtcStayTime < 2)
                {
                    shouldShow = true;
                }
            }

            SetTransportVisibility(shouldShow);
        });
    }

    private void SetTransportVisibility(bool isVisible)
    {
        if (_transportControl is null)
        {
            return;
        }

        var visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        ViewModel?.CheckBottomProgressVisibility(!isVisible);
        if (_transportControl.Visibility == visibility)
        {
            return;
        }

        _transportControl.Visibility = visibility;
    }

    private void OnCursorTimerTick(object? sender, object e)
    {
        _cursorStayTime += 0.5;
        _mtcStayTime += 0.5;

        if (_lastPressedTime != null && DateTimeOffset.Now - _lastPressedTime > TimeSpan.FromSeconds(1) && !_isHolding)
        {
            _isHolding = true;
            ToggleTripleSpeed(true);
        }

        if (_lastRightArrowPressedTime != null && DateTimeOffset.Now - _lastRightArrowPressedTime > TimeSpan.FromSeconds(1) && !_isHolding)
        {
            _isHolding = true;
            ToggleTripleSpeed(true);
        }

        CheckTransportControlVisibility();
        if (_cursorStayTime >= 2
            && _transportControl is not null
            && _transportControl.Visibility == Visibility.Collapsed
            && !ViewModel.IsPaused
            && !ViewModel.IsPlayerDataLoading)
        {
            if (!_isCursorDisposed)
            {
                _overlayContainer.HideCursor();
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
            _overlayContainer.ShowCursor();
            _isCursorDisposed = false;
        }
    }

    private void OnInteractionControlContextRequested(UIElement sender, ContextRequestedEventArgs args)
    {
        if (_isHolding)
        {
            args.Handled = true;
            sender.ContextFlyout?.Hide();
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

    private void OnRootPointerCanceled(object sender, PointerRoutedEventArgs e)
    {
        _lastPressedTime = default;
        _lastRightArrowPressedTime = default;
        if (_isHolding)
        {
            ToggleTripleSpeed(false);
        }

        HandlePointerEvent(e, true);
    }

    private void OnRootPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
        {
            _lastPressedTime = DateTimeOffset.Now;
        }

        if (ViewModel is IslandPlayerViewModel)
        {
            var point = e.GetCurrentPoint((UIElement)sender);
            if (point.Properties.IsXButton1Pressed || point.Properties.IsXButton2Pressed)
            {
                if (ViewModel.IsFullScreen || ViewModel.IsFullWindow || ViewModel.IsCompactOverlay)
                {
                    e.Handled = true;
                    if (ViewModel.IsFullWindow)
                    {
                        ViewModel.ToggleFullWindowCommand.Execute(default);
                    }
                    else if (ViewModel.IsCompactOverlay)
                    {
                        ViewModel.ToggleCompactOverlayCommand.Execute(default);
                    }
                    else
                    {
                        ViewModel.ToggleFullScreenCommand.Execute(default);
                    }
                }
                else if (this.Get<AppViewModel>().ActivatedWindow is MainWindow)
                {
                    this.Get<NavigationViewModel>().Back();
                }
            }
        }
    }

    private void OnRootPointerExited(object sender, PointerRoutedEventArgs e)
    {
        _lastPressedTime = default;
        _lastRightArrowPressedTime = default;
        if (_isHolding)
        {
            ToggleTripleSpeed(false);
        }

        HandlePointerEvent(e, true);
    }

    private void OnRootPointerEntered(object sender, PointerRoutedEventArgs e)
        => HandlePointerEvent(e);

    private void OnRootPointerMoved(object sender, PointerRoutedEventArgs e)
        => HandlePointerEvent(e);

    private void OnRootPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        _lastPressedTime = default;
        _lastRightArrowPressedTime = default;
        if (_isHolding)
        {
            ToggleTripleSpeed(false);
        }
        else
        {
            if (_isDoubleTapped)
            {
                _isDoubleTapped = false;
                return;
            }

            var isManual = SettingsToolkit.ReadLocalSetting(SettingNames.MTCBehavior, MTCBehavior.Automatic) == MTCBehavior.Manual;
            if (isManual)
            {
                if (_transportControl is not null)
                {
                    SetTransportVisibility(_transportControl.Visibility == Visibility.Collapsed);
                }
            }
            else if (!_isHolding)
            {
                ViewModel?.TogglePlayPauseCommand.Execute(default);
            }
        }
    }
}
