// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls.Mpv.Common;
using Mpv.Core;

namespace BiliCopilot.UI.Controls.Mpv;

/// <summary>
/// 播放器.
/// </summary>
public sealed partial class MpvPlayer
{
    private RenderControl _renderControl;
    private TextBlock _positionBlock;
    private Button _playPauseButton;
    private Button _skipForwardButton;
    private Button _skipBackwardButton;
    private ComboBox _playRateComboBox;

    /// <summary>
    /// 播放器.
    /// </summary>
    public Player Player { get; private set; }
}
