// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Richasy.MpvKernel;
using Richasy.MpvKernel.Core;
using Richasy.MpvKernel.Core.Enums;
using Richasy.MpvKernel.WinUI;
using Richasy.WinUIKernel.Share.ViewModels;
using Windows.Graphics;

namespace BiliCopilot.UI.ViewModels.Core;

public sealed partial class MpvPlayerWindowViewModel : ViewModelBase
{
    public MpvPlayerWindowViewModel(
        IMediaSourceResolver sourceResolver,
        IMediaUIProvider? uiProvider)
    {
        _logger = this.Get<ILogger<MpvPlayerWindowViewModel>>();
        _queue = this.Get<DispatcherQueue>();
        _sourceResolver = sourceResolver;
        _uiProvider = uiProvider;
        _tipTimer = _queue.CreateTimer();
        _tipTimer.Interval = TimeSpan.FromSeconds(1);
        _tipTimer.Tick += OnTipTimerTick;
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Client is not null)
        {
            return;
        }

        _uiProvider?.SetWindowViewModel(this);
        Client = await MpvClient.CreateAsync();
        Client.DataNotify += OnClientDataNotify;
        Client.ReachFileLoading += OnFileLoading;
        Client.ReachFileLoaded += OnFileLoaded;
        await Client.SetLogLevelAsync(MpvLogLevel.Warn);
        await Client.UseIdleAsync(true);

        await InitializeConfigAsync();
        await InitializeDecodeAsync();

        InitializeWindow();

        try
        {
            IsSourceLoading = true;
            await _sourceResolver.InitializeAsync();
            IsSourceLoading = false;
        }
        catch (Exception ex)
        {
            IsSourceLoading = false;
            _logger.LogError(ex, "加载媒体源失败.");
            _uiProvider?.ShowError("加载媒体源失败.", ex.Message);
            return;
        }

