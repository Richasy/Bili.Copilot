// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Forms;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Resolvers;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using Humanizer;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models.Media;
using Richasy.MpvKernel;
using Richasy.MpvKernel.Core;
using Richasy.MpvKernel.Core.Enums;
using Richasy.MpvKernel.Core.Models;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Windows.Media;
using Windows.Storage.Streams;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 播放器视图模型.
/// </summary>
public sealed partial class PlayerViewModel
{
    /// <summary>
    /// 初始化播放器.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    private async Task InitializeInternalAsync()
    {
        if (Connector is null)
        {
            Connector = _snapshot.Type switch
            {
                BiliMediaType.Video => new VideoConnectorViewModel(this),
                BiliMediaType.Pgc => new PgcConnectorViewModel(this),
                _ => default,
            };

            if (Connector != null)
            {
                Connector.PlaylistInitialized += OnConnectorPlaylistInitialized;
                Connector.NewMediaRequest += OnConnectorNewMediaRequest;
                Connector.RequestOpenExtraPanel += OnRequestOpenExtraPanel;
                Connector.PropertiesUpdated += OnConnectorPropertiesUpdated;
            }
        }

        Subtitles.Clear();
        IsSubtitleEmpty = true;

        if (Client?.IsDisposed != false)
        {
            _nativeMp = new Windows.Media.Playback.MediaPlayer();
            _nativeMp.CommandManager.IsEnabled = false; // 禁用原生播放器的命令管理器.
            _smtc = _nativeMp.SystemMediaTransportControls;
            _smtc.IsEnabled = true; // 启用系统媒体控制.
            _smtc.IsPlayEnabled = true;
            _smtc.IsPauseEnabled = true;
            _smtc.ButtonPressed += OnSmtcButtonPressed;
            var gpuNames = AppToolkit.GetGpuNames();
            HasNvidiaGpu = gpuNames.Any(p => p.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase));
            var mpvPath = SettingsToolkit.ReadLocalSetting(SettingNames.CustomLibmpvPath, string.Empty);
            if (string.IsNullOrEmpty(mpvPath) || !File.Exists(mpvPath))
            {
                var architecture = RuntimeInformation.ProcessArchitecture;
                var identifier = architecture == Architecture.Arm64 ? "arm64" : "x64";
                mpvPath = Path.Combine(Package.Current.InstalledPath, "Assets", "libmpv", identifier, "libmpv-2.dll");
            }

            var configPath = string.Empty;
            var decodeType = SettingsToolkit.ReadLocalSetting(SettingNames.PreferDecode, PreferDecodeType.Auto);
            if (decodeType == PreferDecodeType.Custom)
            {
                configPath = await AppToolkit.EnsureMpvConfigExistAsync();
            }

            var options = new MpvInitializeOptions
            {
                UseConfig = false,
                LoadScripts = true,
            };
            Client = await MpvClient.CreateAsync(mpvPath, options, logger: logger);
            Client.ErrorOccurred += OnClientErrorOccurred;
            Client.CacheStateChanged += OnClientCacheStateChanged;
            SetSubtitleDelaySecondsCommand.Execute(SubtitleDelaySeconds);

            if (decodeType == PreferDecodeType.Auto)
            {
                await Client.SetVideoOutputAsync(VideoOutputType.GpuNext);
                await Client.SetGpuApiAsync(GpuApiType.D3D11);
                await Client.SetGpuContextAsync(GpuContextType.Auto);
                await Client.SetHardwareDecodeAsync(HardwareDecodeType.Auto);
            }
            else if (decodeType == PreferDecodeType.D3D11)
            {
                await Client.SetVideoOutputAsync(VideoOutputType.GpuNext);
                await Client.SetGpuApiAsync(GpuApiType.D3D11);
                await Client.SetGpuContextAsync(GpuContextType.D3D11);
                await Client.SetHardwareDecodeAsync(HardwareDecodeType.D3D11va);
            }
            else if (decodeType == PreferDecodeType.D3D12)
            {
                await Client.SetVideoOutputAsync(VideoOutputType.GpuNext);
                await Client.SetGpuApiAsync(GpuApiType.D3D11);
                await Client.SetGpuContextAsync(GpuContextType.D3D11);
                await Client.SetHardwareDecodeAsync(HardwareDecodeType.D3D12vaCopy);
            }
            else if (decodeType == PreferDecodeType.NVDEC)
            {
                await Client.SetVideoOutputAsync(VideoOutputType.GpuNext);
                await Client.SetGpuApiAsync(GpuApiType.D3D11);
                await Client.SetGpuContextAsync(GpuContextType.D3D11);
                await Client.SetHardwareDecodeAsync(HardwareDecodeType.Nvdec);
            }
            else if (decodeType == PreferDecodeType.Vulkan)
            {
                await Client.SetVideoOutputAsync(VideoOutputType.GpuNext);
                await Client.SetGpuApiAsync(GpuApiType.Vulkan);
                await Client.SetGpuContextAsync(GpuContextType.WindowsVulkan);
                await Client.SetHardwareDecodeAsync(HardwareDecodeType.Vulkan);
            }
            else if (decodeType == PreferDecodeType.Software)
            {
                await Client.SetHardwareDecodeAsync(HardwareDecodeType.None);
            }

            await Client.SetMuteAsync(IsMute);

            if (!string.IsNullOrEmpty(configPath))
            {
                var configLines = await File.ReadAllLinesAsync(configPath);
                MaxVolume = 130;

                // 查找以 volume-max 开头的行，获取等号后面的数字值.
                var volumeMaxLine = Array.Find(configLines, line => line.TrimStart().StartsWith("volume-max", StringComparison.OrdinalIgnoreCase));
                if (volumeMaxLine != null)
                {
                    // 提取等号后的数字值（等号后可能跟着空格）.
                    var match = Regex.Match(volumeMaxLine, @"=\s*(\d+(\.\d+)?)");
                    if (match.Success && double.TryParse(match.Groups[1].Value, out var maxVolume))
                    {
                        MaxVolume = maxVolume;
                    }
                }

                await Client.SetConfigFileAsync(configPath);
            }
            else
            {
                // 未使用自定义配置文件时，使用附加设置.
                var preferAudioChannelLayout = SettingsToolkit.ReadLocalSetting(SettingNames.PreferAudioChannelLayout, Richasy.MpvKernel.Core.Enums.AudioChannelLayoutType.Auto);
                if (preferAudioChannelLayout != AudioChannelLayoutType.Auto)
                {
                    if (preferAudioChannelLayout == AudioChannelLayoutType.Custom)
                    {
                        await Client.SetAudioChannelLayoutAsync(AudioChannelLayoutType.Custom, ["7.1", "5.1", "stereo"]);
                    }
                    else
                    {
                        await Client.SetAudioChannelLayoutAsync(preferAudioChannelLayout);
                    }
                }

                var cacheSize = SettingsToolkit.ReadLocalSetting(SettingNames.MaxCacheSize, 300d);
                var cacheSeconds = SettingsToolkit.ReadLocalSetting(SettingNames.MaxCacheSeconds, 360d);
                var backCacheSize = SettingsToolkit.ReadLocalSetting(SettingNames.MaxBackCacheSize, 0d);
                await Client.SetDemuxerMaxBytesAsync($"{Convert.ToInt32(cacheSize)}MiB");
                await Client.SetDemuxerReadheadSecondsAsync(Convert.ToInt32(cacheSeconds));
                if (Convert.ToInt32(backCacheSize) > 0)
                {
                    await Client.SetDemuxerMaxBackBytesAsync($"{Convert.ToInt32(backCacheSize)}MiB");
                }

                var useCacheOnDisk = SettingsToolkit.ReadLocalSetting(SettingNames.CacheOnDisk, false);
                await Client.SetCacheOnDiskAsync(useCacheOnDisk);
                if (useCacheOnDisk)
                {
                    var cacheDir = SettingsToolkit.ReadLocalSetting(SettingNames.CacheDir, string.Empty);
                    if (!string.IsNullOrEmpty(cacheDir) && Directory.Exists(cacheDir))
                    {
                        try
                        {
                            await Client.SetCacheDirAsync(cacheDir);
                        }
                        catch
                        {
                        }
                    }
                }

                var profile = SettingsToolkit.ReadLocalSetting(SettingNames.MpvBuiltInProfile, MpvBuiltInProfile.HighQuality);
                if (profile != MpvBuiltInProfile.HighQuality)
                {
                    await Client.SetLoadAutoProfilesAsync(true);
                    await Client.SetBuiltInProfileAsync(profile.ToString().ToLowerInvariant());
                }

                var hrSeek = SettingsToolkit.ReadLocalSetting(SettingNames.HrSeek, HrSeekType.Default);
                await Client.SetHrSeekAsync(hrSeek);

                MaxVolume = SettingsToolkit.ReadLocalSetting(SettingNames.MaxVolume, 100d);
                await Client.SetMaxVolumeAsync(Convert.ToInt32(MaxVolume));

                var audioExclusive = SettingsToolkit.ReadLocalSetting(SettingNames.AudioExclusiveEnabled, false);
                await Client.SetAudioExclusiveAsync(audioExclusive);
            }

            var logLevel = SettingsToolkit.ReadLocalSetting(SettingNames.MpvLogLevel, MpvLogLevel.Warn);
            await Client.SetLogLevelAsync(logLevel);
            await Client.UseIdleAsync(true);
            await Client.UseKeepOpenAsync(true);
            await Client.SetTlsVerifyAsync(false);
            await Client.SetAutoCreatePlaylistAsync(AutoCreatePlaylistKind.No);
        }

