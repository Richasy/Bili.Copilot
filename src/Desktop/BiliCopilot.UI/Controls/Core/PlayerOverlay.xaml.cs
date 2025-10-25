// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Input;
using Richasy.MpvKernel.Core.Enums;
using Richasy.WinUIKernel.Share.Base;
using Richasy.WinUIKernel.Share.Toolkits;
using System.ComponentModel;

namespace BiliCopilot.UI.Controls.Core;

public sealed partial class PlayerOverlay : PlayerControlBase
{
    private const int MediaTransportControlsHeight = 160;
    private const int HeaderBarHeight = 70;
    private readonly DispatcherQueueTimer _controlTimer;
    private readonly DispatcherQueueTimer _cursorTimer;
    private bool _isTouch;
    private bool _isInTransportControls;
    private bool _isInTitleBar;
    private Point _lastPointerPosition;

    public PlayerOverlay()
    {
        InitializeComponent();
        _controlTimer = DispatcherQueue.CreateTimer();
        _controlTimer.Interval = TimeSpan.FromMilliseconds(1500);
        _controlTimer.Tick += OnControlTimerTick;
        _cursorTimer = DispatcherQueue.CreateTimer();
        _cursorTimer.Interval = TimeSpan.FromMilliseconds(500);
        _cursorTimer.Tick += OnCursorTimerTick;
    }

    public void ShowTip(string text, InfoType type = InfoType.Error)
    {
        DispatcherQueue.TryEnqueue(async () =>
        {
            var popup = new TipPopup() { Text = text };
            TipContainer.Visibility = Visibility.Visible;
            TipContainer.Children.Add(popup);
            await popup.ShowAsync(type);
            TipContainer.Children.Remove(popup);
            TipContainer.Visibility = Visibility.Collapsed;

        });
    }

    internal static Visibility IsTopMostButtonVisible(bool isFullScreen, bool isAot)
        => (isFullScreen || isAot) ? Visibility.Collapsed : Visibility.Visible;

    protected override void OnControlLoaded()
    {
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        ViewModel.WarningOccurred += OnWarningOccurred;
        ViewModel.RequestShowNextTip += OnRequestShowNextTip;
        ViewModel.RequestHideNextTip += OnRequestHideNextTip;
    }

    protected override void OnControlUnloaded()
    {
        ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        ViewModel.WarningOccurred -= OnWarningOccurred;
        ViewModel.RequestShowNextTip -= OnRequestShowNextTip;
        ViewModel.RequestHideNextTip -= OnRequestHideNextTip;
        _controlTimer.Tick -= OnControlTimerTick;
        _controlTimer.Stop();
        _cursorTimer.Tick -= OnCursorTimerTick;
        _cursorTimer.Stop();
        SourceListView?.ItemsSource = default;
    }

    private void OnRequestHideNextTip(object? sender, EventArgs e)
        => NextTip?.Hide();

    private void OnRequestShowNextTip(object? sender, EventArgs e)
        => NextTip?.Show();

    private void OnWarningOccurred(object? sender, string e)
    {
        System.Diagnostics.Debug.WriteLine($"Warning: {e}");
        //var popup = new TipPopup() { Text = e };
        //RootGrid.Children.Add(popup);
        //await popup.ShowAsync(InfoType.Warning);
        //RootGrid.Children.Remove(popup);
    }

