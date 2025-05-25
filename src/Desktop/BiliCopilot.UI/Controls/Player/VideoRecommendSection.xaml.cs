// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// 视频推荐区域.
/// </summary>
public sealed partial class VideoRecommendSection : VideoRecommendSectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoRecommendSection"/> class.
    /// </summary>
    public VideoRecommendSection() => InitializeComponent();

    protected override void OnControlUnloaded()
        => VideoRepeater.ItemsSource = null;
}

/// <summary>
/// 视频推荐区域基类.
/// </summary>
public abstract class VideoRecommendSectionBase : LayoutUserControlBase<VideoPlayerRecommendSectionDetailViewModel>
{
}
