// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Pages;

/// <summary>
/// 视频分区页面.
/// </summary>
public sealed partial class VideoPartitionPage : VideoPartitionPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPartitionPage"/> class.
    /// </summary>
    public VideoPartitionPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => ViewModel.InitializeCommand.Execute(default);
}

/// <summary>
/// 视频分区页面基类.
/// </summary>
public abstract class VideoPartitionPageBase : LayoutPageBase<VideoPartitionPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPartitionPageBase"/> class.
    /// </summary>
    protected VideoPartitionPageBase() => ViewModel = this.Get<VideoPartitionPageViewModel>();
}
