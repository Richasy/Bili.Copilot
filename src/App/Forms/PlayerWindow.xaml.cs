// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.App.Pages;
using Bili.Copilot.Models.Data.Local;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIEx;

namespace Bili.Copilot.App.Forms;

/// <summary>
/// 播放器窗口.
/// </summary>
public sealed partial class PlayerWindow : WindowBase
{
    private readonly PlaySnapshot _snapshot;
    private bool _isActivated;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerWindow"/> class.
    /// </summary>
    public PlayerWindow(PlaySnapshot snapshot)
    {
        InitializeComponent();
        _snapshot = snapshot;
        Title = snapshot.Title;
        Activated += OnActivated;
        Closed += OnClosed;
        Width = 1280;
        Height = 720;

        if (snapshot.VideoType == Models.Constants.Bili.VideoType.Video)
        {
            MainFrame.Navigate(typeof(VideoPlayerPage), snapshot);
        }
    }

    private void OnClosed(object sender, WindowEventArgs args)
    {
        MainFrame.Navigate(typeof(Page));
        GC.SuppressFinalize(this);
        GC.Collect();
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
