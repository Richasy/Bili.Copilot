// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls.Mpv.Common;
using BiliCopilot.UI.ViewModels.Core;
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
        SizeChanged += OnSizeChanged;
        await _viewModel.InitializeAsync(_renderControl);
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        SizeChanged -= OnSizeChanged;
        UnregisterPropertyChangedCallback(ViewModelProperty, _viewModelChangedToken);
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

        _viewModel = ViewModel;
        await _viewModel.InitializeAsync(_renderControl);
    }

    private void OnRender(TimeSpan e) => Render();

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        => MeasureTransportTriggerRect();
}
