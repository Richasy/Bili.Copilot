// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.App.Args;
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
        DataContext = ViewModel;
    }

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is PlayerPageNavigateEventArgs args)
        {
            ViewModel.SetWindow(args.AttachedWindow);

            if (args.Snapshot != null)
            {
                ViewModel.SetSnapshot(args.Snapshot);
            }
            else if (args.Playlist != null)
            {
                ViewModel.SetPlaylist(args.Playlist);
            }
        }
    }

    /// <inheritdoc/>
    protected override void OnPageUnloaded()
    {
        ViewModel.PlayerDetail.ReportViewProgressCommand.Execute(default);
        ViewModel?.Dispose();
    }

    private void OnSectionHeaderItemInvoked(object sender, Models.App.Other.PlayerSectionHeader e)
    {
        if (ViewModel.CurrentSection != e)
        {
            ViewModel.CurrentSection = e;
        }
    }
}

/// <summary>
/// <see cref="VideoPlayerPage"/> 的基类.
/// </summary>
public abstract class VideoPlayerPageBase : PageBase<VideoPlayerPageViewModel>
{
}
