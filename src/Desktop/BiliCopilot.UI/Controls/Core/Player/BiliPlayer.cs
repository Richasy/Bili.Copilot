// Copyright (c) Bili Copilot. All rights reserved.

using System.ComponentModel;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
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

    private bool _isHolding;
    private double _lastSpeed;
    private double _cursorStayTime;
    private double _mtcStayTime;
    private bool _isCursorDisposed;
    private bool _needRemeasureSize = true;
    private Point? _lastPointerPoint;

    private double _manipulationDeltaX;
    private double _manipulationDeltaY;
    private double _manipulationProgress;
    private double _manipulationVolume;
    private double _manipulationUnitLength;
    private bool _manipulationBeforeIsPlay;
    private PlayerManipulationType _manipulationType = PlayerManipulationType.None;

    /// <summary>
    /// Initializes a new instance of the <see cref="BiliPlayer"/> class.
    /// </summary>
    public BiliPlayer() => InitializeComponent();

    /// <summary>
    /// 获取当前是否有文本框获得焦点.
    /// </summary>
    /// <returns>结果.</returns>
    public bool IsTextBoxFocused()
        => _overlayContainer is not null ? FocusManager.GetFocusedElement(_overlayContainer.XamlRoot) is TextBox : false;

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        if (_cursorTimer is null)
        {
            _cursorTimer = new DispatcherTimer();
            _cursorTimer.Interval = TimeSpan.FromSeconds(0.5);
            _cursorTimer.Tick += OnCursorTimerTick;
        }

        if (ViewModel is null)
        {
            return;
        }

        PlayerPresenter.ViewModel = ViewModel;
        ViewModel.SetIsTextBoxFocusedFunc(IsTextBoxFocused);
        SizeChanged += OnSizeChanged;

        SetTransportVisibility(true);
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            ArrangeSubtitleSize();
            MeasureTransportTriggerRect();
            SetTransportVisibility(false);
        });

        _cursorTimer?.Start();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        UnHookEventsWithoutViewModel();
        if (ViewModel is not null)
        {
            ViewModel.RequestShowNotification -= OnRequestShowNotification;
            ViewModel.RequestCancelNotification -= OnRequestCancelNotification;
            ViewModel.PropertyChanged -= OnViewModelInnerPropertyChanged;
            ViewModel.Initialized -= OnViewModelInitialized;
        }

        ViewModel = default;
        PlayerPresenter.ViewModel = default;
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(PlayerViewModelBase? oldValue, PlayerViewModelBase? newValue)
    {
        if (newValue is null)
        {
            PlayerPresenter.ViewModel = default;
            return;
        }

        if (oldValue is not null)
        {
            oldValue.PropertyChanged -= OnViewModelInnerPropertyChanged;
            oldValue.RequestShowNotification -= OnRequestShowNotification;
            oldValue.RequestCancelNotification -= OnRequestCancelNotification;
            oldValue.Initialized -= OnViewModelInitialized;
        }

        PlayerPresenter.ViewModel = newValue;
        newValue.PropertyChanged += OnViewModelInnerPropertyChanged;
        newValue.RequestShowNotification += OnRequestShowNotification;
        newValue.RequestCancelNotification += OnRequestCancelNotification;
        newValue.Initialized += OnViewModelInitialized;

        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            ArrangeSubtitleSize();
        });
    }

    private void HookRootPointerEvents()
    {
        _overlayContainer.PointerMoved += OnRootPointerMoved;
        _overlayContainer.PointerEntered += OnRootPointerEntered;
        _overlayContainer.PointerExited += OnRootPointerExited;
        _overlayContainer.PointerPressed += OnRootPointerPressed;
        _overlayContainer.PointerCanceled += OnRootPointerCanceled;
        _overlayContainer.PointerReleased += OnRootPointerReleased;
    }

    private void OnGestureRecognizerHolding(GestureRecognizer sender, HoldingEventArgs args)
    {
        _isHolding = true;
        if (args.HoldingState == HoldingState.Started)
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

    private void UnhookRootPointerEvents()
    {
        if (_overlayContainer is not null)
        {
            _overlayContainer.PointerMoved -= OnRootPointerMoved;
            _overlayContainer.PointerEntered -= OnRootPointerEntered;
            _overlayContainer.PointerExited -= OnRootPointerExited;
            _overlayContainer.PointerPressed -= OnRootPointerPressed;
            _overlayContainer.PointerCanceled -= OnRootPointerCanceled;
            _overlayContainer.PointerReleased -= OnRootPointerReleased;
        }
    }

    private void HookInteractionControlEvents()
    {
        _interactionControl.Tapped += OnCoreTapped;
        _interactionControl.DoubleTapped += OnCoreDoubleTapped;
        _interactionControl.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
        _interactionControl.ManipulationStarted += OnInteractionControlManipulationStarted;
        _interactionControl.ManipulationDelta += OnInteractionControlManipulationDelta;
        _interactionControl.ManipulationCompleted += OnInteractionControlManipulationCompleted;
        _interactionControl.ContextRequested += OnInteractionControlContextRequested;
        _gestureRecognizer.Holding += OnGestureRecognizerHolding;
    }

    private void UnhookInteractionControlEvents()
    {
        if (_interactionControl != null)
        {
            _interactionControl.Tapped -= OnCoreTapped;
            _interactionControl.DoubleTapped -= OnCoreDoubleTapped;
            _interactionControl.ManipulationStarted -= OnInteractionControlManipulationStarted;
            _interactionControl.ManipulationDelta -= OnInteractionControlManipulationDelta;
            _interactionControl.ManipulationCompleted -= OnInteractionControlManipulationCompleted;
            _interactionControl.ContextRequested -= OnInteractionControlContextRequested;
            _gestureRecognizer.Holding -= OnGestureRecognizerHolding;
        }
    }

    private void UnHookEventsWithoutViewModel()
    {
        SizeChanged -= OnSizeChanged;

        if (_cursorTimer is not null)
        {
            _cursorTimer.Tick -= OnCursorTimerTick;
            _cursorTimer.Stop();
            _cursorTimer = default;
        }

        UnhookInteractionControlEvents();
        UnhookRootPointerEvents();
    }

    private void HandlePointerEvent(PointerRoutedEventArgs e, bool forceHideTransportControls = false)
    {
        RestoreCursor();
        var isManual = SettingsToolkit.ReadLocalSetting(SettingNames.MTCBehavior, MTCBehavior.Automatic) == MTCBehavior.Manual;
        if (!isManual)
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

    private void OnViewModelInitialized(object? sender, EventArgs e)
    {
        if (_overlayContainer is not null)
        {
            return;
        }

        CreateOverlayContainer();
        if (ViewModel is IslandPlayerViewModel vm)
        {
            vm.SetXamlContent(_overlayContainer);
        }
        else
        {
            RootGrid.Children.Add(_overlayContainer);
        }
    }

    private void OnCoreTapped(object sender, TappedRoutedEventArgs e)
    {
        if (_isHolding)
        {
            _isHolding = false;
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
        else
        {
            ViewModel?.TogglePlayPauseCommand.Execute(default);
        }
    }

    private void OnCoreDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (ViewModel is null)
        {
            return;
        }

        var isManual = SettingsToolkit.ReadLocalSetting(SettingNames.MTCBehavior, MTCBehavior.Automatic) == MTCBehavior.Manual;
        if (isManual)
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

    private void OnViewModelInnerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_overlayContainer is not null)
        {
            HandleViewModelPropertyChanged(e);
        }

        if (ViewModel != null && e.PropertyName == nameof(MpvPlayerViewModel.IsPaused) && _transportControl is not null && !ViewModel.IsExternalPlayer)
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
