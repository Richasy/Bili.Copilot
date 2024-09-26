// Copyright (c) Bili Copilot. All rights reserved.

using System.ComponentModel;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Input;

namespace BiliCopilot.UI.Controls.Core;

/// <summary>
/// MPV 播放器.
/// </summary>
public sealed partial class BiliPlayer : PlayerControlBase
{
    private Rect _transportControlTriggerRect;
    private DispatcherTimer? _cursorTimer;

    private double _lastSpeed;
    private double _cursorStayTime;
    private double _mtcStayTime;
    private bool _isCursorDisposed;
    private bool _isTouch;
    private Point? _lastPointerPoint;

    private double _manipulationDeltaX = 0d;
    private double _manipulationDeltaY = 0d;
    private double _manipulationProgress = 0d;
    private double _manipulationVolume = 0d;
    private double _manipulationUnitLength = 0d;
    private bool _manipulationBeforeIsPlay = false;
    private PlayerManipulationType _manipulationType = PlayerManipulationType.None;
    private bool _isIslandPlayer = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="BiliPlayer"/> class.
    /// </summary>
    public BiliPlayer() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        if (_cursorTimer is null)
        {
            _cursorTimer = new DispatcherTimer();
            _cursorTimer.Interval = TimeSpan.FromSeconds(0.5);
            _cursorTimer.Tick += OnCursorTimerTick;
        }

        InteractionControl.Tapped += OnCoreTapped;
        InteractionControl.DoubleTapped += OnCoreDoubleTapped;
        InteractionControl.Holding += OnCoreHolding;
        InteractionControl.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
        InteractionControl.ManipulationStarted += OnInteractionControlManipulationStarted;
        InteractionControl.ManipulationDelta += OnInteractionControlManipulationDelta;
        InteractionControl.ManipulationCompleted += OnInteractionControlManipulationCompleted;

        if (ViewModel is null)
        {
            return;
        }

        PlayerPresenter.ViewModel = ViewModel;
        SizeChanged += OnSizeChanged;
        if (TransportControls is not null)
        {
            TransportControls.Visibility = Visibility.Visible;
            MeasureTransportTriggerRect();
            SetTransportVisibility(false);
        }

        _cursorTimer?.Start();
        ArrangeSubtitleSize();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        SizeChanged -= OnSizeChanged;
        if (ViewModel is not null)
        {
            ViewModel.RequestShowNotification -= OnRequestShowNotification;
            ViewModel.RequestCancelNotification -= OnRequestCancelNotification;
            ViewModel.PropertyChanged -= OnViewModelInnerPropertyChanged;
            if (ViewModel is IslandPlayerViewModel islandVM)
            {
                islandVM.Initialized -= OnIslandViewModelInitialized;
            }
        }

        if (_cursorTimer is not null)
        {
            _cursorTimer.Tick -= OnCursorTimerTick;
            _cursorTimer.Stop();
            _cursorTimer = default;
        }

        ViewModel = default;
        PlayerPresenter.ViewModel = default;

