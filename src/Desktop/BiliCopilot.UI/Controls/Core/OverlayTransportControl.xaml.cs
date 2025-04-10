// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using Richasy.MpvKernel.Core.Enums;

namespace BiliCopilot.UI.Controls.Core;

/// <summary>
/// 视频播放器控件.
/// </summary>
public sealed partial class OverlayTransportControl : MpvPlayerControlBase
{
    public static readonly DependencyProperty LeftElementProperty =
        DependencyProperty.Register(nameof(LeftElement), typeof(object), typeof(OverlayTransportControl), new PropertyMetadata(default));

    public static readonly DependencyProperty IsNextButtonEnabledProperty =
        DependencyProperty.Register(nameof(IsNextButtonEnabled), typeof(bool), typeof(OverlayTransportControl), new PropertyMetadata(default));

    public static readonly DependencyProperty IsPrevButtonEnabledProperty =
        DependencyProperty.Register(nameof(IsPrevButtonEnabled), typeof(bool), typeof(OverlayTransportControl), new PropertyMetadata(default));

    public static readonly DependencyProperty IsNavigateButtonVisibleProperty =
        DependencyProperty.Register(nameof(IsNavigateButtonVisible), typeof(bool), typeof(OverlayTransportControl), new PropertyMetadata(default));

    public static readonly DependencyProperty NextButtonTipProperty =
        DependencyProperty.Register(nameof(NextButtonTip), typeof(string), typeof(OverlayTransportControl), new PropertyMetadata(default));

    public static readonly DependencyProperty PrevButtonTipProperty =
        DependencyProperty.Register(nameof(PrevButtonTip), typeof(string), typeof(OverlayTransportControl), new PropertyMetadata(default));

    public OverlayTransportControl() => InitializeComponent();

    public event EventHandler PrevButtonClick;

    public event EventHandler NextButtonClick;

    public object LeftElement
    {
        get => (object)GetValue(LeftElementProperty);
        set => SetValue(LeftElementProperty, value);
    }

    public bool IsNextButtonEnabled
    {
        get => (bool)GetValue(IsNextButtonEnabledProperty);
        set => SetValue(IsNextButtonEnabledProperty, value);
    }

    public bool IsPrevButtonEnabled
    {
        get => (bool)GetValue(IsPrevButtonEnabledProperty);
        set => SetValue(IsPrevButtonEnabledProperty, value);
    }

    public bool IsNavigateButtonVisible
    {
        get => (bool)GetValue(IsNavigateButtonVisibleProperty);
        set => SetValue(IsNavigateButtonVisibleProperty, value);
    }

    public string NextButtonTip
    {
        get => (string)GetValue(NextButtonTipProperty);
        set => SetValue(NextButtonTipProperty, value);
    }

    public string PrevButtonTip
    {
        get => (string)GetValue(PrevButtonTipProperty);
        set => SetValue(PrevButtonTipProperty, value);
    }

    private FluentIcons.Common.Symbol GetPlayPauseSymbol(MpvPlayerState state)
        => state == MpvPlayerState.Playing ? FluentIcons.Common.Symbol.Pause : FluentIcons.Common.Symbol.Play;

    private FluentIcons.Common.Symbol GetCompactOverlaySymbol(bool isCompactOverlay)
        => isCompactOverlay ? FluentIcons.Common.Symbol.ContractDownLeft : FluentIcons.Common.Symbol.ContractUpRight;

    private FluentIcons.Common.Symbol GetFullScreenSymbol(bool isFullScreen)
        => isFullScreen ? FluentIcons.Common.Symbol.FullScreenMinimize : FluentIcons.Common.Symbol.FullScreenMaximize;

    private bool IsPlayPauseEnabled(MpvPlayerState state)
        => state is MpvPlayerState.Playing or MpvPlayerState.Paused or MpvPlayerState.End;

    private bool IsLoading(MpvPlayerState state)
        => state is MpvPlayerState.Seeking or MpvPlayerState.Buffering;

    private string GetTimeText(double seconds)
        => TimeSpan.FromSeconds(seconds).ToString(@"hh\:mm\:ss");

    private string GetSpeedText(double speed)
        => $"{Math.Round(speed, 1)}x";

    private async void OnPlayPauseButtonClick(object sender, RoutedEventArgs e)
    {
        if (ViewModel.LastState is MpvPlayerState.Playing)
        {
            await ViewModel.Client.PauseAsync();
        }
        else if (ViewModel.LastState is MpvPlayerState.Paused)
        {
            await ViewModel.Client.ResumeAsync();
        }
        else if(ViewModel.LastState is MpvPlayerState.End)
        {
            await ViewModel.Client.ReplayAsync();
            await ViewModel.Client.ResumeAsync();
        }
    }

    private async void OnProgressValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        var v = e.NewValue;
        if (Math.Abs(v - ViewModel.CurrentPosition) > 1.5)
        {
            if (ViewModel.LastState is MpvPlayerState.End)
            {
                await ViewModel.Client.ReplayAsync(v);
                await ViewModel.Client.ResumeAsync();
                return;
            }

            await ViewModel.Client.SetCurrentPositionAsync(v);
        }
    }

    private async void OnBackwardButtonClick(object sender, RoutedEventArgs e)
    {
        var nextPos = ViewModel.CurrentPosition - 10;
        if (nextPos < 0)
        {
            nextPos = 0;
        }

        await ViewModel.Client.SetCurrentPositionAsync(nextPos);
    }

    private async void OnForwardButtonClick(object sender, RoutedEventArgs e)
    {
        var nextPos = ViewModel.CurrentPosition + 30;
        if (nextPos > ViewModel.Duration)
        {
            nextPos = ViewModel.Duration - 0.1;
        }

        await ViewModel.Client.SetCurrentPositionAsync(nextPos);
    }

    private async void OnVolumeValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        var newValue = e.NewValue;
        if (ViewModel?.Client is null)
        {
            return;
        }

        if (Math.Abs(newValue - ViewModel.Volume) < 1)
        {
            return;
        }

        await ViewModel.Client.SetVolumeAsync(newValue);
    }

    private async void OnSpeedValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        var newValue = e.NewValue;
        if (ViewModel?.Client is null)
        {
            return;
        }

        if (Math.Abs(newValue - ViewModel.Speed) < 0.1)
        {
            return;
        }

        await ViewModel.Client.SetSpeedAsync(newValue);
    }

    private async void OnCompactOverlayButtonClick(object sender, RoutedEventArgs e)
        => await ViewModel.Client.SetCompactOverlayState(!ViewModel.IsCompactOverlay);

    private async void OnFullScreenButtonClick(object sender, RoutedEventArgs e)
        => await ViewModel.Client.SetFullScreenState(!ViewModel.IsFullScreen);

    private void OnPrevButtonClick(object sender, RoutedEventArgs e)
        => PrevButtonClick?.Invoke(sender, EventArgs.Empty);

    private void OnNextButtonClick(object sender, RoutedEventArgs e)
        => NextButtonClick?.Invoke(sender, EventArgs.Empty);

    private string GetCompactOverlayTip(bool isCompactOverlay)
        => isCompactOverlay ? ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.ExitCompactOverlay) : ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.EnterCompactOverlay);

    private string GetFullScreenTip(bool isFullScreen)
        => isFullScreen ? ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.ExitFullScreen) : ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.EnterFullScreen);
}
