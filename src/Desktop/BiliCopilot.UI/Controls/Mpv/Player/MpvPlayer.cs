// Copyright (c) Bili Copilot. All rights reserved.

using System.ComponentModel;
using BiliCopilot.UI.Controls.Mpv.Common;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Mpv;

/// <summary>
/// MPV 播放器.
/// </summary>
public sealed partial class MpvPlayer : LayoutControlBase<PlayerViewModel>
{
    private PlayerViewModel? _viewModel;
    private long _viewModelChangedToken;
    private RenderControl _renderControl;
    private Rect _transportControlTriggerRect;
    private Rectangle _interactionControl;
    private StackPanel? _notificationContainer;

    private double _lastSpeed;

    /// <summary>
    /// Initializes a new instance of the <see cref="MpvPlayer"/> class.
    /// </summary>
    public MpvPlayer() => DefaultStyleKey = typeof(MpvPlayer);

    /// <inheritdoc/>
    protected override async void OnControlLoaded()
    {
        _viewModelChangedToken = RegisterPropertyChangedCallback(ViewModelProperty, new DependencyPropertyChangedCallback(OnViewModelPropertyChangedAsync));

        _renderControl.Render += OnRender;
        _interactionControl.Tapped += OnCoreTapped;
        _interactionControl.DoubleTapped += OnCoreDoubleTapped;
        _interactionControl.Holding += OnCoreHolding;

        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        _viewModel.PropertyChanged += OnViewModelInnerPropertyChanged;
        _viewModel.RequestShowNotification += OnRequestShowNotification;
        SizeChanged += OnSizeChanged;
        await _viewModel.InitializeAsync(_renderControl);
        if (TransportControls is not null)
        {
            TransportControls.Visibility = Visibility.Visible;
            MeasureTransportTriggerRect();
            TransportControls.Visibility = Visibility.Collapsed;
        }
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        SizeChanged -= OnSizeChanged;
        UnregisterPropertyChangedCallback(ViewModelProperty, _viewModelChangedToken);
        if (ViewModel is not null)
        {
            ViewModel.PropertyChanged -= OnViewModelInnerPropertyChanged;
            ViewModel.RequestShowNotification -= OnRequestShowNotification;
        }

        _viewModel = default;

        _renderControl.Render -= OnRender;

        if (_interactionControl != null)
        {
            _interactionControl.Tapped -= OnCoreTapped;
            _interactionControl.DoubleTapped -= OnCoreDoubleTapped;
            _interactionControl.Holding -= OnCoreHolding;
            _interactionControl = default;
        }
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _renderControl = (RenderControl)GetTemplateChild("RenderControl");
        _interactionControl = (Rectangle)GetTemplateChild("InteractionControl");
        _notificationContainer = (StackPanel)GetTemplateChild("NotificationContainer");
        _renderControl.Setting = new ContextSettings()
        {
            MajorVersion = 4,
            MinorVersion = 6,
            GraphicsProfile = OpenTK.Windowing.Common.ContextProfile.Compatability,
        };
    }

    /// <inheritdoc/>
    protected override void OnPointerMoved(PointerRoutedEventArgs e)
    {
        CheckTransportControlVisibility(e);
    }

    /// <inheritdoc/>
    protected override void OnPointerEntered(PointerRoutedEventArgs e)
    {
        CheckTransportControlVisibility(e);
    }

    /// <inheritdoc/>
    protected override void OnPointerExited(PointerRoutedEventArgs e)
    {
        if (TransportControls is not null)
        {
            TransportControls.Visibility = Visibility.Collapsed;
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

    private void OnCoreTapped(object sender, TappedRoutedEventArgs e)
        => ViewModel?.TogglePlayPauseCommand.Execute(default);

    private void OnCoreDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (ViewModel is null)
        {
            return;
        }

        ViewModel.ToggleFullScreenCommand.Execute(default);
        if (ViewModel.IsPaused)
        {
            ViewModel.TogglePlayPauseCommand.Execute(default);
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

    private async void OnViewModelPropertyChangedAsync(DependencyObject sender, DependencyProperty dp)
    {
        if (ViewModel is null)
        {
            return;
        }

        if (_viewModel is not null)
        {
            _viewModel.PropertyChanged -= OnViewModelInnerPropertyChanged;
            _viewModel.RequestShowNotification -= OnRequestShowNotification;
        }

        _viewModel = ViewModel;
        _viewModel.PropertyChanged += OnViewModelInnerPropertyChanged;
        _viewModel.RequestShowNotification += OnRequestShowNotification;
        await _viewModel.InitializeAsync(_renderControl);
    }

    private void OnViewModelInnerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PlayerViewModel.IsPaused) && TransportControls is not null)
        {
            TransportControls.Visibility = ViewModel.IsPaused ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private void OnRender(TimeSpan e) => Render();

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        MeasureTransportTriggerRect();
        ArrangeSubtitleSize();
    }
}
