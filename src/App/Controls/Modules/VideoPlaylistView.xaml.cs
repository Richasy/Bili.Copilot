// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 视频播放列表视图.
/// </summary>
public sealed partial class VideoPlaylistView : VideoPlaylistViewBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPlaylistView"/> class.
    /// </summary>
    public VideoPlaylistView() => InitializeComponent();
}

/// <summary>
/// <see cref="VideoPlaylistView"/> 的基类.
/// </summary>
public abstract class VideoPlaylistViewBase : ReactiveUserControl<VideoPlayerPageViewModel>
{
}
