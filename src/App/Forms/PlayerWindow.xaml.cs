﻿// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Threading.Tasks;
using Bili.Copilot.App.Pages;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Local;
using Bili.Copilot.Models.Data.Video;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIEx;

namespace Bili.Copilot.App.Forms;

/// <summary>
/// 播放器窗口.
/// </summary>
public sealed partial class PlayerWindow : WindowBase
{
    private bool _isActivated;
    private bool _isHidden;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerWindow"/> class.
    /// </summary>
    public PlayerWindow()
    {
        InitializeComponent();
        Activated += OnActivated;
        Closed += OnClosedAsync;
        Width = 1280;
        Height = 720;
        MinWidth = 560;
        MinHeight = 320;
        AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerWindow"/> class.
    /// </summary>
    public PlayerWindow(PlaySnapshot snapshot)
        : this()
    {
        Title = snapshot.Title;
        var navArgs = new PlayerPageNavigateEventArgs
        {
            Snapshot = snapshot,
            AttachedWindow = this,
        };
        if (snapshot.VideoType == Models.Constants.Bili.VideoType.Video)
        {
            _ = MainFrame.Navigate(typeof(VideoPlayerPage), navArgs);
        }
        else if (snapshot.VideoType == Models.Constants.Bili.VideoType.Live)
        {
            _ = MainFrame.Navigate(typeof(LivePlayerPage), navArgs);
        }
        else if (snapshot.VideoType == Models.Constants.Bili.VideoType.Pgc)
        {
            _ = MainFrame.Navigate(typeof(PgcPlayerPage), navArgs);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerWindow"/> class.
    /// </summary>
    public PlayerWindow(List<VideoInformation> snapshots)
        : this()
    {
        Title = ResourceToolkit.GetLocalizedString(StringNames.Playlist);
        var navArgs = new PlayerPageNavigateEventArgs
        {
            Playlist = snapshots,
            AttachedWindow = this,
        };

        _ = MainFrame.Navigate(typeof(VideoPlayerPage), navArgs);
    }

    private async void OnClosedAsync(object sender, WindowEventArgs args)
    {
        if (!_isHidden)
        {
            args.Handled = true;
            _ = MainFrame.Navigate(typeof(Page));
            MainWindow.Instance.Activate();
            _ = this.Hide();
            _isHidden = true;
            await Task.Delay(1000);
            Close();
        }
    }

    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        if (_isActivated)
        {
            return;
        }

        this.CenterOnScreen();
        _isActivated = true;
    }
}