    protected override void OnPointerMoved(PointerRoutedEventArgs e)
    {
        var position = e.GetCurrentPoint(this).Position;
        if (position.Equals(_lastPointerPosition))
        {
            // If the pointer hasn't moved, do nothing.
            return;
        }

        _lastPointerPosition = position;
        ViewModel.Window?.ShowCursor();
        _isTouch = e.Pointer.PointerDeviceType is Microsoft.UI.Input.PointerDeviceType.Touch or Microsoft.UI.Input.PointerDeviceType.Pen;
        _cursorTimer.Stop();
        _controlTimer.Stop();
        TestRect.Visibility = Visibility.Visible;
        if (!_isTouch && !ViewModel.IsExtraPanelVisible)
        {
            _isInTitleBar = position.Y <= HeaderBarHeight && position.Y >= 0;
            _isInTransportControls = position.Y >= ActualHeight - MediaTransportControlsHeight;
            if (_isInTitleBar || _isInTransportControls)
            {
                // If the pointer is in the title bar or transport controls, show controls immediately.
                _cursorTimer.Start();
                ViewModel.IsControlsVisible = true;
            }
            else
            {
                _controlTimer.Start();
                if (!ViewModel.IsEnd)
                {
                    ViewModel.IsControlsVisible = false;
                }
            }
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.IsControlsVisible))
        {
            if (!ViewModel.IsControlsVisible && !_controlTimer.IsRunning)
            {
                _controlTimer.Start();
            }
            else if (!_cursorTimer.IsRunning)
            {
                _cursorTimer.Start();
            }
        }
        else if (e.PropertyName == nameof(ViewModel.CurrentVolume))
        {
            var thumbHeight = VolumeContainer.ActualHeight * (ViewModel.CurrentVolume / ViewModel.MaxVolume);
            VolumeThumb?.Height = Math.Max(0, thumbHeight);
        }
    }

    private void OnControlTimerTick(DispatcherQueueTimer sender, object args)
    {
        if (_isTouch)
        {
            return;
        }

        _controlTimer.Stop();
        TestRect.Visibility = Visibility.Collapsed;
        ViewModel.Window?.HideCursor();
    }

    private void OnCursorTimerTick(DispatcherQueueTimer sender, object args)
    {
        if (!IsCursorInWindow() && !_isTouch)
        {
            ViewModel.IsControlsVisible = false;
            _cursorTimer.Stop();
        }
    }

    private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        => ViewModel.Close();

    private void OnCloseTipButtonClick(object sender, RoutedEventArgs e)
        => ViewModel.ErrorMessage = default;

    private void OnBackRequested(object sender, EventArgs e)
        => ViewModel.BackCommand.Execute(default);

    private void OnSubtitleSettingItemClick(object sender, RoutedEventArgs e)
    {
        ViewModel.IsSubtitleSettingsVisible = false;
        ViewModel.IsSubtitleSettingsVisible = true;
    }

    private void OnSubtitlePositionChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        if (ViewModel.Player?.IsPlaybackInitialized != true || ViewModel.SubtitlePosition == e.NewValue)
        {
            return;
        }

        ViewModel.ChangeSubtitlePositionCommand.Execute(e.NewValue);
    }

    private void OnHeaderBarSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width == 0)
        {
            return;
        }

        try
        {
            DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
            {
                if (e.NewSize.Width < 700)
                {
                    VisualStateManager.GoToState(this, nameof(NarrowState), false);
                    HeaderBar.TitleMaxWidth = e.NewSize.Width > 200 ? e.NewSize.Width - 200 : 120;
                }
                else
                {
                    VisualStateManager.GoToState(this, nameof(DefaultState), false);
                    HeaderBar.TitleMaxWidth = e.NewSize.Width - 520;
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnHeaderBarSizeChanged: {ex.Message}");
        }
    }

    private bool IsCursorInWindow()
    {
        var isSuccess = PInvoke.GetCursorPos(out var point);
        if (!isSuccess)
        {
            return false;
        }

        var window = ViewModel.Window?.GetWindow();
        if (window?.IsVisible != true)
        {
            return false;
        }

        var handle = ViewModel.Window!.Handle;
        isSuccess = PInvoke.GetClientRect(new(handle), out var clientRect);
        if (!isSuccess)
        {
            return false;
        }

        var lt = new System.Drawing.Point(clientRect.X, clientRect.Y);
        isSuccess = PInvoke.ClientToScreen(new(handle), ref lt);
        if (!isSuccess)
        {
            return false;
        }

        var rb = new System.Drawing.Point(clientRect.X + clientRect.Width, clientRect.Y + clientRect.Height);
        isSuccess = PInvoke.ClientToScreen(new(handle), ref rb);
        if (!isSuccess)
        {
            return false;
        }

        return point.X >= lt.X && point.X <= rb.X && point.Y >= lt.Y && point.Y <= rb.Y;
    }

    private void OnSubtitleFontSizeChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        if (ViewModel?.Player == null || ViewModel.Player?.IsPlaybackInitialized != true || ViewModel.SubtitleFontSize == e.NewValue)
        {
            return;
        }

        ViewModel.ChangeSubtitleFontSizeCommand.Execute(e.NewValue);
    }

#pragma warning disable CA1822
    private string GetTimeText(double seconds)
        => TimeSpan.FromSeconds(seconds).ToString(@"hh\:mm\:ss");

    private string GetVolumeText(double volume)
        => Math.Round(volume).ToString();

    private string GetSpeedText(double speed)
        => $"{Math.Round(speed, 2)}x";
#pragma warning restore CA1822

    private void OnSubtitleFontChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = e.AddedItems.FirstOrDefault() as SystemFont;
        if (ViewModel?.Player == null || ViewModel.Player?.IsPlaybackInitialized != true || item == null || ViewModel.SubtitleFontFamily == item.LocalName)
        {
            return;
        }

        ViewModel.ChangeSubtitleFontFamilyCommand.Execute(item.LocalName);
    }

    private void OnSubtitleDelayChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        var v = args.NewValue;
        if (v == ViewModel.SubtitleDelaySeconds)
        {
            return;
        }

        ViewModel.SetSubtitleDelaySecondsCommand.Execute(v);
    }

    internal static bool IsPaused(MpvPlayerState state)
    {
        return state is MpvPlayerState.Paused;
    }

    internal static bool IsLoading(MpvPlayerState state, bool isLoading)
    {
        return (state is MpvPlayerState.Seeking or MpvPlayerState.Buffering) || isLoading;
    }

    internal static Visibility IsBottomProgressShouldDisplay(bool isAot, bool isFullScreen, bool isLoading, MpvPlayerState state)
    {
        if ((isAot || isFullScreen) && !IsLoading(state, isLoading))
        {
            return Visibility.Collapsed;
        }

        return Visibility.Visible;
    }

    private void OnSourceItemClick(object sender, RoutedEventArgs e)
    {
        var selectedSource = (sender as FrameworkElement)?.DataContext as SourceItemViewModel;
        selectedSource?.ActiveCommand.Execute(default);
        SourceFlyout?.Hide();
    }

    private void OnSubtitleFontComboLoaded(object sender, RoutedEventArgs e)
    {
        var font = ViewModel.SubtitleFontFamily;
        var source = ViewModel.Fonts.FirstOrDefault(p => p.LocalName == font);
        if (source != null)
        {
            SubtitleFontComboBox.SelectedItem = source;
        }
    }
}
