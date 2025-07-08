// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Forms;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Humanizer;
using Microsoft.UI.Windowing;
using Richasy.MpvKernel;
using Richasy.MpvKernel.Core;
using Richasy.MpvKernel.Core.Enums;
using Richasy.MpvKernel.Core.Models;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// Mpv播放器视图模型.
/// </summary>
public sealed partial class MpvPlayerViewModel
{
    /// <summary>
    /// 初始化播放器.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    private async Task InitializeInternalAsync()
    {
        //if (Connector is null)
        //{
        //    Connector = _snapshot.Connector switch
        //    {
        //        ConnectorType.Local => this.Get<LocalPlayerConnectorViewModel>(),
        //        ConnectorType.WebDav => this.Get<WebDavPlayerConnectorViewModel>(),
        //        ConnectorType.Smb => this.Get<SmbPlayerConnectorViewModel>(),
        //        ConnectorType.Emby => this.Get<EmbyPlayerConnectorViewModel>(),
        //        ConnectorType.Jellyfin => this.Get<JellyfinPlayerConnectorViewModel>(),
        //        _ => default,
        //    };

        //    if (Connector != null)
        //    {
        //        Connector.PlaylistInitialized += OnConnectorPlaylistInitialized;
        //        Connector.NewMediaRequest += OnConnectorNewMediaRequest;
        //        Connector.RequestOpenExtraPanel += OnRequestOpenExtraPanel;
        //    }
        //}

        Subtitles.Clear();
        Audios.Clear();
        IsAudioSelectable = false;
        IsSubtitleEmpty = true;

        if (Client?.IsDisposed != false)
        {
            var architecture = RuntimeInformation.ProcessArchitecture;
            var identifier = architecture == Architecture.Arm64 ? "arm64" : "x64";
            var mpvPath = Path.Combine(Package.Current.InstalledPath, "Assets", "libmpv", identifier, "libmpv-2.dll");
            var configPath = string.Empty;
            var decodeType = SettingsToolkit.ReadLocalSetting(SettingNames.PreferDecodeType, PreferDecodeType.Auto);
            if (decodeType == PreferDecodeType.Custom)
            {
                configPath = await AppToolkit.EnsureMpvConfigExistAsync();
            }

            var options = new MpvInitializeOptions
            {
                UseConfig = false,
            };
            Client = await MpvClient.CreateAsync(mpvPath, options, logger: logger);
            Client.ErrorOccurred += OnClientErrorOccurred;
            Client.CacheStateChanged += OnClientCacheStateChanged;

            if (decodeType == PreferDecodeType.Auto)
            {
                await Client.SetVideoOutputAsync(VideoOutputType.GpuNext);
                await Client.SetGpuContextAsync(GpuContextType.Auto);
                await Client.SetHardwareDecodeAsync(HardwareDecodeType.Auto);
            }
            else if (decodeType == PreferDecodeType.D3D11)
            {
                await Client.SetVideoOutputAsync(VideoOutputType.GpuNext);
                await Client.SetGpuContextAsync(GpuContextType.D3D11);
                await Client.SetHardwareDecodeAsync(HardwareDecodeType.D3D11va);
            }
            else if (decodeType == PreferDecodeType.NVDEC)
            {
                await Client.SetVideoOutputAsync(VideoOutputType.GpuNext);
                await Client.SetGpuContextAsync(GpuContextType.D3D11);
                await Client.SetHardwareDecodeAsync(HardwareDecodeType.Nvdec);
            }
            else if (decodeType == PreferDecodeType.Vulkan)
            {
                await Client.SetVideoOutputAsync(VideoOutputType.GpuNext);
                await Client.SetGpuContextAsync(GpuContextType.WindowsVulkan);
                await Client.SetHardwareDecodeAsync(HardwareDecodeType.Vulkan);
            }
            else if (decodeType == PreferDecodeType.Software)
            {
                await Client.SetHardwareDecodeAsync(HardwareDecodeType.None);
            }

            if (!string.IsNullOrEmpty(configPath))
            {
                await Client.SetConfigFileAsync(configPath);
            }
            else
            {
                // 未使用自定义配置文件时，使用附加设置.
                var autoProfileLoadBehavior = SettingsToolkit.ReadLocalSetting(SettingNames.AutoProfileLoadBehavior, -1);
                await Client.SetLoadAutoProfilesAsync(autoProfileLoadBehavior == -1 ? null : Convert.ToBoolean(autoProfileLoadBehavior));
                var preferAudioChannelLayout = SettingsToolkit.ReadLocalSetting(SettingNames.PreferAudioChannelLayout, Richasy.MpvKernel.Core.Enums.AudioChannelLayoutType.Auto);
                if (preferAudioChannelLayout != AudioChannelLayoutType.Auto)
                {
                    await Client.SetAudioChannelLayoutAsync(preferAudioChannelLayout);
                }

                var cacheSize = SettingsToolkit.ReadLocalSetting(SettingNames.MaxCacheSize, 300d);
                var cacheSeconds = SettingsToolkit.ReadLocalSetting(SettingNames.MaxCacheSeconds, 360d);
                await Client.SetDemuxerMaxBytesAsync($"{Convert.ToInt32(cacheSize)}MiB");
                await Client.SetDemuxerReadheadSecondsAsync(Convert.ToInt32(cacheSeconds));
                var proxyType = SettingsToolkit.ReadLocalSetting(SettingNames.MpvProxyType, MpvProxyType.System);
                if (proxyType == MpvProxyType.System)
                {
                    var proxy = AppToolkit.GetSystemProxy();
                    if (!string.IsNullOrEmpty(proxy))
                    {
                        await Client.SetHttpProxyAsync(proxy);
                    }
                }
            }

            var logLevel = SettingsToolkit.ReadLocalSetting(SettingNames.MpvLogLevel, MpvLogLevel.Warn);
            await Client.SetLogLevelAsync(logLevel);
            await Client.UseIdleAsync(true);
            await Client.UseKeepOpenAsync(true);
            await Client.SetTlsVerifyAsync(false);
            await Client.SetAutoCreatePlaylistAsync(AutoCreatePlaylistKind.No);
        }
    }