        if (_rightKeyLongPressTimer == null)
        {
            _rightKeyLongPressTimer = queue.CreateTimer();
            _rightKeyLongPressTimer.Interval = TimeSpan.FromMilliseconds(RightKeyLongPressDelay);
            _rightKeyLongPressTimer.Tick += OnRightKeyLongPressTimerTick;
        }

        if (_tripleTimer == null)
        {
            _tripleTimer = queue.CreateTimer();
            _tripleTimer.Interval = TimeSpan.FromSeconds(0.2);
            _tripleTimer.Tick += OnTripleTimerTick;
        }
    }

    private void OnRightKeyLongPressTimerTick(DispatcherQueueTimer sender, object args)
    {
        if (_isRightKeyDown && !_isRightKeyTripleSpeed)
        {
            _isRightKeyTripleSpeed = true;
            TripleSpeedCommand.Execute(default);
        }

        _rightKeyLongPressTimer?.Stop();
    }

    private void OnTripleTimerTick(DispatcherQueueTimer sender, object args)
    {
        // 检查鼠标左键是否在按，同时检查键盘右方向键是否在按.
        var isLeftMouseClick = (PInvoke.GetAsyncKeyState(0x01) & 0x8000) != 0;
        var isRightArrowKeyPressed = (PInvoke.GetAsyncKeyState(0x27) & 0x8000) != 0;
        if (IsHoldingSpeedChanging && !isLeftMouseClick && !isRightArrowKeyPressed && !IsTouching)
        {
            RestoreSpeedCommand.Execute(default);
        }
    }

    private void OnConnectorPropertiesUpdated(object? sender, PlayerInformationUpdatedEventArgs e)
    {
        var updater = _smtc!.DisplayUpdater;
        //updater.AppMediaId = GetSnapshotItemId();
        updater.Type = MediaPlaybackType.Video;
        updater.VideoProperties.Title = e.Title;
        updater.VideoProperties.Subtitle = e.Subtitle;
        if (e.Cover is RandomAccessStreamReference streamRef)
        {
            updater.Thumbnail = streamRef;
        }
        else if (e.Cover is Uri uri)
        {
            updater.Thumbnail = RandomAccessStreamReference.CreateFromUri(uri);
        }
        else
        {
            updater.Thumbnail = null;
        }

        updater.Update();
    }

    private void OnSmtcButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
    {
        queue.TryEnqueue(() =>
        {
            if (args.Button is SystemMediaTransportControlsButton.Play or SystemMediaTransportControlsButton.Pause)
            {
                PlayPauseCommand.Execute(default);
            }
            else if (args.Button == SystemMediaTransportControlsButton.Next && IsNextButtonEnabled)
            {
                PlayNextCommand.Execute(default);
            }
            else if (args.Button == SystemMediaTransportControlsButton.Previous && IsPrevButtonEnabled)
            {
                PlayPreviousCommand.Execute(default);
            }
        });
    }

    private void OnClientCacheStateChanged(object? sender, MpvCacheStateEventArgs e)
        => CacheStateChanged?.Invoke(this, e);

    private async void OnClientErrorOccurred(object? sender, MpvError e)
    {
        if (e == MpvError.TlsError)
        {
            if (!_isTlsFailed)
            {
                _isTlsFailed = true;
                return;
            }

            return;
        }
        else if (Player.Duration < 1 && !_isBroken)
        {
            _isBroken = true;
            await Player.ReplayAsync();
            return;
        }

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

            if (!string.IsNullOrEmpty(errorContent) && (Player.Duration < 1 || e == MpvError.PassthroughFormatUnsupported))
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

    private void CheckBlackBackgroundVisible()
        => IsBlackBackgroundVisible = Player?.IsPlaybackInitialized != true || !string.IsNullOrEmpty(ErrorMessage);

    private void OnWindowSizeChanged(object? sender, Size e)
    {
        queue.TryEnqueue(() =>
        {
            var isVertical = e.Height > e.Width;
            if (isVertical != IsVerticalScreen)
            {
                IsVerticalScreen = isVertical;
                ResetSubtitlePositionCommand.Execute(default);
            }
        });
    }

    private async void OnWindowDestroying(AppWindow sender, object args)
    {
        var appVM = this.Get<AppViewModel>();
        var hideMainWindowOnPlay = SettingsToolkit.ReadLocalSetting(SettingNames.HideMainWindowOnPlay, true);
        if (hideMainWindowOnPlay && !UseIntegrationOperation)
        {
            ShowMainWindow();
        }
        else if (appVM.Players.Count <= 1 && !appVM.IsMainWindowVisible() && !appVM.IsClosed && !(hideMainWindowOnPlay && UseIntegrationOperation))
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
            IsPreviewProgressChanging = false;
            PositionText = TimeSpan.FromSeconds(Player.Position).ToString(@"hh\:mm\:ss");
            ProgressChanged?.Invoke(this, Player.Position);
            Danmaku.UpdatePosition(Convert.ToInt32(Player.Position));
            foreach (var chapter in Chapters)
            {
                chapter.IsPlayed = chapter.Position <= Player.Position;
                chapter.IsPlaying = Math.Abs(chapter.Position - Player.Position) < 2;
            }

            if (Player.Duration > 5)
            {
                IsEnd = false;
                var isShowTip = !SettingsToolkit.ReadLocalSetting(SettingNames.PlayNextWithoutTip, false);
                var endSec = isShowTip ? 2 : 0.1;
                if (Math.Abs(Player.Position - Player.Duration) < endSec)
                {
                    IsHoldingSpeedChanging = false;
                    if (Math.Abs(_lastSpeed - Player.PlaybackRate) > 0.01)
                    {
                        await Client!.SetSpeedAsync(_lastSpeed);
                    }

                    if (_isTlsFailed)
                    {
                        // 如果 TLS 失败了，可能会导致播放器无法正确获取位置，所以这里不处理跳过逻辑.
                        ErrorMessage = ResourceToolkit.GetLocalizedString(StringNames.SeekFailed);
                        CheckBlackBackgroundVisible();
                        return;
                    }

                    // 因为使用了 keep-open，所以视作播放结束.
                    var autoPlayNext = SettingsToolkit.ReadLocalSetting(SettingNames.AutoPlayNext, true);
                    if (autoPlayNext && IsNextButtonEnabled && !IsConnecting)
                    {
                        if (isShowTip)
                        {
                            if (!IsNextTipShown && !IsNextTipShownInThisMedia)
                            {
                                IsNextTipShownInThisMedia = true;
                                RequestShowNextTip?.Invoke(this, EventArgs.Empty);
                            }
                        }
                        else
                        {
                            PlayNextCommand.Execute(default);
                            _isTlsFailed = false;
                            return;
                        }
                    }
                    else
                    {
                        IsEnd = true;
                        IsControlsVisible = true;
                    }
                }
                else if (Math.Abs(Player.Position - Player.Duration) > 2 && IsNextTipShown)
                {
                    RequestHideNextTip?.Invoke(this, EventArgs.Empty);
                }

                if (Player.PlaybackState == MpvPlayerState.Playing && _isTlsFailed)
                {
                    _isTlsFailed = false;
                }

                _prevPosition = Player.Position;
            }
            else if (Player.Duration < 5)
            {
                return;
            }
        }
        else if (e.PropertyName == nameof(Player.Duration))
        {
            DurationText = TimeSpan.FromSeconds(Player.Duration).ToString(@"hh\:mm\:ss");
            ChapterInitialized?.Invoke(this, EventArgs.Empty);
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
        else if (e.PropertyName == nameof(Player.Volume) && Player.IsPlaybackInitialized)
        {
            if (IsVolumeChanging)
            {
                if (DateTimeOffset.Now - LastVolumeChangingTime < TimeSpan.FromMilliseconds(500))
                {
                    return;
                } 

                IsVolumeChanging = false;
            }

            CurrentVolume = Player.Volume;
            SettingsToolkit.WriteLocalSetting(SettingNames.PlayerVolume, Player.Volume);
        }
        else if (e.PropertyName == nameof(Player.PlaybackRate) && Player.IsPlaybackInitialized)
        {
            Danmaku.ExtraSpeed = Player.PlaybackRate;
            if (SettingsToolkit.ReadLocalSetting(SettingNames.IsPlayerSpeedShare, true))
            {
                SettingsToolkit.WriteLocalSetting(SettingNames.PlayerSpeed, Player.PlaybackRate);
            }
        }
        else if (e.PropertyName == nameof(Player.IsPlaybackInitialized) && Player.IsPlaybackInitialized && !(Window?.IsClosed ?? true))
        {
            ErrorMessage = default;
            _prevPosition = 0;
            CheckBlackBackgroundVisible();
            CheckBottomProgressBarVisible();
            ResetSubtitlePositionCommand.Execute(default);
            ReloadFontsCommand.Execute(default);
            var subtitleFontSize = SettingsToolkit.ReadLocalSetting(SettingNames.SubtitleFontSize, 32d);
            ChangeSubtitleFontSizeCommand.Execute(Math.Max(1, subtitleFontSize));
            if (_continuePosition > 0)
            {
                await Client!.SetCurrentPositionAsync(_continuePosition!.Value);
                _continuePosition = null;
            }

            // 强制关闭可能开启的统计信息覆盖层.
            await Client!.SendKeyPressAsync("ESC");
            IsStatsOverlayShown = false;

            // Make sure playing.
            if (Player.PlaybackState != MpvPlayerState.Playing)
            {
                await Client!.ResumeAsync();
            }

            await VerifyChaptersAsync();

            if (!_isDanmakuInitialized)
            {
                InitializeDanmakuCommand.Execute(default);
                _isDanmakuInitialized = true;
            }
        }
        else if (e.PropertyName == nameof(Player.CacheSpeed))
        {
            CacheSpeedText = Player.CacheSpeed < 1 ? default : Player.CacheSpeed.Bytes().Humanize() + "/s";
        }
        else if (e.PropertyName == nameof(Player.PlaybackState))
        {
            if (IsDanmakuControlVisible && Danmaku.IsShowDanmaku)
            {
                if (Player.PlaybackState == MpvPlayerState.Playing)
                {
                    Danmaku.Resume();
                }
                else
                {
                    Danmaku.Pause();
                }
            }

            if (Player.PlaybackState is MpvPlayerState.Buffering or MpvPlayerState.Seeking && !string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = default;
                CheckBlackBackgroundVisible();
            }

            _smtc?.PlaybackStatus = Player.PlaybackState switch
            {
                MpvPlayerState.Playing => MediaPlaybackStatus.Playing,
                MpvPlayerState.Paused => MediaPlaybackStatus.Paused,
                MpvPlayerState.Buffering or MpvPlayerState.Buffering => MediaPlaybackStatus.Changing,
                _ => MediaPlaybackStatus.Stopped,
            };
        }
    }

    private async void OnConnectorNewMediaRequest(object? sender, MediaSnapshot e)
    {
        if (_snapshot == e)
        {
            return;
        }

        await Client!.PauseAsync();
        await Client!.StopAsync();
        await InitializeAsync(e);
    }

    private void OnConnectorPlaylistInitialized(object? sender, PlaylistInitializedEventArgs e)
    {
        IsPrevButtonEnabled = e.CanPrev;
        IsNextButtonEnabled = e.CanNext;
        PrevButtonToolTip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.PrevVideoTemplate), e.PrevTitle);
        NextButtonToolTip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.NextVideoTemplate), e.NextTitle);
        IsVideoNavigationAvailable = e.CanPrev || e.CanNext;
        if (_smtc != null)
        {
            _smtc.IsNextEnabled = IsNextButtonEnabled;
            _smtc.IsPreviousEnabled = IsPrevButtonEnabled;
        }
    }

    private void OnRequestOpenExtraPanel(object? sender, EventArgs e)
    {
        if (IsControlsVisible)
        {
            IsControlsVisible = false;
        }

        IsExtraPanelVisible = true;
    }

    private void OnWindowPointerReleased(InputPointerSource sender, PointerEventArgs args)
    {
        if ((args.CurrentPoint.Properties.PointerUpdateKind is PointerUpdateKind.XButton1Released or PointerUpdateKind.XButton2Released) && UseIntegrationOperation)
        {
            BackCommand.Execute(default);
            args.Handled = true;
        }
    }

    private void OnWindowKeyDown(InputKeyboardSource sender, KeyEventArgs args)
    {
        if (IsControlsVisible || IsPopupVisible || IsExtraPanelVisible)
        {
            return;
        }

        if (Player is null || Player.PlaybackState == MpvPlayerState.Idle || Player.PlaybackState == MpvPlayerState.End)
        {
            return;
        }

        if (args.VirtualKey == Windows.System.VirtualKey.Right && !_isRightKeyDown && AppToolkit.NotModifierKeyPressed())
        {
            _isRightKeyDown = true;
            _isRightKeyTripleSpeed = false;
            _rightKeyLongPressTimer?.Start();
        }
    }

    private async void OnWindowKeyUp(InputKeyboardSource sender, KeyEventArgs args)
    {
        if (IsPopupVisible || IsExtraPanelVisible)
        {
            return;
        }

        if (args.VirtualKey == Windows.System.VirtualKey.Escape)
        {
            BackToDefaultModeCommand.Execute(default);
        }

        if (Player is null || Player.PlaybackState == MpvPlayerState.Idle || Player.PlaybackState == MpvPlayerState.End)
        {
            return;
        }

        if (IsStatsOverlayShown)
        {
            if (args.VirtualKey is Windows.System.VirtualKey.Up or
                Windows.System.VirtualKey.Down or
                Windows.System.VirtualKey.Number1 or
                Windows.System.VirtualKey.Number2 or
                Windows.System.VirtualKey.Number3 or
                Windows.System.VirtualKey.Number4 or
                Windows.System.VirtualKey.Number5)
            {
                await Client!.SendKeyPressAsync(args.VirtualKey.ToString().Replace("Number", string.Empty, StringComparison.Ordinal).ToLowerInvariant());
                return;
            }
        }

        if (args.VirtualKey == Windows.System.VirtualKey.Space)
        {
            PlayPauseCommand.Execute(default);
        }
        else if (args.VirtualKey == Windows.System.VirtualKey.Right)
        {
            _rightKeyLongPressTimer?.Stop();
            if (_isRightKeyTripleSpeed)
            {
                _isRightKeyTripleSpeed = false;
                await Client!.SetSpeedAsync(_lastSpeed);
                LastSpeedChangingTime = DateTimeOffset.Now;
                IsSpeedChanging = true;
            }
            else
            {
                if (AppToolkit.IsOnlyCtrlPressed())
                {
                    IncreaseSpeedCommand.Execute(default);
                }
                else if (AppToolkit.IsOnlyShiftPressed())
                {
                    if (IsNextButtonEnabled)
                    {
                        PlayNextCommand.Execute(default);
                    }
                }
                else if (!IsControlsVisible && AppToolkit.NotModifierKeyPressed())
                {
                    SkipForwardCommand.Execute(default);
                }
            }

            _isRightKeyDown = false;
        }
        else if (args.VirtualKey == Windows.System.VirtualKey.Left)
        {
            if (AppToolkit.IsOnlyCtrlPressed())
            {
                DecreaseSpeedCommand.Execute(default);
            }
            else if (AppToolkit.IsOnlyShiftPressed())
            {
                if (IsPrevButtonEnabled)
                {
                    PlayPreviousCommand.Execute(default);
                }
            }
            else if (!IsControlsVisible && AppToolkit.NotModifierKeyPressed())
            {
                SkipBackwardCommand.Execute(default);
            }
        }
        else if (args.VirtualKey == Windows.System.VirtualKey.Up)
        {
            if (AppToolkit.IsOnlyCtrlPressed())
            {
                IncreaseSpeedCommand.Execute(default);
            }
            else if (!IsControlsVisible && AppToolkit.NotModifierKeyPressed())
            {
                IncreaseVolumeCommand.Execute(default);
            }
        }
        else if (args.VirtualKey == Windows.System.VirtualKey.Down)
        {
            if (AppToolkit.IsOnlyCtrlPressed())
            {
                DecreaseSpeedCommand.Execute(default);
            }
            else if (!IsControlsVisible && AppToolkit.NotModifierKeyPressed())
            {
                DecreaseVolumeCommand.Execute(default);
            }
        }
        else if (args.VirtualKey == Windows.System.VirtualKey.F11)
        {
            ToggleFullScreenCommand.Execute(default);
        }
        else if (args.VirtualKey == Windows.System.VirtualKey.M && AppToolkit.IsOnlyCtrlPressed())
        {
            ToggleCompactOverlayCommand.Execute(default);
        }
        else if (args.VirtualKey == Windows.System.VirtualKey.PageUp)
        {
            SwitchToPreviousChapterCommand.Execute(default);
        }
        else if (args.VirtualKey == Windows.System.VirtualKey.PageDown)
        {
            SwitchToNextChapterCommand.Execute(default);
        }
        else if (args.VirtualKey == Windows.System.VirtualKey.I)
        {
            ToggleStatsOverlayCommand.Execute(default);
        }
    }

    private async Task VerifyChaptersAsync()
    {
        if (Client is null || Player is null)
        {
            return;
        }

        var chapters = await Client.GetChaptersAsync();
        if (chapters.IsSuccess && Chapters.Count == 0 && chapters.Value.Count > 1)
        {
            var index = 0;
            foreach (var chapter in chapters.Value)
            {
                Chapters.Add(new ChapterItemViewModel(index, chapter.Title, chapter.Time));
                index++;
            }

            ChapterInitialized?.Invoke(this, EventArgs.Empty);
        }
    }

    private async Task LoadSourcesAsync()
    {
        try
        {
            if (_sourceResolver is VideoMediaSourceResolver videoResolver)
            {
                var sources = await videoResolver.GetSourcesAsync();
                IsSourceSelectable = sources?.Count > 1;
                Sources.Clear();
                if (sources != null)
                {
                    var currentSource = videoResolver.GetCurrentFormat();
                    foreach (var source in sources)
                    {
                        var vm = new SourceItemViewModel(source, SelectSourceAsync);
                        if (currentSource != null)
                        {
                            vm.IsSelected = source.Quality == currentSource.Quality;
                        }

                        Sources.Add(vm);
                    }

                    SelectedSource = Sources.FirstOrDefault(p => p.IsSelected);
                }

                var c = ((VideoConnectorViewModel)Connector);
                DanmakuSend.ResetData(c._view.Information.Identifier.Id, c._part.Identifier.Id);
                c.InitializeDownloader(sources);
            }
            else if (_sourceResolver is PgcMediaSourceResolver pgcResolver)
            {
                var sources = await pgcResolver.GetSourcesAsync();
                IsSourceSelectable = sources?.Count > 1;
                Sources.Clear();
                if (sources != null)
                {
                    var currentSource = pgcResolver.GetCurrentFormat();
                    foreach (var source in sources)
                    {
                        var vm = new SourceItemViewModel(source, SelectSourceAsync);
                        if (currentSource != null)
                        {
                            vm.IsSelected = source.Quality == currentSource.Quality;
                        }

                        Sources.Add(vm);
                    }

                    SelectedSource = Sources.FirstOrDefault(p => p.IsSelected);
                }

                var c = ((PgcConnectorViewModel)Connector);
                var aid = c._episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Aid).ToString();
                var cid = c._episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Cid).ToString();
                DanmakuSend.ResetData(aid.ToString(), cid.ToString());
                c.InitializeDownloader(sources);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "加载清晰度列表失败");
        }
    }

    private async Task LoadSubtitlesAsync()
    {
        try
        {
            if (!IsSubtitleEnabled)
            {
                return;
            }

            var service = this.Get<ISubtitleService>();
            if (Connector is VideoConnectorViewModel videoVM)
            {
                var aid = videoVM._view.Information.Identifier.Id;
                var cid = videoVM._part.Identifier.Id;
                var metas = await service.GetSubtitleMetasAsync(aid, cid);
                Subtitles.Clear();
                if (metas?.Count > 0)
                {
                    foreach (var item in metas)
                    {
                        var vm = new SubtitleItemViewModel(item, SelectSubtitleAsync, DeselectSubtitleAsync);
                        Subtitles.Add(vm);
                    }
                }

                if (Subtitles.Count > 0 && Subtitles.Any(p => !p.IsAI))
                {
                    var firstNonAi = Subtitles.First(p => !p.IsAI);
                    firstNonAi.SelectCommand.Execute(default);
                }

                IsSubtitleEmpty = Subtitles.Count == 0;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "加载字幕列表失败");
        }
    }

    private async Task SelectSubtitleAsync(SubtitleItemViewModel vm)
    {
        foreach (var item in Subtitles)
        {
            item.IsSelected = item.Data.Equals(vm.Data);
        }

        if (string.IsNullOrEmpty(vm._srtFilePath))
        {
            return;
        }

        await Client!.SetExternalSubtitleTrackAsync(vm._srtFilePath);
    }

    private async Task DeselectSubtitleAsync(SubtitleItemViewModel vm)
    {
        foreach (var item in Subtitles)
        {
            item.IsSelected = false;
        }

        await Client!.SetSubtitleTrackAsync(default);
    }

    private static string[] FindAnime4KShaders(Anime4KMode mode)
    {
        if (mode == Anime4KMode.None)
        {
            return [];
        }

        var assetsFolder = Path.Combine(Package.Current.InstalledPath, "Assets", "Anime4K");
        if (mode == Anime4KMode.LowA)
        {
            return [GetShaderPath("Anime4K_Clamp_Highlights.glsl"), GetShaderPath("Anime4K_Restore_CNN_M.glsl"), GetShaderPath("Anime4K_Upscale_CNN_x2_M.glsl"), GetShaderPath("Anime4K_AutoDownscalePre_x2.glsl"), GetShaderPath("Anime4K_AutoDownscalePre_x4.glsl"), GetShaderPath("Anime4K_Upscale_CNN_x2_S.glsl")];
        }
        else if (mode == Anime4KMode.LowB)
        {
            return [GetShaderPath("Anime4K_Clamp_Highlights.glsl"), GetShaderPath("Anime4K_Restore_CNN_Soft_M.glsl"), GetShaderPath("Anime4K_Upscale_CNN_x2_M.glsl"), GetShaderPath("Anime4K_AutoDownscalePre_x2.glsl"), GetShaderPath("Anime4K_AutoDownscalePre_x4.glsl"), GetShaderPath("Anime4K_Upscale_CNN_x2_S.glsl")];
        }
        else if (mode == Anime4KMode.LowC)
        {
            return [GetShaderPath("Anime4K_Clamp_Highlights.glsl"), GetShaderPath("Anime4K_Upscale_Denoise_CNN_x2_M.glsl"), GetShaderPath("Anime4K_AutoDownscalePre_x2.glsl"), GetShaderPath("Anime4K_AutoDownscalePre_x4.glsl"), GetShaderPath("Anime4K_Upscale_CNN_x2_S.glsl")];
        }
        else if (mode == Anime4KMode.LowAA)
        {
            return [GetShaderPath("Anime4K_Clamp_Highlights.glsl"), GetShaderPath("Anime4K_Restore_CNN_M.glsl"), GetShaderPath("Anime4K_Upscale_CNN_x2_M.glsl"), GetShaderPath("Anime4K_Restore_CNN_S.glsl"), GetShaderPath("Anime4K_AutoDownscalePre_x2.glsl"), GetShaderPath("Anime4K_AutoDownscalePre_x4.glsl"), GetShaderPath("Anime4K_Upscale_CNN_x2_S.glsl")];
        }
        else if (mode == Anime4KMode.LowBB)
        {
            return [GetShaderPath("Anime4K_Clamp_Highlights.glsl"), GetShaderPath("Anime4K_Restore_CNN_Soft_M.glsl"), GetShaderPath("Anime4K_Upscale_CNN_x2_M.glsl"), GetShaderPath("Anime4K_AutoDownscalePre_x2.glsl"), GetShaderPath("Anime4K_AutoDownscalePre_x4.glsl"), GetShaderPath("Anime4K_Restore_CNN_Soft_S.glsl"), GetShaderPath("Anime4K_Upscale_CNN_x2_S.glsl")];
        }
        else if (mode == Anime4KMode.LowCA)
        {
            return [GetShaderPath("Anime4K_Clamp_Highlights.glsl"), GetShaderPath("Anime4K_Upscale_Denoise_CNN_x2_M.glsl"), GetShaderPath("Anime4K_AutoDownscalePre_x2.glsl"), GetShaderPath("Anime4K_AutoDownscalePre_x4.glsl"), GetShaderPath("Anime4K_Restore_CNN_S.glsl"), GetShaderPath("Anime4K_Upscale_CNN_x2_S.glsl")];
        }
        else if (mode == Anime4KMode.HighA)
        {
            return [GetShaderPath("Anime4K_Clamp_Highlights.glsl"), GetShaderPath("Anime4K_Restore_CNN_VL.glsl"), GetShaderPath("Anime4K_Upscale_CNN_x2_VL.glsl"), GetShaderPath("Anime4K_AutoDownscalePre_x2.glsl"), GetShaderPath("Anime4K_AutoDownscalePre_x4.glsl"), GetShaderPath("Anime4K_Upscale_CNN_x2_M.glsl")];
        }
        else if (mode == Anime4KMode.HighB)
        {
            return [GetShaderPath("Anime4K_Clamp_Highlights.glsl"), GetShaderPath("Anime4K_Restore_CNN_Soft_VL.glsl"), GetShaderPath("Anime4K_Upscale_CNN_x2_VL.glsl"), GetShaderPath("Anime4K_AutoDownscalePre_x2.glsl"), GetShaderPath("Anime4K_AutoDownscalePre_x4.glsl"), GetShaderPath("Anime4K_Upscale_CNN_x2_M.glsl")];
        }
        else if (mode == Anime4KMode.HighC)
        {
            return [GetShaderPath("Anime4K_Clamp_Highlights.glsl"), GetShaderPath("Anime4K_Upscale_Denoise_CNN_x2_VL.glsl"), GetShaderPath("Anime4K_AutoDownscalePre_x2.glsl"), GetShaderPath("Anime4K_AutoDownscalePre_x4.glsl"), GetShaderPath("Anime4K_Upscale_CNN_x2_M.glsl")];
        }
        else if (mode == Anime4KMode.HighAA)
        {
            return [GetShaderPath("Anime4K_Clamp_Highlights.glsl"), GetShaderPath("Anime4K_Restore_CNN_VL.glsl"), GetShaderPath("Anime4K_Upscale_CNN_x2_VL.glsl"), GetShaderPath("Anime4K_Restore_CNN_M.glsl"), GetShaderPath("Anime4K_AutoDownscalePre_x2.glsl"), GetShaderPath("Anime4K_AutoDownscalePre_x4.glsl"), GetShaderPath("Anime4K_Upscale_CNN_x2_M.glsl")];
        }
        else if (mode == Anime4KMode.HighBB)
        {
            return [GetShaderPath("Anime4K_Clamp_Highlights.glsl"), GetShaderPath("Anime4K_Restore_CNN_Soft_VL.glsl"), GetShaderPath("Anime4K_Upscale_CNN_x2_VL.glsl"), GetShaderPath("Anime4K_AutoDownscalePre_x2.glsl"), GetShaderPath("Anime4K_AutoDownscalePre_x4.glsl"), GetShaderPath("Anime4K_Restore_CNN_Soft_M.glsl"), GetShaderPath("Anime4K_Upscale_CNN_x2_M.glsl")];
        }
        else if (mode == Anime4KMode.HighCA)
        {
            return [GetShaderPath("Anime4K_Clamp_Highlights.glsl"), GetShaderPath("Anime4K_Upscale_Denoise_CNN_x2_VL.glsl"), GetShaderPath("Anime4K_AutoDownscalePre_x2.glsl"), GetShaderPath("Anime4K_AutoDownscalePre_x4.glsl"), GetShaderPath("Anime4K_Restore_CNN_M.glsl"), GetShaderPath("Anime4K_Upscale_CNN_x2_M.glsl")];
        }

        return [];

        string GetShaderPath(string shaderName) => Path.Combine(assetsFolder, shaderName);
    }

    private static string[] FindArtCNNShaders(ArtCNNMode mode)
    {
        if (mode == ArtCNNMode.None)
        {
            return [];
        }

        var assetsFolder = Path.Combine(Package.Current.InstalledPath, "Assets", "ArtCNN");
        if (mode == ArtCNNMode.C4F16)
        {
            return [GetShaderPath("ArtCNN_C4F16.glsl")];
        }
        else if (mode == ArtCNNMode.C4F16DN)
        {
            return [GetShaderPath("ArtCNN_C4F16_DN.glsl")];
        }
        else if (mode == ArtCNNMode.C4F16DS)
        {
            return [GetShaderPath("ArtCNN_C4F16_DS.glsl")];
        }
        else if (mode == ArtCNNMode.C4F32)
        {
            return [GetShaderPath("ArtCNN_C4F32.glsl")];
        }
        else if (mode == ArtCNNMode.C4F32DN)
        {
            return [GetShaderPath("ArtCNN_C4F32_DN.glsl")];
        }
        else if (mode == ArtCNNMode.C4F32DS)
        {
            return [GetShaderPath("ArtCNN_C4F32_DS.glsl")];
        }
        else if (mode == ArtCNNMode.C4F32Ani4KI2)
        {
            return [GetShaderPath("Ani4Kv2_ArtCNN_C4F32_i2.glsl")];
        }
        else if (mode == ArtCNNMode.C4F32AniSDI4)
        {
            return [GetShaderPath("AniSD_ArtCNN_C4F32_i4.glsl")];
        }

        return [];
        string GetShaderPath(string shaderName) => Path.Combine(assetsFolder, shaderName);
    }

    private static string[] FindNnedi3Shaders(Nnedi3Mode mode)
    {
        if (mode == Nnedi3Mode.None)
        {
            return [];
        }

        var assetsFolder = Path.Combine(Package.Current.InstalledPath, "Assets", "nnedi3");
        var igvFolder = Path.Combine(Package.Current.InstalledPath, "Assets", "igv");

        if (mode == Nnedi3Mode.Basic)
        {
            return [GetShaderPath("nnedi3-nns32-win8x4.glsl"), GetIgvShaderPath("adaptive-sharpen_luma.glsl")];
        }
        else if (mode == Nnedi3Mode.Plus)
        {
            return [GetShaderPath("nnedi3-nns64-win8x4.glsl"), GetIgvShaderPath("adaptive-sharpen_luma.glsl")];
        }
        else if (mode == Nnedi3Mode.Pro)
        {
            return [GetShaderPath("nnedi3-nns128-win8x4.glsl"), GetIgvShaderPath("adaptive-sharpen_luma.glsl")];
        }
        else if (mode == Nnedi3Mode.ProMax)
        {
            return [GetShaderPath("nnedi3-nns256-win8x4.glsl"), GetIgvShaderPath("adaptive-sharpen_luma.glsl")];
        }

        return [];

        string GetShaderPath(string shaderName) => Path.Combine(assetsFolder, shaderName);
        string GetIgvShaderPath(string shaderName) => Path.Combine(igvFolder, shaderName);
    }

    private static string[] FindRavuShaders(RavuMode mode)
    {
        if (mode == RavuMode.None)
        {
            return [];
        }

        var assetsFolder = Path.Combine(Package.Current.InstalledPath, "Assets", "Ravu");

        if (mode == RavuMode.LiteArR2)
        {
            return [GetShaderPath("ravu-lite-ar-r2.hook")];
        }
        else if (mode == RavuMode.LiteArR3)
        {
            return [GetShaderPath("ravu-lite-ar-r2.hook")];
        }
        else if (mode == RavuMode.LiteArR4)
        {
            return [GetShaderPath("ravu-lite-ar-r4.hook")];
        }
        else if (mode == RavuMode.LiteR2)
        {
            return [GetShaderPath("ravu-lite-r2.hook")];
        }
        else if (mode == RavuMode.LiteR3)
        {
            return [GetShaderPath("ravu-lite-r3.hook")];
        }
        else if (mode == RavuMode.LiteR4)
        {
            return [GetShaderPath("ravu-lite-r4.hook")];
        }

        return [];

        string GetShaderPath(string shaderName) => Path.Combine(assetsFolder, shaderName);
    }

    private string[] GetCombinedShaders()
    {
        var shaders = new List<string>();
        shaders.AddRange(FindAnime4KShaders(Anime4KMode));
        shaders.AddRange(FindArtCNNShaders(ArtCNNMode));
        shaders.AddRange(FindRavuShaders(RavuMode));
        shaders.AddRange(FindNnedi3Shaders(Nnedi3Mode));
        return [.. shaders.Distinct()];
    }

    private void CheckClearShaderEnabled()
    {
        IsClearShaderEnabled = Anime4KMode != Anime4KMode.None ||
                                 ArtCNNMode != ArtCNNMode.None ||
                                 RavuMode != RavuMode.None ||
                                 Nnedi3Mode != Nnedi3Mode.None;
    }
}
