// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.Settings;

/// <summary>
/// 播放器控制设置.
/// </summary>
public sealed partial class PlayerControlSettingControl : SettingsPageControlBase
{
    private bool _isLoaded;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerControlSettingControl"/> class.
    /// </summary>
    public PlayerControlSettingControl() => InitializeComponent();

    protected override void OnControlLoaded()
    {
        StepForwardBox.Value = ViewModel.StepForwardSecond;
        StepBackwardBox.Value = ViewModel.StepBackwardSecond;
        TempSpeedSlider.Value = ViewModel.TempPlaybackRate;
        _isLoaded = true;
    }

    private void OnStepForwardValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (!_isLoaded)
        {
            return;
        }

        ViewModel.StepForwardSecond = Convert.ToInt32(args.NewValue);
    }

    private void OnStepBackwardValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (!_isLoaded)
        {
            return;
        }

        ViewModel.StepBackwardSecond = Convert.ToInt32(args.NewValue);
    }

    private void OnTempSpeedChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        if (!_isLoaded)
        {
            return;
        }

        var v = TempSpeedSlider.Value;
        if (Math.Abs(ViewModel.TempPlaybackRate - v) < 0.05)
        {
            return;
        }

        ViewModel.TempPlaybackRate = v;
    }
}