    private void OnClientCacheStateChanged(object? sender, MpvCacheStateEventArgs e)
        => CacheStateChanged?.Invoke(this, e);

    private void OnClientErrorOccurred(object? sender, MpvError e)
    {
        queue.TryEnqueue(() =>
        {
            var errorContent = e switch
            {
                MpvError.Nomem => ResourceToolkit.GetLocalizedString(StringNames.MpvErrorNoMemory),
                MpvError.Uninitialized => ResourceToolkit.GetLocalizedString(StringNames.MpvErrorUninitialized),
                MpvError.LoadingFailed => ResourceToolkit.GetLocalizedString(StringNames.MpvErrorLoadingFailed),
                MpvError.AoInitFailed => ResourceToolkit.GetLocalizedString(StringNames.MpvErrorAudioFailed),
                MpvError.VoInitFailed => ResourceToolkit.GetLocalizedString(StringNames.MpvErrorVideoFailed),
                MpvError.NothingToPlay => ResourceToolkit.GetLocalizedString(StringNames.MpvErrorNotingToPlay),
                MpvError.UnknownFormat => ResourceToolkit.GetLocalizedString(StringNames.MpvErrorUnknownFormat),
                MpvError.Unsupported => ResourceToolkit.GetLocalizedString(StringNames.MpvErrorUnsupported),
                _ => default,
            };

            if (!string.IsNullOrEmpty(errorContent))
            {
                ErrorMessage = errorContent;
            }
            else
            {
                WarningOccurred?.Invoke(this, ResourceToolkit.GetLocalizedString(StringNames.MpvErrorGeneric));
            }

            CheckBlackBackgroundVisible();
        });
    }

    private void CheckBlackBackgroundVisible()
        => IsBlackBackgroundVisible = Player?.IsPlaybackInitialized != true || !string.IsNullOrEmpty(ErrorMessage);

    private async Task ResetPlayerPresenterModeAsync()
    {
        if (Window is null || Window.GetWindow() == null || Window.IsClosed)
        {
            return;
        }

        var currentWindowPresenter = Window!.GetWindow().Presenter.Kind;
        if (currentWindowPresenter == AppWindowPresenterKind.FullScreen)
        {
            await Client!.SetFullScreenStateAsync(true);
        }
        else if (currentWindowPresenter == AppWindowPresenterKind.CompactOverlay)
        {
            await Client!.SetCompactOverlayStateAsync(true);
        }
    }

    private void CheckFullScreen()
    {
        if (Window is null || Client is null || Window.IsClosed)
        {
            return;
        }

        if (Player.IsFullScreen && Window.GetWindow().Presenter.Kind != AppWindowPresenterKind.FullScreen)
        {
            Window.EnterFullScreen();
        }
        else if (!Player.IsFullScreen && Window.GetWindow().Presenter.Kind == AppWindowPresenterKind.FullScreen)
        {
            Window.ExitFullScreen();
        }
    }

