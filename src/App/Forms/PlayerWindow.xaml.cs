// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading.Tasks;
using Bili.Copilot.App.Pages;
using Bili.Copilot.Models.Data.Local;
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
    public PlayerWindow(PlaySnapshot snapshot)
    {
        InitializeComponent();
        Title = snapshot.Title;
        Activated += OnActivated;
        Closed += OnClosedAsync;
        Width = 1280;
        Height = 720;
        AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;

        if (snapshot.VideoType == Models.Constants.Bili.VideoType.Video)
        {
            MainFrame.Navigate(typeof(VideoPlayerPage), snapshot);
        }
    }

    private async void OnClosedAsync(object sender, WindowEventArgs args)
    {
        if (!_isHidden)
        {
            args.Handled = true;
            MainFrame.Navigate(typeof(Page));
            MainWindow.Instance.Activate();
            this.Hide();
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