        if (InteractionControl != null)
        {
            InteractionControl.Tapped -= OnCoreTapped;
            InteractionControl.DoubleTapped -= OnCoreDoubleTapped;
            InteractionControl.Holding -= OnCoreHolding;
            InteractionControl.ManipulationStarted -= OnInteractionControlManipulationStarted;
            InteractionControl.ManipulationDelta -= OnInteractionControlManipulationDelta;
            InteractionControl.ManipulationCompleted -= OnInteractionControlManipulationCompleted;
        }
    }

    /// <inheritdoc/>
    protected override void OnPointerMoved(PointerRoutedEventArgs e)
        => HandlePointerEvent(e);

    /// <inheritdoc/>
    protected override void OnPointerEntered(PointerRoutedEventArgs e)
        => HandlePointerEvent(e);

    /// <inheritdoc/>
    protected override void OnPointerExited(PointerRoutedEventArgs e)
        => HandlePointerEvent(e, true);

    /// <inheritdoc/>
    protected override void OnPointerPressed(PointerRoutedEventArgs e)
        => CheckPointerType(e.Pointer);

    /// <inheritdoc/>
    protected override void OnPointerCanceled(PointerRoutedEventArgs e)
        => HandlePointerEvent(e, true);

    /// <inheritdoc/>
    protected override void OnViewModelChanged(PlayerViewModelBase? oldValue, PlayerViewModelBase? newValue)
    {
        if (newValue is null)
        {
            PlayerPresenter.ViewModel = default;
            _isIslandPlayer = false;
            return;
        }

        if (oldValue is not null)
        {
            oldValue.PropertyChanged -= OnViewModelInnerPropertyChanged;
            oldValue.RequestShowNotification -= OnRequestShowNotification;
            oldValue.RequestCancelNotification -= OnRequestCancelNotification;
            if (oldValue is IslandPlayerViewModel oldIslandVM)
            {
                oldIslandVM.Initialized -= OnIslandViewModelInitialized;
            }
        }

        _isIslandPlayer = newValue is IslandPlayerViewModel;
        PlayerPresenter.ViewModel = newValue;
        newValue.PropertyChanged += OnViewModelInnerPropertyChanged;
        newValue.RequestShowNotification += OnRequestShowNotification;
        newValue.RequestCancelNotification += OnRequestCancelNotification;
        if (newValue is IslandPlayerViewModel islandVM)
        {
            islandVM.Initialized += OnIslandViewModelInitialized;
        }

        ArrangeSubtitleSize();
    }

    private void HandlePointerEvent(PointerRoutedEventArgs e, bool forceHideTransportControls = false)
    {
        if (!ShouldResponsePlayerRequest())
        {
            return;
        }

        CheckPointerType(e.Pointer);
        RestoreCursor();
        if (!_isTouch)
        {
            if (forceHideTransportControls)
            {
                _lastPointerPoint = default;
                _mtcStayTime = 0;
                SetTransportVisibility(false);
                return;
            }

            _mtcStayTime = 0;
            CheckTransportControlVisibility(e);
        }
    }

    private void OnRequestShowNotification(object? sender, PlayerNotificationItemViewModel e)
    {
        if (!ShouldResponsePlayerRequest())
        {
            return;
        }

        NotificationContainer.Children.Clear();
        var control = new PlayerNotificationControl();
        control.ViewModel = e;
        e.IsNotificationVisible = true;
        NotificationContainer.Children.Add(control);
    }

    private void OnRequestCancelNotification(object? sender, EventArgs e)
    {
        if (!ShouldResponsePlayerRequest())
        {
            return;
        }

        if (NotificationContainer is not null && NotificationContainer.Children.FirstOrDefault() is PlayerNotificationControl control)
        {
            control.ViewModel.CancelCommand.Execute(default);
            NotificationContainer.Children.Clear();
        }
    }

    private void OnIslandViewModelInitialized(object? sender, EventArgs e)
    {
        var vm = ViewModel as IslandPlayerViewModel;
        RootGrid.Children.Remove(OverlayContainer);
        vm.SetXamlContent(OverlayContainer);
    }

    private void OnCoreTapped(object sender, TappedRoutedEventArgs e)
    {
        if (!ShouldResponsePlayerRequest())
        {
            return;
        }

        _isTouch = e.PointerDeviceType == PointerDeviceType.Touch;
        if (_isTouch)
        {
            SetTransportVisibility(TransportControls.Visibility == Visibility.Collapsed);
        }
        else
        {
            ViewModel?.TogglePlayPauseCommand.Execute(default);
        }
    }

    private void OnCoreDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (!ShouldResponsePlayerRequest())
        {
            return;
        }

        _isTouch = e.PointerDeviceType == PointerDeviceType.Touch;
        if (ViewModel is null)
        {
            return;
        }

        if (_isTouch)
        {
            ViewModel.TogglePlayPauseCommand.Execute(default);
        }
        else
        {
            if (ViewModel.IsPaused)
            {
                ViewModel.TogglePlayPauseCommand.Execute(default);
            }

            ViewModel.ToggleFullScreenCommand.Execute(default);
        }
    }

    private void OnCoreHolding(object sender, HoldingRoutedEventArgs e)
    {
        if (!ShouldResponsePlayerRequest())
        {
            return;
        }

        if (e.HoldingState == HoldingState.Started)
        {
            _lastSpeed = ViewModel.Speed;
            ViewModel.SetSpeedCommand.Execute(3d);
        }
        else
        {
            _lastSpeed = _lastSpeed == 0 ? 1.0 : _lastSpeed;
            ViewModel.SetSpeedCommand.Execute(_lastSpeed);
            _lastSpeed = 0;
        }
    }

    private void OnViewModelInnerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (!ShouldResponsePlayerRequest())
        {
            return;
        }

        if (ViewModel != null && e.PropertyName == nameof(MpvPlayerViewModel.IsPaused) && TransportControls is not null && !ViewModel.IsExternalPlayer)
        {
            SetTransportVisibility(ViewModel.IsPaused);
        }
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (ShouldResponsePlayerRequest())
        {
            MeasureTransportTriggerRect();
            ArrangeSubtitleSize();
        }
    }

    private bool ShouldResponsePlayerRequest()
        => !_isIslandPlayer || IsIsland;
}
