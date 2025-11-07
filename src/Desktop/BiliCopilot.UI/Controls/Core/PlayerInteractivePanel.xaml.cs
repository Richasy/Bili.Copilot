// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

namespace BiliCopilot.UI.Controls.Core;

public sealed partial class PlayerInteractivePanel : PlayerControlBase
{
    private readonly DispatcherQueueTimer _tapTimer;
    private readonly DispatcherQueueTimer _holdTimer;
    private readonly DispatcherQueueTimer _tempControlTimer;
    private Point _startPoint;
    private InteractiveArea _interactiveArea;
    private double _totalDeltaX;
    private int _tapCount;
    private bool _isManipulating;
    private bool _isTouch;
    private bool _isLeftButton;
    private bool _isRightButton;
    private bool _isHolding;

    public PlayerInteractivePanel()
    {
        InitializeComponent();
        _tapTimer = DispatcherQueue.CreateTimer();
        _tapTimer.Interval = TimeSpan.FromMilliseconds(200);
        _tapTimer.Tick += OnTapTimerTick;
        _holdTimer = DispatcherQueue.CreateTimer();
        _holdTimer.Interval = TimeSpan.FromMilliseconds(500);
        _holdTimer.Tick += OnHoldTimerTick;
        _tempControlTimer = DispatcherQueue.CreateTimer();
        _tempControlTimer.Interval = TimeSpan.FromMilliseconds(500);
        _tempControlTimer.Tick += OnTempControlTimerTick;
        HorizontalAlignment = HorizontalAlignment.Stretch;
        VerticalAlignment = VerticalAlignment.Stretch;
    }

