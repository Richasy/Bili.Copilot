// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Forms;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using NLog;
using Windows.Storage;

namespace Bili.Copilot.App;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private Window _window;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
        UnhandledException += OnUnhandledException;
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var rootFolder = ApplicationData.Current.LocalFolder;
        var fullPath = $"{rootFolder.Path}\\Logger\\";
        NLog.GlobalDiagnosticsContext.Set("LogPath", fullPath);
        _window = new MainWindow();
        var appWindow = _window.AppWindow;
        appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        appWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        _window.Activate();
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        // This will prevent the application crashing due to some unhandled errors.
        LogManager.GetCurrentClassLogger().Error(e.Exception);
        e.Handled = true;
    }
}
