// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Core;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Mpv;

/// <summary>
/// Mpv 播放器控件.
/// </summary>
public sealed class MpvTransportControl : LayoutControlBase<PlayerViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MpvTransportControl"/> class.
    /// </summary>
    public MpvTransportControl() => DefaultStyleKey = typeof(MpvTransportControl);
}