    internal static string GetPanscanText(bool usingPanscan)
        => usingPanscan ? ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.CancelCutBlackArea) : ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.CutBlackArea);

    protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
    {
        var distance = e.GetCurrentPoint(this).Properties.MouseWheelDelta;
        if (!AppToolkit.IsShiftPressed() && !AppToolkit.IsCtrlPressed() && !AppToolkit.IsAltPressed())
        {
            if (ViewModel?.Client is not null)
            {
                if (distance > 0)
                {
                    ViewModel.IncreaseVolumeCommand.Execute(default);
                }
                else if (distance < 0)
                {
                    ViewModel.DecreaseVolumeCommand.Execute(default);
                }
            }
        }
        else if (AppToolkit.IsOnlyCtrlPressed())
        {
            if (ViewModel?.Client is not null)
            {
                if (distance > 0)
                {
                    ViewModel.IncreaseSpeedCommand.Execute(default);
                }
                else if (distance < 0)
                {
                    ViewModel.DecreaseSpeedCommand.Execute(default);
                }
            }
        }
    }

    /// <inheritdoc/>
    protected override void OnPointerPressed(PointerRoutedEventArgs e)
    {
        var point = e.GetCurrentPoint(this);
        CheckPointerType(e);
        _isLeftButton = e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Mouse &&
                        point.Properties.IsLeftButtonPressed;
        _isRightButton = e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Mouse &&
                         point.Properties.IsRightButtonPressed;
        _startPoint = point.Position;
        var width = ActualWidth;
        var sideWidth = Math.Max(80, width / 5d);

        if (point.Position.X > width - sideWidth)
        {
            _interactiveArea = InteractiveArea.Aside;
        }
        else
        {
            _interactiveArea = InteractiveArea.Main;
        }

        _isHolding = false;
        if (!_isRightButton)
        {
            _holdTimer.Start();
        }

        CapturePointer(e.Pointer);
        e.Handled = true;
    }

    protected override void OnControlLoaded()
        => _tempControlTimer.Start();

    protected override void OnControlUnloaded()
    {
        _holdTimer.Tick -= OnHoldTimerTick;
        _holdTimer.Stop();
        _tapTimer.Tick -= OnTapTimerTick;
        _tapTimer.Stop();
        _tempControlTimer.Tick -= OnTempControlTimerTick;
        _tempControlTimer.Stop();
    }

    /// <inheritdoc/>
    protected override void OnPointerMoved(PointerRoutedEventArgs e)
    {
        if (PointerCaptures?.Any(p => p.PointerId == e.Pointer.PointerId) != true)
        {
            return;
        }

        CheckPointerType(e);
        var currentPoint = e.GetCurrentPoint(this);
        var deltaX = currentPoint.Position.X - _startPoint.X;
        var deltaY = currentPoint.Position.Y - _startPoint.Y;

        if (!_isManipulating && (Math.Abs(deltaX) > 5 || Math.Abs(deltaY) > 5))
        {
            _isManipulating = true;
            _tapTimer.Stop();
            _holdTimer.Stop();
            _tapCount = 0;
        }

        if (_isManipulating)
        {
            _totalDeltaX += deltaX;
            HandleManipulationUpdate(deltaX, deltaY);
            _startPoint = currentPoint.Position;
        }

        e.Handled = true;
    }

    /// <inheritdoc/>
    protected override void OnPointerReleased(PointerRoutedEventArgs e)
    {
        if (PointerCaptures?.Any(p => p.PointerId == e.Pointer.PointerId) != true)
        {
            return;
        }

        CheckPointerType(e);
        var currentPoint = e.GetCurrentPoint(this);
        var deltaX = currentPoint.Position.X - _startPoint.X;
        var deltaY = currentPoint.Position.Y - _startPoint.Y;

        _holdTimer.Stop();

        if (_isHolding)
        {
            ViewModel?.RestoreSpeedCommand?.Execute(default);
            _isHolding = false;
        }
        else if (!_isManipulating)
        {
            _tapCount++;
            if (_tapCount == 1)
            {
                _tapTimer.Start();
            }
        }
        else
        {
            HandleManipulationCompleted();
        }

        if (_isRightButton)
        {
            _isRightButton = false;
            if (!ViewModel!.IsConnecting && ViewModel.Player?.IsPlaybackInitialized == true)
            {
                FlyoutBase.GetAttachedFlyout(this).ShowAt(this, new FlyoutShowOptions
                {
                    Position = e.GetCurrentPoint(this).Position,
                });
            }
        }

        ReleasePointerCapture(e.Pointer);
        e.Handled = true;
    }

    private void OnTempControlTimerTick(DispatcherQueueTimer sender, object args)
    {
        if (ViewModel == null)
        {
            return;
        }

        var now = DateTimeOffset.Now;
        if (ViewModel.IsVolumeChanging && now - ViewModel.LastVolumeChangingTime > TimeSpan.FromSeconds(2))
        {
            ViewModel.IsVolumeChanging = false;
        }

        if (ViewModel.IsProgressChanging && now - ViewModel.LastProgressChangingTime > TimeSpan.FromSeconds(2))
        {
            ViewModel.IsProgressChanging = false;
        }

        if (ViewModel.IsSpeedChanging && now - ViewModel.LastSpeedChangingTime > TimeSpan.FromSeconds(2))
        {
            ViewModel.IsSpeedChanging = false;
        }
    }

    private void OnHoldTimerTick(object sender, object e)
    {
        _holdTimer.Stop();
        _isHolding = true;
        ViewModel?.TripleSpeedCommand?.Execute(default);
    }

    private async void OnTapTimerTick(object? sender, object? e)
    {
        _tapTimer.Stop();
        if (ViewModel.Client is null)
        {
            return;
        }

        if (_tapCount == 2)
        {
            if (_isTouch)
            {
                var state = ViewModel.Player.PlaybackState;
                if (state == Richasy.MpvKernel.Core.Enums.MpvPlayerState.Playing)
                {
                    await ViewModel.Client.PauseAsync();
                }
                else if (state == Richasy.MpvKernel.Core.Enums.MpvPlayerState.Paused)
                {
                    await ViewModel.Client.ResumeAsync();
                }
            }
            else if (_isLeftButton)
            {
                ViewModel.ToggleFullScreenCommand.Execute(default);
            }
        }
        else if (_tapCount == 1 && !ViewModel.IsEnd && (!ViewModel.IsControlsVisible || (_startPoint.Y >= 50 && ActualHeight - _startPoint.Y >= 150)))
        {
            if (_isTouch)
            {
                ViewModel.IsControlsVisible = !ViewModel.IsControlsVisible;
                ViewModel.IsTouchControlsVisible = !ViewModel.IsTouchControlsVisible;
            }
            else if (_isLeftButton)
            {
                ViewModel.PlayPauseCommand.Execute(default);
            }
        }

        _tapCount = 0;
    }

    private async void HandleManipulationUpdate(double deltaX, double deltaY)
    {
        switch (_interactiveArea)
        {
            case InteractiveArea.Main:
                {
                    if (Math.Abs(deltaX) > 2 && Math.Abs(deltaX) > Math.Abs(deltaY))
                    {
                        if (ViewModel.Player.PlaybackState == Richasy.MpvKernel.Core.Enums.MpvPlayerState.Playing)
                        {
                            await ViewModel.Client!.PauseAsync();
                        }

                        var newPos = GetNewPosition();
                        if (newPos != null)
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                ViewModel.IsPreviewProgressChanging = true;
                                ViewModel.IsControlsVisible = true;
                                ViewModel.PreviewPosition = newPos.Value;
                            });
                        }
                    }
                }

                break;
            case InteractiveArea.Aside:
                {
                    if (Math.Abs(deltaY) > 5 && Math.Abs(deltaY) > Math.Abs(deltaX))
                    {
                        var volumeChange = -deltaY / 10;
                        DispatcherQueue.TryEnqueue(async () =>
                        {
                            var currentVolume = ViewModel.Player.Volume;
                            var newVolume = Math.Max(0, Math.Min(ViewModel.MaxVolume, currentVolume + volumeChange));
                            ViewModel.LastVolumeChangingTime = DateTimeOffset.Now;
                            ViewModel.IsVolumeChanging = true;
                            await ViewModel.Client!.SetVolumeAsync(newVolume);
                        });
                    }
                }

                break;
            default:
                break;
        }
    }

    private async void HandleManipulationCompleted()
    {
        if (_interactiveArea == InteractiveArea.Main && Math.Abs(_totalDeltaX) > 10)
        {
            var newPos = GetNewPosition();
            if (newPos.HasValue)
            {
                await ViewModel.Client!.SetCurrentPositionAsync(newPos.Value);
                await ViewModel.Client.ResumeAsync();
            }
        }

        _startPoint = new(0, 0);
        _totalDeltaX = 0;
        _interactiveArea = InteractiveArea.None;
        _isManipulating = false;
    }

    private double? GetNewPosition()
    {
        var state = ViewModel.Player.PlaybackState;
        var isValidState = state is Richasy.MpvKernel.Core.Enums.MpvPlayerState.Playing or Richasy.MpvKernel.Core.Enums.MpvPlayerState.Paused;
        if (isValidState)
        {
            var duration = ViewModel.Player.Duration;
            var currentPosition = ViewModel.Player.Position;
            var newPosition = currentPosition + (_totalDeltaX / ActualWidth / 2 * duration);
            if (newPosition < 0)
            {
                newPosition = 0;
            }
            else if (newPosition > duration)
            {
                newPosition = duration - 0.1;
            }

            return newPosition;
        }

        return default;
    }

    private void CheckPointerType(PointerRoutedEventArgs e)
        => _isTouch = e.Pointer.PointerDeviceType is Microsoft.UI.Input.PointerDeviceType.Touch or Microsoft.UI.Input.PointerDeviceType.Pen;

    /// <summary>
    /// Interactive area types.
    /// </summary>
    internal enum InteractiveArea
    {
        /// <summary>
        /// None.
        /// </summary>
        None,

        /// <summary>
        /// Main area.
        /// </summary>
        Main,

        /// <summary>
        /// Side area.
        /// </summary>
        Aside,
    }

    private void OnFlyoutOpened(object sender, object e)
    {
        ViewModel.IsPopupVisible = true;
        foreach (var item in Anime4KItem.Items.OfType<RadioMenuFlyoutItem>())
        {
            var tag = item.Tag.ToString();
            if (Enum.TryParse<Anime4KMode>(tag, out var mode))
            {
                item.IsChecked = ViewModel.Anime4KMode == mode;
            }
        }

        foreach (var item in ArtCNNItem.Items.OfType<RadioMenuFlyoutItem>())
        {
            var tag = item.Tag.ToString();
            if (Enum.TryParse<ArtCNNMode>(tag, out var mode))
            {
                item.IsChecked = ViewModel.ArtCNNMode == mode;
            }
        }

        foreach (var item in NNEDI3Item.Items.OfType<RadioMenuFlyoutItem>())
        {
            var tag = item.Tag.ToString();
            if (Enum.TryParse<Nnedi3Mode>(tag, out var mode))
            {
                item.IsChecked = ViewModel.Nnedi3Mode == mode;
            }
        }

        foreach (var item in RavuItem.Items.OfType<RadioMenuFlyoutItem>())
        {
            var tag = item.Tag.ToString();
            if (Enum.TryParse<RavuMode>(tag, out var mode))
            {
                item.IsChecked = ViewModel.RavuMode == mode;
            }
        }

        foreach (var item in NvidiaVsrItem.Items.OfType<RadioMenuFlyoutItem>())
        {
            var tag = item.Tag.ToString();
            if (Enum.TryParse<VsrScale>(tag, out var mode))
            {
                item.IsChecked = ViewModel.VsrScale == mode;
            }
        }
    }

    private void OnRowBarButtonClick(object sender, EventArgs e)
    {
        ContextMenu.Hide();
    }

    private void OnFlyoutClosed(object sender, object e)
    {
        ViewModel.IsPopupVisible = false;
    }

    private void OnMenuButtonClick(object sender, RoutedEventArgs e)
    {
        var btn = sender as FrameworkElement;
        var point = btn!.TransformToVisual(this).TransformPoint(new Point(0, 0));
        FlyoutBase.GetAttachedFlyout(this).ShowAt(this, new FlyoutShowOptions
        {
            Position = point,
            Placement = FlyoutPlacementMode.Right,
        });
    }

    private void OnAnime4KItemClick(object sender, RoutedEventArgs e)
    {
        var tag = (sender as FrameworkElement)!.Tag.ToString();
        if (Enum.TryParse<Anime4KMode>(tag, out var mode))
        {
            ViewModel.ChangeAnime4KModeCommand.Execute(mode);
        }
    }

    private void OnArtCNNItemClick(object sender, RoutedEventArgs e)
    {
        var tag = (sender as FrameworkElement)!.Tag.ToString();
        if (Enum.TryParse<ArtCNNMode>(tag, out var mode))
        {
            ViewModel.ChangeArtCNNModeCommand.Execute(mode);
        }
    }

    private void OnNvidiaVsrItemClick(object sender, RoutedEventArgs e)
    {
        var tag = (sender as FrameworkElement)!.Tag.ToString();
        if (Enum.TryParse<VsrScale>(tag, out var mode))
        {
            ViewModel.ChangeNvidiaVsrModeCommand.Execute(mode);
        }
    }

    private void OnNNEDI3ItemClick(object sender, RoutedEventArgs e)
    {
        var tag = (sender as FrameworkElement)!.Tag.ToString();
        if (Enum.TryParse<Nnedi3Mode>(tag, out var mode))
        {
            ViewModel.ChangeNnedi3ModeCommand.Execute(mode);
        }
    }

    private void OnRavuItemClick(object sender, RoutedEventArgs e)
    {
        var tag = (sender as FrameworkElement)!.Tag.ToString();
        if (Enum.TryParse<RavuMode>(tag, out var mode))
        {
            ViewModel.ChangeRavuModeCommand.Execute(mode);
        }
    }
}
