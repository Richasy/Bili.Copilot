// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Data.Local;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml.Navigation;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 视频播放页面.
/// </summary>
public sealed partial class VideoPlayerPage : VideoPlayerPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPlayerPage"/> class.
    /// </summary>
    public VideoPlayerPage()
    {
        InitializeComponent();
        ViewModel = new VideoPlayerPageViewModel();
    }

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is PlaySnapshot snapshot)
        {
            ViewModel.SetSnapshot(snapshot);
        }
    }

    /// <inheritdoc/>
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        ViewModel?.Dispose();
        ViewModel = default;
        Bindings.StopTracking();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// <see cref="VideoPlayerPage"/> 的基类.
/// </summary>
public abstract class VideoPlayerPageBase : PageBase<VideoPlayerPageViewModel>
{
}
