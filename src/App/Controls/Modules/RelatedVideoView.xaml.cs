// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 关联视频视图.
/// </summary>
public sealed partial class RelatedVideoView : RelatedVideoViewBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RelatedVideoView"/> class.
    /// </summary>
    public RelatedVideoView()
    {
        InitializeComponent();
    }
}

/// <summary>
/// <see cref="RelatedVideoView"/> 的基类.
/// </summary>
public abstract class RelatedVideoViewBase : ReactiveUserControl<VideoPlayerPageViewModel>
{
}
