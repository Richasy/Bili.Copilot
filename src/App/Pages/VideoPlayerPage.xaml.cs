﻿// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.ViewModels;

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
    protected override async void OnPageUnloaded()
    {
        try
        {
            ViewModel.PlayerDetail.Player?.Dispose();
            await ViewModel.PlayerDetail.ReportViewProgressCommand.ExecuteAsync(default);
            ViewModel?.Dispose();
            ViewModel = null;
        }
        catch (Exception)
        {
        }
    }

    private void OnSectionHeaderItemInvoked(object sender, Models.App.Other.PlayerSectionHeader e)
    {
        if (ViewModel.CurrentSection != e)
        {
            ViewModel.CurrentSection = e;
        }
    }

    private void OnBackButtonClick(object sender, RoutedEventArgs e)
    {
        if (ViewModel.PlayerDetail.DisplayMode == PlayerDisplayMode.CompactOverlay)
        {
            ViewModel.PlayerDetail.ToggleCompactOverlayModeCommand.Execute(default);
        }
        else if (ViewModel.PlayerDetail.DisplayMode == PlayerDisplayMode.FullScreen)
        {
            ViewModel.PlayerDetail.ToggleFullScreenModeCommand.Execute(default);
        }

        AppViewModel.Instance.BackCommand.Execute(default);
    }
}

/// <summary>
/// <see cref="VideoPlayerPage"/> 的基类.
/// </summary>
public abstract class VideoPlayerPageBase : PageBase<VideoPlayerPageViewModel>
{
}
