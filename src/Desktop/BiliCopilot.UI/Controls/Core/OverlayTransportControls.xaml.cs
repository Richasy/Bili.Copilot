// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using Richasy.MpvKernel.Core.Enums;

namespace BiliCopilot.UI.Controls.Core;

public sealed partial class OverlayTransportControls : PlayerControlBase
{
    public static readonly DependencyProperty LeftElementProperty =
        DependencyProperty.Register(nameof(LeftElement), typeof(object), typeof(OverlayTransportControls), new PropertyMetadata(default));

    public static readonly DependencyProperty ProgressBarVisibleProperty =
        DependencyProperty.Register(nameof(ProgressBarVisible), typeof(bool), typeof(OverlayTransportControls), new PropertyMetadata(true));

    public static readonly DependencyProperty IsSkipButtonsVisibleProperty =
        DependencyProperty.Register(nameof(IsSkipButtonsVisible), typeof(bool), typeof(OverlayTransportControls), new PropertyMetadata(true));

    private readonly List<double> _volumeChangeList = [];
    private readonly List<double> _speedChangeList = [];
    private readonly List<double> _progressChangeList = [];
    private readonly DispatcherTimer _volumeChangeTimer;
    private readonly DispatcherTimer _speedChangeTimer;
    private readonly DispatcherTimer _progressChangeTimer;
    private bool _isFirstChangeVolume = true;

    public OverlayTransportControls()
    {
        InitializeComponent();
        _volumeChangeTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5) };
        _speedChangeTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5) };
        _progressChangeTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5) };
        _volumeChangeTimer.Tick += OnVolumeChangeTick;
        _speedChangeTimer.Tick += OnSpeedChangeTick;
        _progressChangeTimer.Tick += OnProgressChangeTick;
    }

    public object LeftElement
    {
        get => (object)GetValue(LeftElementProperty);
        set => SetValue(LeftElementProperty, value);
    }

    public bool ProgressBarVisible
    {
        get => (bool)GetValue(ProgressBarVisibleProperty);
        set => SetValue(ProgressBarVisibleProperty, value);
    }

    public bool IsSkipButtonsVisible
    {
        get => (bool)GetValue(IsSkipButtonsVisibleProperty);
        set => SetValue(IsSkipButtonsVisibleProperty, value);
    }

    protected override void OnControlLoaded()
    {
        BalanceLeftRightWidth();
    }

    protected override void OnViewModelChanged(PlayerViewModel? oldValue, PlayerViewModel? newValue)
    {
        if (oldValue != null)
        {
            oldValue.ProgressChanged -= OnViewModelProgressChanged;
        }

        if (newValue != null)
        {
            newValue.ProgressChanged += OnViewModelProgressChanged;
        }
    }

    protected override void OnControlUnloaded()
    {
        _volumeChangeTimer?.Stop();
        _speedChangeTimer?.Stop();
        _progressChangeTimer?.Stop();
        if (_volumeChangeTimer != null)
        {
            _volumeChangeTimer.Tick -= OnVolumeChangeTick;
        }

        if (_speedChangeTimer != null)
        {
            _speedChangeTimer.Tick -= OnSpeedChangeTick;
        }

        if (_progressChangeTimer != null)
        {
            _progressChangeTimer.Tick -= OnProgressChangeTick;
        }

        ViewModel.ProgressChanged -= OnViewModelProgressChanged;
        SubtitleRepeater?.ItemsSource = null;
    }

