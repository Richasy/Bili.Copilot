// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Core;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Input;
using Richasy.MpvKernel.WinUI;
using System.ComponentModel;

namespace BiliCopilot.UI.Controls.Core;

/// <summary>
/// WEBDAV 播放器覆盖层.
/// </summary>
public sealed partial class WebDavPlayerOverlay : MpvPlayerControlBase, IMpvUIElement
{
    private readonly DispatcherQueueTimer _controlTimer;
    private Point _lastCursorPoint;
    private bool _isPointerOnUI;

    public WebDavSourceViewModel Source { get; }

    public WebDavPlayerOverlay(WebDavSourceViewModel sourceVM, MpvPlayerWindowViewModel stateVM)
    {
        InitializeComponent();
        _controlTimer = DispatcherQueue.CreateTimer();
        _controlTimer.Interval = TimeSpan.FromMilliseconds(1500);
        _controlTimer.Tick += OnControlTimerTick;
        Source = sourceVM;
        ViewModel = stateVM;
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override void OnPointerEntered(PointerRoutedEventArgs e)
        => _isPointerOnUI = true;

    protected override void OnPointerExited(PointerRoutedEventArgs e)
        => _isPointerOnUI = false;

    private void OnControlTimerTick(DispatcherQueueTimer sender, object args)
    {
        if (_isPointerOnUI)
        {
            return;
        }

        _controlTimer.Stop();
        ViewModel.Window.HideCursor();
        ViewModel.IsControlVisible = false;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.IsControlVisible) && ViewModel.IsControlVisible)
        {
            Source.IsPlaylistSectionPanelVisible = false;

            if (!_controlTimer.IsRunning)
            {
                _controlTimer.Start();
            }
        }
    }

    private void OnPlaylistButtonClick(object sender, RoutedEventArgs e)
    {
        Source.IsPlaylistSectionPanelVisible = true;
        ViewModel.IsControlVisible = false;
    }

    public void HandleUINotify(MpvUIEventId id, object data)
    {
        if (id == MpvUIEventId.PointerMoved)
        {
            var args = data as PointerRoutedEventArgs;
            _lastCursorPoint = args.GetCurrentPoint(this).Position;
            ViewModel.Window.ShowCursor();

            if (!Source.IsPlaylistSectionPanelVisible)
            {
                ViewModel.IsControlVisible = true;
                _controlTimer.Stop();
                _controlTimer.Start();
            }
        }
    }

    public void Disconnect()
    {
        if (_controlTimer != null)
        {
            _controlTimer.Stop();
            _controlTimer.Tick -= OnControlTimerTick;
        }

        ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    private void OnNextButtonClick(object sender, EventArgs e)
        => Source.PlayNextVideoCommand.Execute(default);

    private void OnPrevButtonClick(object sender, EventArgs e)
        => Source.PlayPrevVideoCommand.Execute(default);
}
