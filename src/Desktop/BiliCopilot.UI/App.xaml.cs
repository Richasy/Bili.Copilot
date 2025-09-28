// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Forms;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using H.NotifyIcon;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Input;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using Richasy.BiliKernel.Models.Media;
using Serilog;
using System.Text.Json;
using Windows.Storage;

namespace BiliCopilot.UI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private const string Id = "Richasy.BiliCopilot";
    private readonly DispatcherQueue _dispatcherQueue;
    private AppNotificationManager _notificationManager;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
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

            instance.Activated += OnInstanceActivated;
            GlobalDependencies.Initialize();
            GlobalDependencies.Kernel.GetRequiredService<AppViewModel>().LaunchCommand.Execute(default);
        }
        else
        {
            var activatedArgs = instance.GetActivatedEventArgs();
            _ = instance.RedirectActivationToAsync(activatedArgs).AsTask().ContinueWith(_ => Current.Exit());
        }
    }

    private static MainWindow? GetMainWindow()
        => GlobalDependencies.Kernel.GetRequiredService<AppViewModel>().Windows.Find(p => p is MainWindow) as MainWindow;

    private void OnInstanceActivated(object? sender, AppActivationArguments e)
    {
        _dispatcherQueue.TryEnqueue(() => GetMainWindow()?.Activate());
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
            var logger = Log.Logger.ForContext<App>();
            logger.Error("初始化任务栏图标失败");
        }
    }

    private void ExitApp()
    {
        TrayIcon?.Dispose();
        TrayIcon = null;
        _notificationManager?.Unregister();
        _notificationManager = null;
        Exit();
    }

    private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        this.Get<AppViewModel>().RestoreOriginalWheelScrollCommand.Execute(default);
        this.Get<ILogger<App>>().LogError(e.Exception, "Unhandled exception occurred.");
        e.Handled = true;

        // 一旦出现布局循环检测异常，就删除上次选中的功能页，然后重启应用.
        if (e.Message.Contains("Layout cycle detected"))
        {
            SettingsToolkit.DeleteLocalSetting(Models.Constants.SettingNames.LastSelectedFeaturePage);
            this.Get<AppViewModel>().RestartCommand.Execute(default);
        }
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
                var identifier = JsonSerializer.Deserialize(argsStr, GlobalSerializeContext.Default.MediaIdentifier);
                if (type == "video")
                {
                    this.Get<AppViewModel>().OpenPlayerCommand.Execute(new MediaSnapshot(new VideoInformation(identifier, default)));
                }
                else if (type == "episode")
                {
                    this.Get<AppViewModel>().OpenPlayerCommand.Execute(new MediaSnapshot(default, new EpisodeInformation(identifier)));
                }
            });
        }
    }

    private void OnQuitCommandExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        => ExitApp();

    private void OnShowHideWindowCommandExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        => GetMainWindow()?.Activate();
}