#pragma warning disable CA1822
    private FluentIcons.Common.Symbol GetPlayPauseSymbol(MpvPlayerState state)
        => state == MpvPlayerState.Playing ? FluentIcons.Common.Symbol.Pause : FluentIcons.Common.Symbol.Play;

    private FluentIcons.Common.Symbol GetCompactOverlaySymbol(bool isCompactOverlay)
        => isCompactOverlay ? FluentIcons.Common.Symbol.ContractDownLeft : FluentIcons.Common.Symbol.ContractUpRight;

    private FluentIcons.Common.Symbol GetFullScreenSymbol(bool isFullScreen)
        => isFullScreen ? FluentIcons.Common.Symbol.FullScreenMinimize : FluentIcons.Common.Symbol.FullScreenMaximize;

    private bool IsPlayPauseEnabled(MpvPlayerState state)
        => state is MpvPlayerState.Playing or MpvPlayerState.Paused or MpvPlayerState.End;

    private string GetPlayPauseTip(bool isPlaying)
        => isPlaying ? ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.Pause) : ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.Play);

    private bool IsLoading(MpvPlayerState state, bool isLoading)
        => state is MpvPlayerState.Seeking or MpvPlayerState.Buffering || isLoading;

    private string GetTimeText(double seconds)
        => TimeSpan.FromSeconds(seconds).ToString(@"hh\:mm\:ss");

    private string GetSpeedText(double speed)
        => $"{ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.PlaybackRate)}: {Math.Round(speed, 2)}x";

    private string GetCompactOverlayTip(bool isCompactOverlay)
        => isCompactOverlay ? ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.ExitCompactOverlay) : ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.EnterCompactOverlay);

    private string GetFullScreenTip(bool isFullScreen)
        => isFullScreen ? ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.ExitFullScreen) : ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.EnterFullScreen);

    private void OnViewModelProgressChanged(object? sender, double e)
    {
        if (_progressChangeList.Count > 0)
        {
            return;
        }

        if (ProgressSlider.Maximum != ViewModel.Player.Duration)
        {
            ProgressSlider.Maximum = ViewModel.Player.Duration;
        }

        if (ProgressSlider.Value != e && !double.IsNaN(e) && !double.IsInfinity(e))
        {
            ProgressSlider.Value = e;
        }
    }

    private async void OnPlayPauseButtonClick(object sender, RoutedEventArgs e)
    {
        if (ViewModel.Player.PlaybackState is MpvPlayerState.Playing)
        {
            await ViewModel.Client!.PauseAsync();
        }
        else if (ViewModel.Player.PlaybackState is MpvPlayerState.Paused)
        {
            await ViewModel.Client!.ResumeAsync();
        }
        else if (ViewModel.Player.PlaybackState is MpvPlayerState.End)
        {
            await ViewModel.Player.ReplayAsync();
        }
    }

    private void OnProgressValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        var v = e.NewValue;
        if (ProgressSlider.Maximum != ViewModel.Player.Duration || !ViewModel.Player.IsPlaybackInitialized)
        {
            return;
        }

        if (Math.Abs(v - ViewModel.Player.Position) < 1.5)
        {
            return;
        }

        _progressChangeList.Add(v);
        _progressChangeTimer.Stop();
        _progressChangeTimer.Start();
    }

    private async void OnVolumeValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        var newValue = e.NewValue;
        if (ViewModel?.Client is null || ViewModel.Player?.IsPlaybackInitialized != true)
        {
            return;
        }

        if (_isFirstChangeVolume && (ViewModel.Player.Volume > 100 && newValue == 100))
        {
            await Task.Delay(400);
            ViewModel.Player.RaisePropertyChanged(nameof(ViewModel.Player.Volume));
            _isFirstChangeVolume = false;
            return;
        }

        if (Math.Abs(newValue - ViewModel.Player.Volume) < 1)
        {
            return;
        }

        _volumeChangeList.Add(newValue);
        _volumeChangeTimer.Stop();
        _volumeChangeTimer.Start();
    }

    private void OnSpeedValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        var newValue = e.NewValue;
        CheckSpeedButtonState();
        if (ViewModel?.Client is null || ViewModel.Player?.IsPlaybackInitialized != true)
        {
            return;
        }

        if (Math.Abs(newValue - ViewModel.Player.PlaybackRate) < 0.03)
        {
            return;
        }

        _speedChangeList.Add(newValue);
        _speedChangeTimer.Stop();
        _speedChangeTimer.Start();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var width = e.NewSize.Width;
        var miniWidth = ViewModel.IsDanmakuControlVisible ? 900 : 760;
        if (width < miniWidth)
        {
            VisualStateManager.GoToState(this, nameof(NarrowState), false);
        }
        else if (width < 1100)
        {
            VisualStateManager.GoToState(this, nameof(MediumState), false);
        }
        else if (width < 1280)
        {
            VisualStateManager.GoToState(this, nameof(BelowWideState), false);
        }
        else
        {
            VisualStateManager.GoToState(this, nameof(WideState), false);
        }

        BalanceLeftRightWidth();
    }

    private void BalanceLeftRightWidth()
    {
        var width = Math.Min(LeftHolderGrid.ActualWidth, RightPanel.ActualWidth);
        if (width == 0 && RightPanel.ActualWidth > 0)
        {
            width = RightPanel.ActualWidth;
        }

        if (width < 300 && LeftHolderGrid.ActualWidth > 300)
        {
            width = 300;
        }

        LeftPresenter.Width = width;
    }

    private async void OnSpeedChangeTick(object? sender, object e)
    {
        var lastSpeed = _speedChangeList.LastOrDefault();
        _speedChangeList.Clear();
        if (Math.Abs(lastSpeed - ViewModel.Player.PlaybackRate) > 0.1)
        {
            await ViewModel.Client!.SetSpeedAsync(lastSpeed);
            CheckSpeedButtonState();
        }

        _speedChangeList.Clear();
        _speedChangeTimer.Stop();
    }

    private async void OnProgressChangeTick(object? sender, object e)
    {
        var lastProgress = _progressChangeList.LastOrDefault();
        _progressChangeList.Clear();
        if (Math.Abs(lastProgress - ViewModel.Player.Position) >= 1.5)
        {
            if (ViewModel.IsNextTipShown)
            {
                ViewModel.HideNextTipCommand.Execute(default);
            }

            if (ViewModel.Player.PlaybackState is MpvPlayerState.End)
            {
                await ViewModel.Player.ReplayAsync(lastProgress);
            }
            else
            {
                await ViewModel.Client!.SetCurrentPositionAsync(lastProgress);
            }
        }

        _progressChangeList.Clear();
        _progressChangeTimer.Stop();
    }

    private async void OnVolumeChangeTick(object? sender, object e)
    {
        var lastVolume = _volumeChangeList.LastOrDefault();
        _volumeChangeList.Clear();
        if (Math.Abs(lastVolume - ViewModel.Player.Volume) > 1)
        {
            await ViewModel.Client!.SetVolumeAsync(lastVolume);
        }

        _volumeChangeList.Clear();
        _volumeChangeTimer.Stop();
    }

    private void OnSubtitleFlyoutOpened(object sender, object e)
    {
        ViewModel.IsPopupVisible = true;
    }

    private void OnDisplayFlyoutClosed(object sender, object e)
    {
        ViewModel.IsPopupVisible = false;
        ViewModel.Danmaku.ResetStyle();
    }

    private void OnRightPanelSizeChanged(object sender, SizeChangedEventArgs e)
        => BalanceLeftRightWidth();

    private async void CheckSpeedButtonState()
    {
        if (ViewModel is null)
        {
            return;
        }

        await Task.Delay(300);
        var speed = ViewModel.Player?.PlaybackRate ?? 0;
        HalfSpeedButton.IsChecked = false;
        DefaultSpeedButton.IsChecked = false;
        OneTwoFiveSpeedButton.IsChecked = false;
        OneHalfSpeedButton.IsChecked = false;
        DoubleSpeedButton.IsChecked = false;
        TripleSpeedButton.IsChecked = false;
        if (Math.Abs(speed - 0.5) < 0.01)
        {
            HalfSpeedButton.IsChecked = true;
        }
        else if (Math.Abs(speed - 1) < 0.01)
        {
            DefaultSpeedButton.IsChecked = true;
        }
        else if (Math.Abs(speed - 1.25) < 0.01)
        {
            OneTwoFiveSpeedButton.IsChecked = true;
        }
        else if (Math.Abs(speed - 1.5) < 0.01)
        {
            OneHalfSpeedButton.IsChecked = true;
        }
        else if (Math.Abs(speed - 2) < 0.01)
        {
            DoubleSpeedButton.IsChecked = true;
        }
        else if (Math.Abs(speed - 3) < 0.01)
        {
            TripleSpeedButton.IsChecked = true;
        }
    }

    private async void OnQuickSpeedButtonClick(object sender, RoutedEventArgs e)
    {
        var speed = Convert.ToDouble((sender as FrameworkElement)?.Tag);
        _speedChangeList.Clear();
        _speedChangeTimer.Stop();
        if (Math.Abs(speed - ViewModel.Player.PlaybackRate) > 0.1)
        {
            await ViewModel.Client!.SetSpeedAsync(speed);
        }
        else
        {
            CheckSpeedButtonState();
        }
    }

    private void OnFlyoutClosed(object sender, object e)
    {
        ViewModel.IsPopupVisible = false;
    }

    private void OnDisplayFlyoutOpened(object sender, object e)
    {
        ViewModel.IsPopupVisible = true;
    }
#pragma warning restore CA1822
}
