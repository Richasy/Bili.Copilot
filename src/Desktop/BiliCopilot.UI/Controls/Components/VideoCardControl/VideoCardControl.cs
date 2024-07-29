// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 视频卡片控件.
/// </summary>
public sealed class VideoCardControl : LayoutControlBase<VideoItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoCardControl"/> class.
    /// </summary>
    public VideoCardControl()
    {
        DefaultStyleKey = typeof(VideoCardControl);
    }
}
