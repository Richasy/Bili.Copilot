// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 分区页面.
/// </summary>
public sealed partial class VideoPartitionPage : VideoPartitionPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPartitionPage"/> class.
    /// </summary>
    public VideoPartitionPage()
    {
        InitializeComponent();
        ViewModel = VideoPartitionModuleViewModel.Instance;
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        CoreViewModel.IsBackButtonShown = ViewModel.IsInDetail;
        CoreViewModel.BackRequest += OnCoreViewModelBackRequest;
    }

    /// <inheritdoc/>
    protected override void OnPageUnloaded()
        => CoreViewModel.BackRequest -= OnCoreViewModelBackRequest;

    private void OnCoreViewModelBackRequest(object sender, EventArgs e)
        => ViewModel.ClosePartitionDetailCommand.Execute(default);
}

/// <summary>
/// <see cref="VideoPartitionPage"/> 的基类.
/// </summary>
public abstract class VideoPartitionPageBase : PageBase<VideoPartitionModuleViewModel>
{
}
