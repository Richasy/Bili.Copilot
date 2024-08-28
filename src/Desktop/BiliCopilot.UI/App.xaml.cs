// Copyright (c) Bili Copilot. All rights reserved.

using System.Text.Json;
using BiliCopilot.UI.Forms;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using H.NotifyIcon;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Input;
using Microsoft.Windows.AppNotifications;
using NLog;
using Richasy.BiliKernel.Models.Media;
using Windows.Storage;

namespace BiliCopilot.UI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private const string Id = "Richasy.BiliCopilot";
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly AppNotificationManager _notificationManager;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        FluentIcons.WinUI.Extensions.UseSegoeMetrics(this);
        UnhandledException += OnUnhandledException;
        _notificationManager = AppNotificationManager.Default;
        _notificationManager.NotificationInvoked += OnAppNotificationInvoked;
        _notificationManager.Register();
    }

    /// <summary>
    /// 日志文件夹.
    /// </summary>
    public static string LoggerFolder { get; private set; }

    private TaskbarIcon TrayIcon { get; set; }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        var instance = Microsoft.Windows.AppLifecycle.AppInstance.FindOrRegisterForKey(Id);
        if (instance.IsCurrent)
        {
            var rootFolder = ApplicationData.Current.LocalFolder;
            var fullPath = $"{rootFolder.Path}\\Logger";
            LoggerFolder = fullPath;
            if (!Directory.Exists(fullPath))
            {
                _ = Directory.CreateDirectory(fullPath);
            }

            NLog.GlobalDiagnosticsContext.Set("LogPath", fullPath);
            GlobalDependencies.Initialize();
            GlobalDependencies.Kernel.GetRequiredService<AppViewModel>().LaunchCommand.Execute(default);
            var mainWindow = GetMainWindow();
            mainWindow.Closed += OnMainWindowClosed;
        }
        else
        {
            var activatedArgs = instance.GetActivatedEventArgs();
            _ = instance.RedirectActivationToAsync(activatedArgs).AsTask().ContinueWith(_ => Current.Exit());
        }
    }

    private static MainWindow? GetMainWindow()
        => GlobalDependencies.Kernel.GetRequiredService<AppViewModel>().Windows.Find(p => p is MainWindow) as MainWindow;

    private void OnMainWindowClosed(object sender, WindowEventArgs args)
    {
        var hideWhenClose = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.HideWhenCloseWindow, false);
        if (hideWhenClose)
        {
            if (TrayIcon is null)
            {
                InitializeTrayIcon();
            }

            GetMainWindow()?.Hide();
        }
        else
        {
            ExitApp();
        }
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
            logger.Error("初始化任务栏图标失败");
        }
    }

    private void ExitApp()
    {
        TrayIcon?.Dispose();
        TrayIcon = null;
        _notificationManager?.Unregister();
        Exit();
    }

    private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        if (!e.Message.Contains("Layout cycle"))
        {
            e.Handled = true;
        }

        GlobalDependencies.Kernel.GetRequiredService<ILogger<App>>().LogCritical(e.Exception, "Unhandled exception occurred.");
    }

    private void OnAppNotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
    {
        if (args.Arguments.TryGetValue("page", out var type))
        {
            var argsStr = args.Arguments.TryGetValue("args", out var a) ? a : string.Empty;
            if (string.IsNullOrEmpty(argsStr))
            {
                return;
            }

            _dispatcherQueue.TryEnqueue(() =>
            {
                var identifier = JsonSerializer.Deserialize<MediaIdentifier>(argsStr);
                var obj = type.Contains("VideoPlayer") ? (object)new VideoSnapshot(new VideoInformation(identifier, default)) : identifier;
                GlobalDependencies.Kernel.GetRequiredService<NavigationViewModel>().NavigateToOver(type, obj);
            });
        }
    }

    private void OnQuitCommandExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        => ExitApp();

    private void OnShowHideWindowCommandExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        => GetMainWindow()?.Activate();
}
