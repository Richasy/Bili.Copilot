// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Forms;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.WinUIKernel.Share.Base;
using Richasy.WinUIKernel.Share.Toolkits;
using Richasy.WinUIKernel.Share.ViewModels;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Storage;
using Windows.System;
using Windows.Win32.UI.WindowsAndMessaging;
using WinUIEx;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 应用视图模型.
/// </summary>
public sealed partial class AppViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppViewModel"/> class.
    /// </summary>
    public AppViewModel(
        ILogger<AppViewModel> logger,
        IBiliTokenResolver tokenResolver)
    {
        _logger = logger;
        _tokenResolver = tokenResolver;
    }

    public bool IsMainWindowVisible()
    {
        var mainWindow = Windows.OfType<MainWindow>().FirstOrDefault();
        if (mainWindow is null)
        {
            return false;
        }

        return mainWindow.Visible && PInvoke.IsIconic(new(mainWindow.GetWindowHandle())) == false && mainWindow.AppWindow.IsVisible;
    }

    [RelayCommand]
    private static void Restart()
    {
        Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().UnregisterKey();
        Microsoft.Windows.AppLifecycle.AppInstance.Restart(default);
    }

    [RelayCommand]
    private async Task InitializeExternalAsync()
    {
        var architecture = RuntimeInformation.ProcessArchitecture;
        var identifier = architecture == Architecture.Arm64 ? "arm64" : "x64";
        var bbdownFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/BBDown/{identifier}/BBDown.exe")).AsTask();
        BBDownPath = bbdownFile.Path;

        var ffmpegFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/ffmpeg/{identifier}/ffmpeg.exe")).AsTask();
        FFmpegPath = ffmpegFile.Path;

        _ = await this.Get<IFontToolkit>().GetFontsAsync();
    }

    [RelayCommand]
    private void HideAllWindows()
    {
        foreach (var wnd in Windows)
        {
            wnd.Hide();
        }
    }

    [RelayCommand]
    private async Task LaunchAsync()
    {
        await LoadOriginalWheelLinesAsync();

        // 清除所有之前的badges.
        //BadgeNotificationManager.Current.ClearBadge();
        if (_tokenResolver.GetToken() is not null)
        {
            IsInitialLoading = true;
            new MainWindow().Activate();
            InitializeExternalCommand.Execute(default);
        }
        else
        {
            new StartupWindow().Activate();
        }
    }

    [RelayCommand]
    private void ChangeTheme(ElementTheme theme)
    {
        foreach (var window in Windows)
        {
            (window.Content as FrameworkElement).RequestedTheme = theme;
        }

        foreach (var player in Players)
        {
            player.UpdateTheme(theme);
        }
    }

    /// <summary>
    /// 显示提示.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    [RelayCommand]
    private async Task ShowTipAsync((string, InfoType) data)
    {
        if (ActivatedWindow is ITipWindow tipWindow)
        {
            await tipWindow.ShowTipAsync(data.Item1, data.Item2);
        }
        else
        {
            var firstWindow = Windows.OfType<ITipWindow>().FirstOrDefault();
            if (firstWindow is not null)
            {
                await firstWindow.ShowTipAsync(data.Item1, data.Item2);
            }
        }
    }

    [RelayCommand]
    private async Task OpenPlayerAsync(MediaSnapshot snapshot)
    {
        var playerVM = this.Get<PlayerViewModel>();
        this.Get<AppViewModel>().Players.Add(playerVM);
        await playerVM.InitializeAsync(snapshot);
    }

    [RelayCommand]
    private void CheckUpate()
    {
        var localVersion = SettingsToolkit.ReadLocalSetting(SettingNames.AppVersion, string.Empty);
        var currentVersion = this.Get<IAppToolkit>().GetPackageVersion();
        if (localVersion != currentVersion)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.AppVersion, currentVersion);
            IsUpdateShown = true;
        }
    }

    [RelayCommand]
    private void HideUpdate()
        => IsUpdateShown = false;

    [RelayCommand]
    private async Task ShowUpdateAsync()
    {
        var packVersion = this.Get<IAppToolkit>().GetPackageVersion();
        var url = $"https://github.com/Richasy/Bili.Copilot/releases/tag/v{packVersion}";
        await Launcher.LaunchUriAsync(new Uri(url));
        HideUpdate();
    }

    [RelayCommand]
    private async Task CheckGpuIsAmdAsync()
    {
        try
        {
            await Task.Run(() =>
            {
                // Initialize the process with required settings
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "wmic",
                    Arguments = "path win32_videocontroller get name",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                using (var process = new Process { StartInfo = processStartInfo })
                {
                    process.Start();
                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    // Process the output to check for AMD GPU
                    var lines = output.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        if (line.Contains("amd", StringComparison.CurrentCultureIgnoreCase))
                        {
                            IsAmdGpu = true;
                            break;
                        }
                    }
                }

                // Write local setting
                IsAmdGpu ??= false;
                SettingsToolkit.WriteLocalSetting(SettingNames.IsGpuChecked, true);
            });

            if (IsAmdGpu == true)
            {
                this.Get<Microsoft.UI.Dispatching.DispatcherQueue>().TryEnqueue(async () =>
                {
                    var dialog = new ContentDialog
                    {
                        Title = ResourceToolkit.GetLocalizedString(StringNames.Tip),
                        Content = ResourceToolkit.GetLocalizedString(StringNames.AmdGpuWarning),
                        XamlRoot = ActivatedWindow.Content.XamlRoot,
                        CloseButtonText = ResourceToolkit.GetLocalizedString(StringNames.Confirm),
                        DefaultButton = ContentDialogButton.Close,
                    };

                    await dialog.ShowAsync();
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查GPU类型时失败");
        }
    }

    [RelayCommand]
    private async Task UseQuickWheelScrollAsync()
    {
        if (_isWheelLinesChanged || !SettingsToolkit.ReadLocalSetting(SettingNames.ScrollAccelerate, true))
        {
            return;
        }

        _isWheelLinesChanged = true;
        await SetWheelLinesAsync(12);
    }

    [RelayCommand]
    private async Task RestoreOriginalWheelScrollAsync()
    {
        if (_isWheelLinesChanged && _originalWheelLines != null)
        {
            _isWheelLinesChanged = false;
            await SetWheelLinesAsync(_originalWheelLines.Value);
        }

        _isWheelLinesChanged = false;
    }

    private async Task LoadOriginalWheelLinesAsync()
    {
        await Task.Run(() =>
        {
            try
            {
                unsafe
                {
                    uint lines = 0;
                    var ok = PInvoke.SystemParametersInfo(
                        SYSTEM_PARAMETERS_INFO_ACTION.SPI_GETWHEELSCROLLLINES,
                        0u,
                        &lines,
                        0);

                    if (ok)
                    {
                        _originalWheelLines = lines == 12 ? 3 : (int)lines;
                        return;
                    }

                    this.Get<ILogger<AppViewModel>>().LogWarning("Get wheel lines via SPI failed, fallback to registry.");
                }
            }
            catch (Exception ex)
            {
                this.Get<ILogger<AppViewModel>>().LogError(ex, "Get wheel lines via SPI exception.");
            }

            try
            {
                using var reg = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", false);
                var value = reg?.GetValue("WheelScrollLines")?.ToString();
                if (uint.TryParse(value, out var regLines))
                {
                    _originalWheelLines = regLines == 12 ? 3 : (int)regLines;
                }
            }
            catch (Exception ex)
            {
                this.Get<ILogger<AppViewModel>>().LogError(ex, "Read wheel lines from registry failed.");
            }
        });
    }

    private async Task SetWheelLinesAsync(int lines)
    {
        if (_originalWheelLines == null)
        {
            return;
        }

        await Task.Run(() =>
        {
            try
            {
                unsafe
                {
                    var result = PInvoke.SystemParametersInfo(SYSTEM_PARAMETERS_INFO_ACTION.SPI_SETWHEELSCROLLLINES, (uint)lines, null, SYSTEM_PARAMETERS_INFO_UPDATE_FLAGS.SPIF_UPDATEINIFILE | SYSTEM_PARAMETERS_INFO_UPDATE_FLAGS.SPIF_SENDCHANGE);
                    if (!result)
                    {
                        this.Get<ILogger<AppViewModel>>().LogWarning("Set wheel lines failed.");
                    }
                }
            }
            catch (Exception ex)
            {
                this.Get<ILogger<AppViewModel>>().LogError(ex, "Set wheel lines exception.");
            }
        });
    }

    partial void OnIsInitialLoadingChanged(bool value)
    {
        if (!value)
        {
            this.Get<NotificationViewModel>().TryStartCommand.Execute(default);
        }
    }
}
