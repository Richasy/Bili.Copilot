// Copyright (c) Bili Copilot. All rights reserved.

using System.Diagnostics;
using System.Text.Json;
using Bili.Copilot.App.Controls;
using Bili.Copilot.App.Forms;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Constants.Community;
using Bili.Copilot.Models.Data.Local;
using Bili.Copilot.Models.Data.Pgc;
using Bili.Copilot.Models.Data.Video;
using Bili.Copilot.ViewModels;
using H.NotifyIcon;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using NLog;
using Windows.ApplicationModel.Activation;
using Windows.Storage;

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
    private readonly AppNotificationManager _notificationManager;
    private DispatcherQueue _dispatcherQueue;
    private WindowBase _window;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        // 初始化 App Center.
        var appCenterFilePath = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets/AppCenterSecret.txt");
        if (File.Exists(appCenterFilePath))
        {
            var id = File.ReadAllText(appCenterFilePath);
            if (!string.IsNullOrEmpty(id))
            {
                AppCenter.Start(id, typeof(Analytics), typeof(Crashes));
            }
        }

        InitializeComponent();
        var mainAppInstance = AppInstance.FindOrRegisterForKey(Id);
        mainAppInstance.Activated += OnAppInstanceActivated;
        UnhandledException += OnUnhandledException;
        _notificationManager = AppNotificationManager.Default;
        _notificationManager.NotificationInvoked += OnAppNotificationInvoked;
        _notificationManager.Register();
    }

    private TaskbarIcon TrayIcon { get; set; }

    private bool HandleCloseEvents { get; set; }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        var instance = AppInstance.FindOrRegisterForKey(Id);
        if (instance.IsCurrent)
        {
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            var rootFolder = ApplicationData.Current.LocalFolder;
            var fullPath = $"{rootFolder.Path}\\Logger\\";
            NLog.GlobalDiagnosticsContext.Set("LogPath", fullPath);
        }

        var eventArgs = instance.GetActivatedEventArgs();
        var data = eventArgs.Data is IActivatedEventArgs
            ? eventArgs.Data as IActivatedEventArgs
            : args.UWPLaunchActivatedEventArgs;

        await LaunchWindowAsync(data);
        TraceLogger.Instance.LogAppLaunched();
    }

    /// <summary>
    /// Try activating the window and bringing it to the foreground.
    /// </summary>
    private void ActivateWindow(AppActivationArguments arguments = default)
    {
        _ = _dispatcherQueue.TryEnqueue(async () =>
        {
            if (_window == null)
            {
                await LaunchWindowAsync();
            }
            else if (_window.Visible && HandleCloseEvents && arguments?.Data == null)
            {
                _ = _window.Hide();
            }
            else
            {
                _window.Activate();
                _ = _window.SetForegroundWindow();
            }
        });
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

        try
        {
            TrayIcon = (TaskbarIcon)Resources["TrayIcon"];
            TrayIcon.ForceCreate();
        }
        catch (Exception)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Error("Failed to initialize tray icon");
        }
    }

    private async Task LaunchWindowAsync(IActivatedEventArgs args = default)
    {
        if (args is IProtocolActivatedEventArgs protocolArgs
            && !string.IsNullOrEmpty(protocolArgs.Uri.Host))
        {
            // 处理协议启动.
        }
        else
        {
            var instance = AppInstance.FindOrRegisterForKey(Id);

            // If the current instance is not the previously registered instance
            if (!instance.IsCurrent)
            {
                var activatedArgs = AppInstance.GetCurrent().GetActivatedEventArgs();

                // Redirect to the existing instance
                await instance.RedirectActivationToAsync(activatedArgs);

                // Kill the current instance
                Current.Exit();
                return;
            }

            var isSignedIn = await AuthorizeProvider.Instance.IsTokenValidAsync();
            if (!isSignedIn)
            {
                var window = new SignInWindow();
                window.Activate();
            }
            else
            {
                _window = new MainWindow();
                _window.Closed += OnMainWindowClosedAsync;

                HandleCloseEvents = SettingsToolkit.ReadLocalSetting(SettingNames.HideWhenCloseWindow, true);
                if (HandleCloseEvents)
                {
                    InitializeTrayIcon();
                }

                _window.Activate();
            }
        }
    }

    private async void OnMainWindowClosedAsync(object sender, WindowEventArgs args)
    {
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
            _ = _window.Hide();
        }
    }

    private void ExitApp()
    {
        HandleCloseEvents = false;
        TrayIcon?.Dispose();
        _window?.Close();
        _notificationManager?.Unregister();
        Process.GetCurrentProcess().Kill();
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

    private void OnAppNotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
    {
        if (args.Arguments.TryGetValue("type", out var type))
        {
            if (type == "dynamic")
            {
                // 视频动态更新.
                var payload = args.Arguments["payload"];
                var playSnapshot = JsonSerializer.Deserialize<PlaySnapshot>(payload);
                _dispatcherQueue.TryEnqueue(() =>
                {
                    AppViewModel.Instance.OpenPlayerCommand.Execute(playSnapshot);
                });
            }
        }
    }
}
