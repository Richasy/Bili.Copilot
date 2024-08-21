// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls.Player;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace BiliCopilot.UI.Controls.Core;

/// <summary>
/// 直播播放页控件.
/// </summary>
public sealed partial class LiveTransportControl : LivePlayerPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveTransportControl"/> class.
    /// </summary>
    public LiveTransportControl() => InitializeComponent();

    private void OnVolumeSliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        => ViewModel?.Player.SetVolumeCommand.Execute(Convert.ToInt32(e.NewValue));

    private void OnDisplayFlyoutClosed(object sender, object e)
        => ViewModel.Danmaku.ResetStyle();
}
