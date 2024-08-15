// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Core;
using Microsoft.UI.Xaml.Controls.Primitives;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Mpv;

/// <summary>
/// Mpv 播放器控件.
/// </summary>
public sealed partial class MpvTransportControl : LayoutControlBase<PlayerViewModel>
{
    private Slider? _progressSlider;
    private Slider? _volumeSlider;
    private Slider? _speedSlider;

    /// <summary>
    /// Initializes a new instance of the <see cref="MpvTransportControl"/> class.
    /// </summary>
    public MpvTransportControl() => DefaultStyleKey = typeof(MpvTransportControl);

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _progressSlider = GetTemplateChild("ProgressSlider") as Slider;
        if (_progressSlider != null)
        {
            _progressSlider.ValueChanged += OnProgressSliderValueChanged;
        }

        _volumeSlider = GetTemplateChild("VolumeSlider") as Slider;
        if (_volumeSlider != null)
        {
            _volumeSlider.ValueChanged += OnVolumeSliderValueChanged;
        }

        _speedSlider = GetTemplateChild("SpeedSlider") as Slider;
        if (_speedSlider != null)
        {
            _speedSlider.ValueChanged += OnSpeedSliderValueChanged;
        }
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (_progressSlider != null)
        {
            _progressSlider.ValueChanged -= OnProgressSliderValueChanged;
            _progressSlider = default;
        }

        if (_volumeSlider != null)
        {
            _volumeSlider.ValueChanged -= OnVolumeSliderValueChanged;
            _volumeSlider = default;
        }

        if (_speedSlider != null)
        {
            _speedSlider.ValueChanged -= OnSpeedSliderValueChanged;
            _speedSlider = default;
        }
    }

    private void OnVolumeSliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        => ViewModel.SetVolumeCommand.Execute(Convert.ToInt32(e.NewValue));

    private void OnProgressSliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        => ViewModel.SeekCommand.Execute(Convert.ToInt32(e.NewValue));

    private void OnSpeedSliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        => ViewModel.SetSpeedCommand.Execute(Math.Round(e.NewValue, 1));
}
