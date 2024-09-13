﻿// Copyright (c) Bili Copilot. All rights reserved.

using System.ComponentModel;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Core;

/// <summary>
/// MPV 播放器.
/// </summary>
public sealed partial class BiliPlayer : LayoutControlBase<PlayerViewModelBase>
{
    private PlayerViewModelBase? _viewModel;
    private long _viewModelChangedToken;
    private Rect _transportControlTriggerRect;
    private Rectangle _interactionControl;
    private StackPanel? _notificationContainer;
    private PlayerPresenter? _playerPresenter;
    private DispatcherTimer? _cursorTimer;

    private double _lastSpeed;
    private double _cursorStayTime;
    private bool _isCursorDisposed;
    private bool _isTouch;

    private double _manipulationDeltaX = 0d;
    private double _manipulationDeltaY = 0d;
    private double _manipulationProgress = 0d;
    private double _manipulationVolume = 0d;
    private double _manipulationUnitLength = 0d;
    private bool _manipulationBeforeIsPlay = false;
    private PlayerManipulationType _manipulationType = PlayerManipulationType.None;

    /// <summary>
    /// Initializes a new instance of the <see cref="BiliPlayer"/> class.
    /// </summary>
    public BiliPlayer() => DefaultStyleKey = typeof(BiliPlayer);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        if (_cursorTimer is null)
        {
            _cursorTimer = new DispatcherTimer();
            _cursorTimer.Interval = TimeSpan.FromSeconds(0.5);
            _cursorTimer.Tick += OnCursorTimerTick;
        }

        _viewModelChangedToken = RegisterPropertyChangedCallback(ViewModelProperty, new DependencyPropertyChangedCallback(OnViewModelPropertyChanged));
        _interactionControl.Tapped += OnCoreTapped;
        _interactionControl.DoubleTapped += OnCoreDoubleTapped;
        _interactionControl.Holding += OnCoreHolding;
        _interactionControl.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
        _interactionControl.ManipulationStarted += OnInteractionControlManipulationStarted;
        _interactionControl.ManipulationDelta += OnInteractionControlManipulationDelta;
        _interactionControl.ManipulationCompleted += OnInteractionControlManipulationCompleted;

        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        _playerPresenter.ViewModel = ViewModel;
        _viewModel.RequestShowNotification += OnRequestShowNotification;
        _viewModel.RequestCancelNotification += OnRequestCancelNotification;
        _viewModel.PropertyChanged += OnViewModelInnerPropertyChanged;
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
        UnregisterPropertyChangedCallback(ViewModelProperty, _viewModelChangedToken);
        if (ViewModel is not null)
        {
            ViewModel.RequestShowNotification -= OnRequestShowNotification;
            ViewModel.RequestCancelNotification -= OnRequestCancelNotification;
            ViewModel.PropertyChanged -= OnViewModelInnerPropertyChanged;
        }

        if (_cursorTimer is not null)
        {
            _cursorTimer.Tick -= OnCursorTimerTick;
            _cursorTimer.Stop();
            _cursorTimer = default;
        }

        _viewModel = default;
        _playerPresenter.ViewModel = default;

        if (_interactionControl != null)
        {
            _interactionControl.Tapped -= OnCoreTapped;
            _interactionControl.DoubleTapped -= OnCoreDoubleTapped;
            _interactionControl.Holding -= OnCoreHolding;
            _interactionControl.ManipulationStarted -= OnInteractionControlManipulationStarted;
            _interactionControl.ManipulationDelta -= OnInteractionControlManipulationDelta;
            _interactionControl.ManipulationCompleted -= OnInteractionControlManipulationCompleted;
            _interactionControl = default;
        }
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _playerPresenter = (PlayerPresenter)GetTemplateChild("PlayerPresenter");
        _interactionControl = (Rectangle)GetTemplateChild("InteractionControl");
        _notificationContainer = (StackPanel)GetTemplateChild("NotificationContainer");
    }

    /// <inheritdoc/>
    protected override void OnPointerMoved(PointerRoutedEventArgs e)
    {
        CheckPointerType(e.Pointer);
        RestoreCursor();
        if (!_isTouch)
        {
            CheckTransportControlVisibility(e);
        }
    }

    /// <inheritdoc/>
    protected override void OnPointerEntered(PointerRoutedEventArgs e)
    {
        CheckPointerType(e.Pointer);
        RestoreCursor();
        if (!_isTouch)
        {
            CheckTransportControlVisibility(e);
        }
    }

    /// <inheritdoc/>
    protected override void OnPointerExited(PointerRoutedEventArgs e)
    {
        CheckPointerType(e.Pointer);
        RestoreCursor();
        if (TransportControls is not null && !_isTouch)
        {
            SetTransportVisibility(false);
        }
    }

    /// <inheritdoc/>
    protected override void OnPointerPressed(PointerRoutedEventArgs e)
    {
        CheckPointerType(e.Pointer);
    }

    private void OnRequestShowNotification(object? sender, PlayerNotificationItemViewModel e)
    {
        _notificationContainer.Children.Clear();
        var control = new PlayerNotificationControl();
        control.ViewModel = e;
        e.IsNotificationVisible = true;
        _notificationContainer.Children.Add(control);
    }

    private void OnRequestCancelNotification(object? sender, EventArgs e)
    {
        if (_notificationContainer is not null && _notificationContainer.Children.FirstOrDefault() is PlayerNotificationControl control)
        {
            control.ViewModel.CancelCommand.Execute(default);
            _notificationContainer.Children.Clear();
        }
    }

    private void OnCoreTapped(object sender, TappedRoutedEventArgs e)
    {
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
            ViewModel.ToggleFullScreenCommand.Execute(default);
            if (ViewModel.IsPaused)
            {
                ViewModel.TogglePlayPauseCommand.Execute(default);
            }
        }
    }

    private void OnCoreHolding(object sender, HoldingRoutedEventArgs e)
    {
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

    private void OnViewModelPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (ViewModel is null)
        {
            _playerPresenter.ViewModel = default;
            return;
        }

        if (_viewModel is not null)
        {
            _viewModel.PropertyChanged -= OnViewModelInnerPropertyChanged;
            _viewModel.RequestShowNotification -= OnRequestShowNotification;
            _viewModel.RequestCancelNotification -= OnRequestCancelNotification;
        }

        _playerPresenter.ViewModel = ViewModel;
        _viewModel = ViewModel;
        _viewModel.PropertyChanged += OnViewModelInnerPropertyChanged;
        _viewModel.RequestShowNotification += OnRequestShowNotification;
        _viewModel.RequestCancelNotification += OnRequestCancelNotification;
        ArrangeSubtitleSize();
    }

    private void OnViewModelInnerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MpvPlayerViewModel.IsPaused) && TransportControls is not null && !ViewModel.IsExternalPlayer)
        {
            SetTransportVisibility(ViewModel.IsPaused);
        }
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        MeasureTransportTriggerRect();
        ArrangeSubtitleSize();
    }
}