        await LoadMediaAsync();
        _sourceResolver.RequestReload += OnRequestReload;
    }

    private async Task InitializeConfigAsync()
    {
        var decodeType = SettingsToolkit.ReadLocalSetting(SettingNames.PreferDecode, PreferDecodeType.Auto);
        string configFilePath = default;
        if (decodeType == PreferDecodeType.Custom)
        {
            var localFolderPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
            configFilePath = System.IO.Path.Combine(localFolderPath, "mpv.conf");
            if (!System.IO.File.Exists(configFilePath))
            {
                await File.WriteAllTextAsync(configFilePath, string.Empty);
            }
        }

        if (!string.IsNullOrEmpty(configFilePath))
        {
            await Client.SetConfigFileAsync(configFilePath);
        }
    }

    private async Task InitializeDecodeAsync()
    {
        var decodeType = PreferDecodeType.Auto;
        try
        {
            decodeType = SettingsToolkit.ReadLocalSetting(SettingNames.PreferDecode, PreferDecodeType.Auto);
        }
        catch (Exception)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.PreferDecode, PreferDecodeType.Auto);
        }

        if (decodeType == PreferDecodeType.Auto)
        {
            await Client.SetVideoOutputAsync(VideoOutputType.Gpu);
            await Client.SetGpuContextAsync(GpuContextType.Auto);
            await Client.SetHardwareDecodeAsync(HardwareDecodeType.Auto);
        }
        else if (decodeType == PreferDecodeType.D3D11)
        {
            await Client.SetVideoOutputAsync(VideoOutputType.Gpu);
            await Client.SetGpuContextAsync(GpuContextType.D3D11);
            await Client.SetHardwareDecodeAsync(HardwareDecodeType.D3D11va);
        }
        else if (decodeType == PreferDecodeType.NVDEC)
        {
            await Client.SetVideoOutputAsync(VideoOutputType.Gpu);
            await Client.SetGpuContextAsync(GpuContextType.Auto);
            await Client.SetHardwareDecodeAsync(HardwareDecodeType.Nvdec);
        }
        else if (decodeType == PreferDecodeType.Vulkan)
        {
            await Client.SetVideoOutputAsync(VideoOutputType.GpuNext);
            await Client.SetGpuContextAsync(GpuContextType.WindowsVulkan);
            await Client.SetHardwareDecodeAsync(HardwareDecodeType.Vulkan);
        }
        else if (decodeType == PreferDecodeType.DXVA2)
        {
            await Client.SetVideoOutputAsync(VideoOutputType.Gpu);
            await Client.SetGpuContextAsync(GpuContextType.D3D11);
            await Client.SetHardwareDecodeAsync(HardwareDecodeType.Dxva2);
        }
    }

    private void MoveAndResize()
    {
        var lastPoint = GetSavedWindowPosition();
        var displayArea = DisplayArea.GetFromPoint(lastPoint, DisplayAreaFallback.Primary)
            ?? DisplayArea.Primary;
        var rect = GetRenderRect(displayArea.WorkArea);
        Window.GetWindow().MoveAndResize(rect);
    }

    private void InitializeWindow()
    {
        Window = new MpvPlayerWindow(Client, this.Get<DispatcherQueue>());
        Window.UINotify += OnUINotify;
        MoveAndResize();
        var wnd = Window.GetWindow();
        wnd.TitleBar.ExtendsContentIntoTitleBar = true;
        wnd.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        wnd.TitleBar.ButtonForegroundColor = Colors.Transparent;
        wnd.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;
        wnd.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

        if (wnd.Presenter is OverlappedPresenter presenter)
        {
            var scaleFactor = PInvoke.GetDpiForWindow(new(Win32Interop.GetWindowFromWindowId(wnd.Id))) / 96d;
            presenter.PreferredMinimumWidth = Convert.ToInt32(WindowMinWidth * scaleFactor);
            presenter.PreferredMinimumHeight = Convert.ToInt32(WindowMinHeight * scaleFactor);
        }

        var isMaximized = SettingsToolkit.ReadLocalSetting(SettingNames.IsPlayerWindowMaximized, false);
        if (isMaximized)
        {
            (wnd.Presenter as OverlappedPresenter).Maximize();
        }

        wnd.Closing += OnWindowClosing;

        if (_uiProvider != null)
        {
            var element = _uiProvider.GetUIElement();
            IsControlVisible = true;
            Window.SetUIElement(element);
        }

        Window.Show();
    }

    private async Task LoadMediaAsync()
    {
        if (Client is null)
        {
            return;
        }

        var (url, options) = _sourceResolver.GetSource();
        options.WindowHandle = Window.Handle;
        options.InitialVolume = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerVolume, 100);
        options.InitialSpeed = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerSpeed, 1d);
        Window.GetWindow().Title = _sourceResolver.GetTitle();
        await Client.PlayAsync(url, options);
    }

    private RectInt32 GetRenderRect(RectInt32 workArea)
    {
        var scaleFactor = PInvoke.GetDpiForWindow(new(Win32Interop.GetWindowFromWindowId(Window.GetWindow().Id))) / 96d;
        var previousWidth = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowWidth, 1120d);
        var previousHeight = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowHeight, 740d);
        var width = Convert.ToInt32(previousWidth * scaleFactor);
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
            : lastPoint.X;
        var top = isZeroPoint || !isValidPosition
            ? (workArea.Height - height) / 2d
            : lastPoint.Y;
        return new RectInt32(Convert.ToInt32(left), Convert.ToInt32(top), width, height);
    }

    private void SaveCurrentWindowStats()
    {
        var wnd = Window.GetWindow();
        var scaleFactor = PInvoke.GetDpiForWindow(new(Win32Interop.GetWindowFromWindowId(wnd.Id))) / 96d;
        var left = wnd.Position.X;
        var top = wnd.Position.Y;
        var isMaximized = PInvoke.IsZoomed(new(Win32Interop.GetWindowFromWindowId(Window.GetWindow().Id)));
        SettingsToolkit.WriteLocalSetting(SettingNames.IsPlayerWindowMaximized, (bool)isMaximized);

        if (!isMaximized)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.PlayerWindowPositionLeft, left);
            SettingsToolkit.WriteLocalSetting(SettingNames.PlayerWindowPositionTop, top);

            if (wnd.Size.Height >= WindowMinHeight && wnd.Size.Width >= WindowMinWidth)
            {
                SettingsToolkit.WriteLocalSetting(SettingNames.PlayerWindowHeight, (wnd.Size.Height / scaleFactor) * 1d);
                SettingsToolkit.WriteLocalSetting(SettingNames.PlayerWindowWidth, (wnd.Size.Width / scaleFactor) * 1d);
            }
        }
    }

    private static PointInt32 GetSavedWindowPosition()
    {
        var left = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowPositionLeft, 0);
        var top = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowPositionTop, 0);
        return new PointInt32(left, top);
    }

    private void CheckBackdropVisible()
        => IsBackdropVisible = IsFileLoading || IsIdle || IsSourceLoading;

    partial void OnIsFileLoadingChanged(bool value) => CheckBackdropVisible();

    partial void OnIsIdleChanged(bool value) => CheckBackdropVisible();

    partial void OnIsSourceLoadingChanged(bool value) => CheckBackdropVisible();

    partial void OnIsProgressChangingChanged(bool value)
    {
        if (!IsControlVisible)
        {
            IsControlVisible = true;
        }
    }
}