    private void CheckCompactOverlay()
    {
        if (Window is null || Window.IsClosed)
        {
            return;
        }

        if (Player.IsCompactOverlay && Window.GetWindow().Presenter.Kind != AppWindowPresenterKind.CompactOverlay)
        {
            Window.EnterCompactOverlay();
        }
        else if (!Player.IsCompactOverlay && Window.GetWindow().Presenter.Kind == AppWindowPresenterKind.CompactOverlay)
        {
            Window.ExitCompactOverlay();
        }
    }

    private void ShowMainWindow()
    {
        var mainWnd = this.Get<Core.AppViewModel>().Windows.Find(p => p is MainWindow);
        if (!this.Get<Core.AppViewModel>().IsClosed && mainWnd is not null)
        {
            var isMax = PInvoke.IsZoomed(new(Window!.Handle));
            if (!isMax)
            {
                var currentPos = Window!.GetWindow().Position;
                var currentSize = Window.GetWindow().Size;
                if (Window.GetWindow().Presenter.Kind == AppWindowPresenterKind.Overlapped)
                {
                    mainWnd.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(currentPos.X, currentPos.Y, currentSize.Width, currentSize.Height));
                }
            }

            mainWnd.Activate();
        }
    }

    private async void OnWindowDestroying(AppWindow sender, object args)
    {
        var hideMainWindowOnPlay = SettingsToolkit.ReadLocalSetting(SettingNames.HideMainWindowOnPlay, false);
        if (hideMainWindowOnPlay && !UseIntegrationOperation)
        {
            ShowMainWindow();
        }

        await DisposeAsync();

        if (UseIntegrationOperation)
        {
            var mainWnd = this.Get<Core.AppViewModel>().Windows.Find(p => p is MainWindow);
            mainWnd?.Close();
        }
    }

    private async void OnPlayerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Player.Position))
        {
            IsProgressChanging = false;
            PositionText = TimeSpan.FromSeconds(Player.Position).ToString(@"hh\:mm\:ss");
            ProgressChanged?.Invoke(this, Player.Position);
            // Danmaku.UpdatePosition(Convert.ToInt32(Player.Position));

            if (Player.Duration > 5 && Math.Abs(Player.Position - Player.Duration) < 0.1)
            {
                // 因为使用了 keep-open，所以视作播放结束.
                var autoPlayNext = SettingsToolkit.ReadLocalSetting(SettingNames.AutoPlayNext, true);
                if (autoPlayNext && IsNextButtonEnabled)
                {
                    PlayNextCommand.Execute(default);
                }
            }
            else if (Player.Duration < 5)
            {
                return;
            }
        }
        else if (e.PropertyName == nameof(Player.Duration))
        {
            DurationText = TimeSpan.FromSeconds(Player.Duration).ToString(@"hh\:mm\:ss");
        }
        else if (e.PropertyName == nameof(Player.IsFullScreen))
        {
            CheckFullScreen();
        }
        else if (e.PropertyName == nameof(Player.IsCompactOverlay))
        {
            CheckCompactOverlay();
        }
        else if (e.PropertyName == nameof(Player.Title))
        {
            Window!.GetWindow().Title = Player.Title;
        }
        else if (e.PropertyName == nameof(Player.Volume))
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.PlayerVolume, Player.Volume);
        }
        else if (e.PropertyName == nameof(Player.PlaybackRate))
        {
            var isShareSpeed = SettingsToolkit.ReadLocalSetting(SettingNames.IsPlayerSpeedShare, true);
            if (isShareSpeed)
            {
                SettingsToolkit.WriteLocalSetting(SettingNames.PlayerSpeed, Player.PlaybackRate);
            }

            // Danmaku.ExtraSpeed = Player.PlaybackRate;
        }
        else if (e.PropertyName == nameof(Player.IsPlaybackInitialized) && Player.IsPlaybackInitialized && !(Window?.IsClosed ?? true))
        {
            CheckBlackBackgroundVisible();
            if (_continuePosition > 0)
            {
                await Client!.SetCurrentPositionAsync(_continuePosition!.Value);
                _continuePosition = null;
            }

            // Make sure playing.
            if (Player.PlaybackState != MpvPlayerState.Playing)
            {
                await Client!.ResumeAsync();
            }
        }
        else if (e.PropertyName == nameof(Player.CacheSpeed))
        {
            CacheSpeedText = Player.CacheSpeed < 1 ? default : Player.CacheSpeed.Bytes().Humanize() + "/s";
        }
    }
}
