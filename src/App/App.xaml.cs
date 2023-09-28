// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.IO;
using Bili.Copilot.App.Controls;
using Bili.Copilot.App.Forms;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using H.NotifyIcon;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.Windows.AppLifecycle;
using NLog;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Graphics;
using Windows.Storage;
using WinRT.Interop;

namespace Bili.Copilot.App;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// 应用标识符.
    /// </summary>
    public const string Id = "643E7C9C-F215-4883-B5B5-97D5FC3622A5";
    private DispatcherQueue _dispatcherQueue;
    private WindowBase _window;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        // 初始化 App Center.
        var appCenterFilePath = Path.Combine(Package.Current.InstalledLocation.Path, "Assets/AppCenterSecret.txt");
        if (File.Exists(appCenterFilePath))
        {
            var id = File.ReadAllText(appCenterFilePath);
            if (!string.IsNullOrEmpty(id))
            {
                AppCenter.Start(id, typeof(Analytics), typeof(Crashes));
            }
        }

        InitializeComponent();
        var mainAppInstance = Microsoft.Windows.AppLifecycle.AppInstance.FindOrRegisterForKey(Id);
        mainAppInstance.Activated += OnAppInstanceActivated;
        UnhandledException += OnUnhandledException;
    }

    private TaskbarIcon TrayIcon { get; set; }

    private bool HandleCloseEvents { get; set; }

    /// <summary>
    /// Try activating the window and bringing it to the foreground.
    /// </summary>
    public void ActivateWindow(AppActivationArguments arguments = default)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (_window == null)
            {
                LaunchWindow();
            }
            else if (_window.Visible && HandleCloseEvents && arguments?.Data == null)
            {
                _window.Hide();
            }
            else
            {
                _window.Activate();
                Windows.Win32.PInvoke.SetForegroundWindow(new Windows.Win32.Foundation.HWND(WindowNative.GetWindowHandle(_window)));
            }

            try
            {
                if (arguments?.Data is IActivatedEventArgs args)
                {
                    MainWindow.Instance.ActivateArgumentsAsync(args);
                }
            }
            catch (Exception)
            {
            }
        });
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        var rootFolder = ApplicationData.Current.LocalFolder;
        var fullPath = $"{rootFolder.Path}\\Logger\\";
        NLog.GlobalDiagnosticsContext.Set("LogPath", fullPath);

        var instance = Microsoft.Windows.AppLifecycle.AppInstance.FindOrRegisterForKey(Id);
        var eventArgs = instance.GetActivatedEventArgs();
        var data = eventArgs.Data is IActivatedEventArgs
            ? eventArgs.Data as IActivatedEventArgs
            : args.UWPLaunchActivatedEventArgs;

        LaunchWindow(data);
        TraceLogger.Instance.LogAppLaunched();
    }

    private static RectInt32 GetRenderRect(DisplayArea displayArea, IntPtr windowHandle)
    {
        var workArea = displayArea.WorkArea;
        var scaleFactor = Windows.Win32.PInvoke.GetDpiForWindow(new Windows.Win32.Foundation.HWND(windowHandle)) / 96d;
        var previousHeight = SettingsToolkit.ReadLocalSetting(SettingNames.WindowHeight, 800d);
        var width = Convert.ToInt32(500 * scaleFactor);
        var height = Convert.ToInt32(previousHeight * scaleFactor);

        // Ensure the window is not larger than the work area.
        if (height > workArea.Height - 20)
        {
            height = workArea.Height - 20;
        }

        var lastPoint = GetSavedWindowPosition();
        var isZeroPoint = lastPoint.X == 0 && lastPoint.Y == 0;
        var isValidPosition = lastPoint.X >= workArea.X && lastPoint.Y >= workArea.Y;
        var left = isZeroPoint || !isValidPosition
            ? (workArea.Width - width) / 2d
            : lastPoint.X - workArea.X;
        var top = isZeroPoint || !isValidPosition
            ? (workArea.Height - height) / 2d
            : lastPoint.Y - workArea.Y;
        return new RectInt32(Convert.ToInt32(left), Convert.ToInt32(top), width, height);
    }

    private static PointInt32 GetSavedWindowPosition()
    {
        var left = SettingsToolkit.ReadLocalSetting(SettingNames.WindowPositionLeft, 0);
        var top = SettingsToolkit.ReadLocalSetting(SettingNames.WindowPositionTop, 0);
        return new PointInt32(left, top);
    }

    private void InitializeTrayIcon()
    {
        if (TrayIcon != null)
        {
            return;
        }

        var showHideWindowCommand = (XamlUICommand)Resources["ShowHideWindowCommand"];
        showHideWindowCommand.ExecuteRequested += OnShowHideWindowCommandExecuteRequested;

        var exitApplicationCommand = (XamlUICommand)Resources["QuitCommand"];
        exitApplicationCommand.ExecuteRequested += OnQuitCommandExecuteRequested;

        TrayIcon = (TaskbarIcon)Resources["TrayIcon"];
        TrayIcon.ForceCreate();
    }

    private void LaunchWindow(IActivatedEventArgs args = default)
    {
        _window = new MainWindow(args);
        MoveAndResize();
        _window.Closed += OnMainWindowClosedAsync;

        HandleCloseEvents = SettingsToolkit.ReadLocalSetting(SettingNames.HideWhenCloseWindow, true);
        if (HandleCloseEvents)
        {
            InitializeTrayIcon();
        }

        _window.Activate();
    }

    private async void OnMainWindowClosedAsync(object sender, WindowEventArgs args)
    {
        SaveCurrentWindowStats();
        HandleCloseEvents = SettingsToolkit.ReadLocalSetting(SettingNames.HideWhenCloseWindow, true);
        if (HandleCloseEvents)
        {
            args.Handled = true;

            var shouldAsk = SettingsToolkit.ReadLocalSetting(SettingNames.ShouldAskBeforeWindowClosed, true);
            if (shouldAsk)
            {
                _window.Activate();
                var dialog = new CloseWindowTipDialog
                {
                    XamlRoot = _window.Content.XamlRoot,
                };
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.None)
                {
                    return;
                }

                var shouldHide = result == ContentDialogResult.Secondary;
                var type = shouldHide ? "Hide" : "Quit";
                TraceLogger.LogCloseWindowTip(dialog.IsNeverAskChecked, type);
                if (dialog.IsNeverAskChecked)
                {
                    SettingsToolkit.WriteLocalSetting(SettingNames.ShouldAskBeforeWindowClosed, false);
                    SettingsToolkit.WriteLocalSetting(SettingNames.HideWhenCloseWindow, shouldHide);
                }

                if (!shouldHide)
                {
                    ExitApp();
                    return;
                }
            }

            InitializeTrayIcon();
            _window.Hide();
        }
    }

    private void MoveAndResize()
    {
        var hwnd = WindowNative.GetWindowHandle(_window);
        var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        var lastPoint = GetSavedWindowPosition();
        var displayArea = lastPoint.X == 0 && lastPoint.Y == 0
            ? DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Nearest)
            : DisplayArea.GetFromPoint(lastPoint, DisplayAreaFallback.Nearest);
        if (displayArea != null)
        {
            var rect = GetRenderRect(displayArea, hwnd);
            var scaleFactor = Windows.Win32.PInvoke.GetDpiForWindow(new Windows.Win32.Foundation.HWND(hwnd)) / 96d;
            _window.MinWidth = 500;
            _window.MaxWidth = 500;
            _window.MinHeight = 400;

            var maxHeight = (displayArea.WorkArea.Height / scaleFactor) + 16;
            _window.MaxHeight = maxHeight < 400 ? 400 : maxHeight;
            _window.AppWindow.MoveAndResize(rect);
        }
    }

    private void SaveCurrentWindowStats()
    {
        var left = _window.AppWindow.Position.X;
        var top = _window.AppWindow.Position.Y;
        SettingsToolkit.WriteLocalSetting(SettingNames.WindowPositionLeft, left);
        SettingsToolkit.WriteLocalSetting(SettingNames.WindowPositionTop, top);
        SettingsToolkit.WriteLocalSetting(SettingNames.WindowHeight, _window.Height > 400 ? _window.Height : 800);
    }

    private void ExitApp()
    {
        HandleCloseEvents = false;
        TrayIcon?.Dispose();
        _window?.Close();
        Environment.Exit(0);
    }

    private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        var logger = LogManager.GetCurrentClassLogger();
        logger.Error(e.Exception, "An exception occurred while the application was running");
        TraceLogger.Instance.LogUnhandledException(e.Exception);
        e.Handled = true;
    }

    private void OnQuitCommandExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        => ExitApp();

    private void OnShowHideWindowCommandExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        => ActivateWindow();

    private void OnAppInstanceActivated(object sender, AppActivationArguments e)
        => ActivateWindow(e);
}
