// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls.Core.Common;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Microsoft.Extensions.Logging;
using Mpv.Core;
using Mpv.Core.Args;
using Mpv.Core.Enums.Client;
using Richasy.BiliKernel.Bili.Authorization;
using WinRT;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// MPV 播放器视图模型.
/// </summary>
[GeneratedBindableCustomProperty]
public sealed partial class MpvPlayerViewModel : PlayerViewModelBase
{
    /// <summary>
    /// 播放器内核.
    /// </summary>
    public Player? Player { get; private set; }

    /// <summary>
    /// 初始化播放器.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task InitializeAsync(RenderControl renderControl)
    {
        Player ??= new Mpv.Core.Player();
        if (!Player.Client.IsInitialized)
        {
            IsPlayerInitializing = true;
            Player.PlaybackPositionChanged += OnPositionChanged;
            Player.PlaybackStateChanged += OnStateChanged;
            Player.PlaybackStopped += OnPlaybackStopped;
            Player.LogMessageReceived += OnLogMessageReceivedAsync;
            renderControl.Initialize();
            Player.Client.SetOption("vo", "libmpv");
#if DEBUG
            Player.Client.RequestLogMessage(MpvLogLevel.V);
#else
            Player.Client.RequestLogMessage(MpvLogLevel.Error);
#endif
            var args = new InitializeArgument(default, func: RenderContext.GetProcAddress);
            await Player.InitializeAsync(args);

            var isGpuChecked = SettingsToolkit.ReadLocalSetting(SettingNames.IsGpuChecked, false);
            if (!isGpuChecked)
            {
                this.Get<AppViewModel>().CheckGpuIsAmdCommand.Execute(default);
            }
        }

        if (!IsWebDav)
        {
            var cookies = this.Get<IBiliCookiesResolver>().GetCookieString();
            var referer = IsLive ? LiveReferer : VideoReferer;
            var userAgent = IsLive ? LiveUserAgent : VideoUserAgent;
            var cookieStr = $"Cookie: {cookies}";
            var refererStr = $"Referer: {referer}";

            Player.Client.SetOption("cookies", "yes");
            Player.Client.SetOption("user-agent", userAgent);
            Player.Client.SetOption("http-header-fields", $"{cookieStr}\n{refererStr}");
        }

        if (IsLive || IsWebDav)
        {
            Player.Client.SetOption("ytdl", "no");
            Player.IsLoggingEnabled = false;
        }

        IsPlayerInitializing = false;
        _isInitialized = true;
        RaiseInitializedEvent();
        await TryLoadPlayDataAsync();
    }

    /// <inheritdoc/>
    protected override void SetWebDavConfig(WebDavConfig config)
        => LoadWebDavAuthorization();

    private void OnPositionChanged(object? sender, PlaybackPositionChangedEventArgs e)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (e.Duration == 0 && !IsLive)
            {
                Player.ResetDuration();
            }

            if (IsPaused && !IsBuffering)
            {
                IsPaused = false;
            }

            UpdatePosition(Convert.ToInt32(e.Position), Convert.ToInt32(e.Duration));
        });
    }

    private void OnStateChanged(object? sender, PlaybackStateChangedEventArgs e)
    {
        if (e.NewState == Mpv.Core.Enums.Player.PlaybackState.Decoding)
        {
            OnSetVolume(SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.PlayerVolume, 100));
        }

        UpdateState((PlayerState)(int)e.NewState);
    }

    private void OnPlaybackStopped(object? sender, PlaybackStoppedEventArgs e)
    {
        if (Math.Abs(Position - Duration) < 5)
        {
            ReachEnd();
        }
        else
        {
            UpdateState(PlayerState.Paused);
        }
    }

    private async void OnLogMessageReceivedAsync(object? sender, LogMessageReceivedEventArgs e)
    {
#if DEBUG
        System.Diagnostics.Debug.WriteLine($"core: [{e.Prefix}] {e.Message}");
#else
        _logger.LogError($"core: {e.Message}");
#endif
        if (e.Message.Contains("mpv_render_context_render() not being called or stuck"))
        {
            await Player.TerminateAsync();

            // 渲染崩溃，需要立刻终止.
            _dispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.High, () =>
            {
                ReloadCommand.Execute(default);
            });
        }
    }

    private void InitializeDecode()
    {
        var decodeType = SettingsToolkit.ReadLocalSetting(SettingNames.PreferDecode, PreferDecodeType.Auto);
        switch (decodeType)
        {
            case PreferDecodeType.Auto:
                Player.Client.SetProperty("hwdec", "no");
                Player.Client.SetProperty("gpu-context", "auto");
                Player.Client.SetProperty("gpu-api", "auto");
                break;
            case PreferDecodeType.D3D11:
                Player.Client.SetProperty("hwdec", "d3d11va");
                Player.Client.SetProperty("gpu-context", "d3d11");
                Player.Client.SetProperty("gpu-api", "d3d11");
                break;
            case PreferDecodeType.NVDEC:
                Player.Client.SetProperty("hwdec", "nvdec");
                Player.Client.SetProperty("gpu-context", "auto");
                Player.Client.SetProperty("gpu-api", "auto");
                break;
            case PreferDecodeType.DXVA2:
                Player.Client.SetProperty("hwdec", "dxva2");
                Player.Client.SetProperty("gpu-context", "dxinterop");
                Player.Client.SetProperty("gpu-api", "auto");
                break;
            default:
                break;
        }
    }

    private async Task WaitUntilAddAudioAsync(string audioUrl)
    {
        const int maxRetryCount = 3;
        var retryCount = 0;
        var isAudioAdded = false;
        do
        {
            if (retryCount >= maxRetryCount)
            {
                break;
            }

            try
            {
                if (Player.IsDisposed)
                {
                    return;
                }

                await Player.Client.ExecuteAsync(["audio-add", audioUrl]);
                isAudioAdded = true;
            }
            catch (Exception)
            {
                retryCount++;
                await Task.Delay(300);
            }
        }
        while (!isAudioAdded);

        if (!isAudioAdded)
        {
            UpdateState(PlayerState.Failed);
            _logger.LogError("尝试播放音频失败.");
        }
    }

    private void LoadWebDavAuthorization()
    {
        if (_webDavConfig is null || Player?.Client is null)
        {
            return;
        }

        var auth = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_webDavConfig.UserName}:{_webDavConfig.Password}"))}";
        Player.Client.SetOption("http-header-fields", $"Authorization: {auth}");
    }
}
