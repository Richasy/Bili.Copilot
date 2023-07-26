// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Bili.Copilot.App.Controls.Danmaku;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Live;
using Bili.Copilot.Models.Data.Player;
using Bili.Copilot.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 哔哩播放器覆盖层.
/// </summary>
public partial class BiliPlayerOverlay
{
    /// <inheritdoc/>
    protected override void OnPointerEntered(PointerRoutedEventArgs e)
    {
        IsPointerStay = true;
        ShowAndResetMediaTransport(e.Pointer.PointerDeviceType == PointerDeviceType.Mouse);
    }

    /// <inheritdoc/>
    protected override void OnPointerMoved(PointerRoutedEventArgs e)
    {
        IsPointerStay = true;
        ShowAndResetMediaTransport(e.Pointer.PointerDeviceType == PointerDeviceType.Mouse);
    }

    /// <inheritdoc/>
    protected override void OnPointerExited(PointerRoutedEventArgs e)
    {
        IsPointerStay = false;
        HideAndResetMediaTransport();
    }

    /// <inheritdoc/>
    protected override void OnPointerCanceled(PointerRoutedEventArgs e)
        => HideAndResetMediaTransport();

    /// <inheritdoc/>
    protected override void OnPointerCaptureLost(PointerRoutedEventArgs e)
        => HideAndResetMediaTransport();

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _unitTimer.Stop();
        ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
        _interactionControl.Tapped -= OnInteractionControlTapped;
        _interactionControl.DoubleTapped -= OnInteractionControlDoubleTapped;
        _interactionControl.ManipulationStarted -= OnInteractionControlManipulationStarted;
        _interactionControl.ManipulationDelta -= OnInteractionControlManipulationDelta;
        _interactionControl.ManipulationCompleted -= OnInteractionControlManipulationCompleted;
        _interactionControl.PointerPressed -= OnInteractionControlPointerPressed;
        _interactionControl.PointerMoved -= OnInteractionControlPointerMoved;
        _interactionControl.PointerReleased -= OnInteractionControlPointerReleased;
        _interactionControl.PointerCanceled -= OnInteractionControlPointerCanceled;
        _gestureRecognizer.Holding -= OnGestureRecognizerHolding;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _unitTimer.Start();
        _isForceHiddenTransportControls = true;
        HideAndResetMediaTransport();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        CheckDanmakuZoom();
        ResizeSubtitle();
        ViewModel.DanmakuViewModel.CanShowDanmaku = e.NewSize.Width >= 480;
    }

    private void OnDanmakuViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.DanmakuViewModel.DanmakuZoom))
        {
            CheckDanmakuZoom();
        }
        else if (e.PropertyName == nameof(ViewModel.DanmakuViewModel.DanmakuArea))
        {
            _danmakuView.DanmakuArea = ViewModel.DanmakuViewModel.DanmakuArea;
        }
        else if (e.PropertyName == nameof(ViewModel.DanmakuViewModel.DanmakuSpeed))
        {
            _danmakuView.DanmakuDuration = Convert.ToInt32((2.1 - ViewModel.DanmakuViewModel.DanmakuSpeed) * 10);
        }
    }

    private void OnSendDanmakuSucceeded(object sender, string e)
    {
        var model = new DanmakuModel
        {
            Color = AppToolkit.HexToColor(ViewModel.DanmakuViewModel.Color),
            Size = ViewModel.DanmakuViewModel.IsStandardSize ? 22 : 16,
            Text = e,
            Location = ViewModel.DanmakuViewModel.Location,
        };

        _danmakuView.AddScreenDanmaku(model, true);
    }

    private void OnLiveDanmakuAdded(object sender, LiveDanmakuInformation e)
    {
        if (_danmakuView != null)
        {
            var myName = AccountViewModel.Instance.Name;
            var isOwn = !string.IsNullOrEmpty(myName) && myName == e.UserName;
            var color = string.IsNullOrEmpty(e.TextColor) ? Colors.White : AppToolkit.HexToColor(e.TextColor);
            _danmakuView.AddLiveDanmaku(e.Text, isOwn, color);
        }
    }

    private void OnRequestClearDanmaku(object sender, EventArgs e)
    {
        _danmakuDictionary.Clear();
        _danmakuTimer.Stop();
        _danmakuView?.ClearAll();
    }

    private void OnDanmakuListAdded(object sender, IEnumerable<DanmakuInformation> e)
    {
        InitializeDanmaku(e);
        _danmakuTimer.Start();
    }

    private void OnDanmkuTimerTick(object sender, object e)
    {
        try
        {
            if (ViewModel.Status != PlayerStatus.Playing)
            {
                return;
            }

            var position = ViewModel.Player.Position.TotalSeconds;
            var positionInt = Convert.ToInt32(position);
            if (_danmakuDictionary.ContainsKey(positionInt))
            {
                var data = _danmakuDictionary[positionInt];

                if (ViewModel.DanmakuViewModel.IsDanmakuMerge)
                {
                    data = data.Distinct().ToList();
                }

                DispatcherQueue.TryEnqueue(() =>
                {
                    foreach (var item in data)
                    {
                        _danmakuView.AddScreenDanmaku(item, false);
                    }
                });
            }
        }
        catch (Exception)
        {
        }
    }

    private void OnUnitTimerTick(object sender, object e)
    {
        _cursorStayTime += 0.5;
        _transportStayTime += 0.5;

        if (_tempMessageStayTime != -1)
        {
            _tempMessageStayTime += 0.5;
        }

        if (ViewModel.IsShowNextVideoTip)
        {
            _nextVideoStayTime += 0.5;
        }

        if (ViewModel.IsShowProgressTip)
        {
            _progressTipStayTime += 0.5;
        }

        HandleTransportAutoHide();
        HandleCursorAutoHide();
        HandleTempMessageAutoHide();
        HandleNextVideoAutoHide();
        HandleProgressTipAutoHide();
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.Status))
        {
            if (ViewModel.Status == PlayerStatus.Playing)
            {
                _danmakuView.ResumeDanmaku();
            }
            else if (!IsLive)
            {
                _danmakuView.PauseDanmaku();
            }
        }
    }

    private void OnInteractionControlTapped(object sender, TappedRoutedEventArgs e)
    {
        if (_isHolding)
        {
            _isHolding = false;
            return;
        }

        if (e.PointerDeviceType == PointerDeviceType.Mouse)
        {
            _isTouch = false;
            ViewModel.PlayPauseCommand.Execute(default);
        }
        else
        {
            _isTouch = true;
            ViewModel.IsShowMediaTransport = !ViewModel.IsShowMediaTransport;
            ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
        }
    }

    private void OnInteractionControlDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        var playerStatus = ViewModel.Status;
        var canDoubleTapped = playerStatus == PlayerStatus.Playing || playerStatus == PlayerStatus.Pause;
        if (canDoubleTapped)
        {
            if (e.PointerDeviceType == PointerDeviceType.Mouse)
            {
                ViewModel.ToggleFullScreenModeCommand.Execute(default);
                if (ViewModel.IsMediaPause)
                {
                    ViewModel.PlayPauseCommand.Execute(default);
                }
            }
            else
            {
                ViewModel.PlayPauseCommand.Execute(default);
            }
        }
    }

    private void OnInteractionControlManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        _manipulationVolume = 0;
        _manipulationProgress = 0;
        _manipulationDeltaX = 0;
        _manipulationDeltaY = 0;
        _manipulationType = PlayerManipulationType.None;

        if (_manipulationBeforeIsPlay)
        {
            ViewModel.PlayPauseCommand.Execute(default);
        }

        _manipulationBeforeIsPlay = false;
    }

    private void OnGestureRecognizerHolding(GestureRecognizer sender, HoldingEventArgs args)
    {
        _isHolding = true;
        if (args.HoldingState == HoldingState.Started)
        {
            ViewModel.StartTempQuickPlayCommand.Execute(default);
        }
        else
        {
            ViewModel.StopTempQuickPlayCommand.Execute(default);
        }
    }

    private void OnInteractionControlManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        if (ViewModel.Status != PlayerStatus.Playing
            && ViewModel.Status != PlayerStatus.Pause)
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

                ViewModel.ChangeVolumeCommand.Execute(Convert.ToInt32(volume));
            }
            else
            {
                var progress = _manipulationProgress + (_manipulationDeltaX * _manipulationUnitLength);
                if (progress > ViewModel.DurationSeconds)
                {
                    progress = ViewModel.DurationSeconds;
                }
                else if (progress < 0)
                {
                    progress = 0;
                }

                ViewModel.ChangeProgressCommand.Execute(progress);
            }
        }
    }

    private void OnInteractionControlManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
    {
        if (ViewModel.Status == PlayerStatus.NotLoad || ViewModel.Status == PlayerStatus.Buffering)
        {
            return;
        }

        _manipulationProgress = ViewModel.ProgressSeconds;
        _manipulationVolume = ViewModel.Volume;

        // 点击事件先于手势事件，当该事件触发时，可能已经切换了播放状态.
        _manipulationBeforeIsPlay = ViewModel.Status == PlayerStatus.Pause;
        if (ViewModel.DurationSeconds > 0)
        {
            // 获取单位像素对应的时长
            var unit = ViewModel.DurationSeconds / ActualWidth;
            _manipulationUnitLength = unit / 1.5;
        }
    }

    private void OnInteractionControlPointerCanceled(object sender, PointerRoutedEventArgs e)
        => _gestureRecognizer.ProcessUpEvent(e.GetCurrentPoint(this));

    private void OnInteractionControlPointerReleased(object sender, PointerRoutedEventArgs e)
        => _gestureRecognizer.ProcessUpEvent(e.GetCurrentPoint(this));

    private void OnInteractionControlPointerMoved(object sender, PointerRoutedEventArgs e)
        => _gestureRecognizer.ProcessMoveEvents(e.GetIntermediatePoints(this));

    private void OnInteractionControlPointerPressed(object sender, PointerRoutedEventArgs e)
        => _gestureRecognizer.ProcessDownEvent(e.GetCurrentPoint(this));

    private void OnRefreshButtonClick(object sender, RoutedEventArgs e)
    {
        if (ViewModel.CurrentFormat != null)
        {
            ViewModel.ChangeFormatCommand.Execute(ViewModel.CurrentFormat);
        }
    }

    private void OnOpenInBrowserButtonClick(object sender, RoutedEventArgs e)
        => ViewModel.OpenInBrowserCommand.Execute(default);
}
