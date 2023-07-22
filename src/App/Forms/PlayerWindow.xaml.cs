// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Flyleaf.MediaPlayer;
using Bili.Copilot.Models.Data.Local;
using Microsoft.UI.Xaml;
using Windows.Storage.Pickers;
using WinUIEx;

namespace Bili.Copilot.App.Forms;

/// <summary>
/// 播放器窗口.
/// </summary>
public sealed partial class PlayerWindow : WindowBase
{
    private readonly Player _tempPlayer;
    private readonly PlaySnapshot _snapshot;
    private bool _isActivated;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerWindow"/> class.
    /// </summary>
    public PlayerWindow(PlaySnapshot snapshot)
    {
        InitializeComponent();
        _snapshot = snapshot;
        Activated += OnActivated;
        Closed += OnClosed;
        Width = 800;
        Height = 600;
    }

    private void OnClosed(object sender, WindowEventArgs args)
        => _tempPlayer?.Dispose();

    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        if (_isActivated)
        {
            return;
        }

        this.CenterOnScreen();
        _isActivated = true;
    }

    private async void OnOpenButtonClickAsync(object sender, RoutedEventArgs e)
    {
        // Create a file picker
        var openPicker = new FileOpenPicker();

        // Retrieve the window handle (HWND) of the current WinUI 3 window.
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

        // Initialize the file picker with the window handle (HWND).
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

        // Set options for your file picker
        openPicker.ViewMode = PickerViewMode.Thumbnail;
        openPicker.FileTypeFilter.Add("*");

        // Open the picker for the user to pick a file
        var file = await openPicker.PickSingleFileAsync();
        if (file == null)
        {
            return;
        }

        _tempPlayer.OpenAsync(file.Path);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => MainPlayer.Player = _tempPlayer;
}
