// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Input;
using Richasy.MpvKernel.WinUI;
using System.ComponentModel;

namespace BiliCopilot.UI.Controls.Core;

public sealed partial class LivePlayerOverlay : MpvPlayerControlBase, IMpvUIElement
{
    private readonly DispatcherQueueTimer _controlTimer;
    private Point _lastCursorPoint;
    private bool _isPointerOnUI;

    public LiveSourceViewModel Source { get; }

    public LivePlayerOverlay(LiveSourceViewModel sourceVM, MpvPlayerWindowViewModel stateVM)
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
        if (_isPointerOnUI || FormatComboBox.IsDropDownOpen)
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
            Source.IsChatSectionPanelVisible = false;

            if (!_controlTimer.IsRunning)
            {
                _controlTimer.Start();
            }
        }
    }

    private void OnFormatChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = (sender as ComboBox).SelectedItem as PlayerFormatItemViewModel;
        if (item is not null && item != Source.SelectedFormat)
        {
            Source.ChangeFormatCommand.Execute(item);
        }
    }

    private void OnChatButtonClick(object sender, RoutedEventArgs e)
    {
        Source.IsChatSectionPanelLoaded = true;
        Source.IsChatSectionPanelVisible = true;
        ViewModel.IsControlVisible = false;
        Source.Chat.ScrollToBottomCommand.Execute(default);
    }

    public void HandleUINotify(MpvUIEventId id, object data)
    {
        if (id == MpvUIEventId.PointerMoved)
        {
            var args = data as PointerRoutedEventArgs;
            _lastCursorPoint = args.GetCurrentPoint(this).Position;
            ViewModel.Window.ShowCursor();

            if (!Source.IsChatSectionPanelVisible)
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

    private void OnDisplayFlyoutClosed(object sender, object e)
       => Source.Danmaku.ResetStyle();
}
