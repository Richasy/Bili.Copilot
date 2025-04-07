// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Microsoft.Extensions.Logging;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Richasy.MpvKernel;
using Richasy.MpvKernel.Core;
using Richasy.MpvKernel.Core.Enums;
using Richasy.MpvKernel.Core.Models;
using Richasy.MpvKernel.WinUI;
using Windows.Graphics;
using WinRT;

namespace BiliCopilot.UI.ViewModels.Core;

[GeneratedBindableCustomProperty]
public sealed partial class MpvPlayerViewModel : PlayerViewModelBase
{
    public async Task InitializeAsync()
    {
        if (Client != null)
        {
            return;
        }

        IsPlayerInitializing = true;
        Client = await MpvClient.CreateAsync(logger: this.Get<ILogger<MpvPlayerViewModel>>());
        Client.DataNotify += OnDataNotify;
        await Client.SetLogLevelAsync(MpvLogLevel.Warn);
        await Client.UseIdleAsync(true);
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
            // TODO: 添加配置.
        }

        await InitializeDecodeAsync();

        _playerWindow = new MpvPlayerWindow(Client, this.Get<DispatcherQueue>());
        MoveAndResize();
        var wnd = _playerWindow.GetWindow();
        wnd.Closing += OnClosing;
        wnd.TitleBar.ExtendsContentIntoTitleBar = true;
        wnd.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        wnd.TitleBar.PreferredTheme = Microsoft.UI.Windowing.TitleBarTheme.UseDefaultAppMode;
        _playerWindow.Show();
        var isMaximized = SettingsToolkit.ReadLocalSetting(SettingNames.IsPlayerWindowMaximized, false);
        if (isMaximized)
        {
            (wnd.Presenter as OverlappedPresenter).Maximize();
        }

        IsPlayerInitializing = false;
        _isInitialized = true;

        RaiseInitializedEvent();
        await TryLoadPlayDataAsync();
    }

    private void OnDataNotify(object? sender, MpvClientNotifyEventArgs e)
    {
        this.Get<DispatcherQueue>().TryEnqueue(() =>
        {
            switch (e.Id)
            {
                case MpvClientEventId.StateChanged:
                    _lastState = (MpvPlayerState)e.Data;
                    break;
                case MpvClientEventId.VolumeChanged:
                    var volume = (double)e.Data;
                    Volume = Convert.ToInt32(volume);
                    break;
                case MpvClientEventId.DurationChanged:
                    var duration = (double)e.Data;
                    Duration = Convert.ToInt32(duration);
                    break;
                case MpvClientEventId.PositionChanged:
                    var position = (double)e.Data;
                    Position = Convert.ToInt32(position);
                    break;
            }
        });
    }

    private void OnClosing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        _playerWindow.GetWindow().Closing -= OnClosing;
        SaveCurrentWindowStats();
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
        _playerWindow.GetWindow().MoveAndResize(rect);
    }

    private RectInt32 GetRenderRect(RectInt32 workArea)
    {
        var scaleFactor = PInvoke.GetDpiForWindow(new(Win32Interop.GetWindowFromWindowId(_playerWindow.GetWindow().Id))) / 96d;
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
        var wnd = _playerWindow.GetWindow();
        var left = wnd.Position.X;
        var top = wnd.Position.Y;
        var isMaximized = PInvoke.IsZoomed(new(Win32Interop.GetWindowFromWindowId(_playerWindow.GetWindow().Id)));
        SettingsToolkit.WriteLocalSetting(SettingNames.IsPlayerWindowMaximized, (bool)isMaximized);

        if (!isMaximized)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.PlayerWindowPositionLeft, left);
            SettingsToolkit.WriteLocalSetting(SettingNames.PlayerWindowPositionTop, top);

            if (wnd.Size.Height >= WindowMinHeight && wnd.Size.Width >= WindowMinWidth)
            {
                SettingsToolkit.WriteLocalSetting(SettingNames.PlayerWindowHeight, wnd.Size.Height);
                SettingsToolkit.WriteLocalSetting(SettingNames.PlayerWindowWidth, wnd.Size.Width);
            }
        }
    }

    private static PointInt32 GetSavedWindowPosition()
    {
        var left = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowPositionLeft, 0);
        var top = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowPositionTop, 0);
        return new PointInt32(left, top);
    }
}
