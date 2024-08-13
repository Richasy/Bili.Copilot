﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls.Mpv.Common;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Mpv.Core.Args;
using Mpv.Core.Enums.Client;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 播放器视图模型.
/// </summary>
public sealed partial class PlayerViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerViewModel"/> class.
    /// </summary>
    public PlayerViewModel(
        ILogger<PlayerViewModel> logger,
        DispatcherQueue dispatcherQueue)
    {
        _logger = logger;
        _dispatcherQueue = dispatcherQueue;
    }

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
            Player.LogMessageReceived += OnLogMessageReceived;
            renderControl.Initialize();
            Player.Client.SetProperty("vo", "libmpv");
            Player.Client.RequestLogMessage(MpvLogLevel.V);
            var args = new InitializeArgument(default, func: RenderContext.GetProcAddress);
            await Player.InitializeAsync(args);
        }

        var cookies = this.Get<IBiliCookiesResolver>().GetCookieString();
        var referer = IsLive ? LiveReferer : VideoReferer;
        var userAgent = IsLive ? LiveUserAgent : VideoUserAgent;
        var cookieStr = $"Cookie: {cookies}";
        var refererStr = $"Referer: {referer}";
        Player.Client.SetOption("cookies", "yes");
        Player.Client.SetOption("user-agent", userAgent);
        Player.Client.SetOption("http-header-fields", $"{cookieStr}\n{refererStr}");
        if (IsLive)
        {
            Player.Client.SetOption("ytdl", "no");
        }
        else
        {
            Player.Client.SetOption("ytdl", "yes");
        }

        IsPlayerInitializing = false;
        _isInitialized = true;

        await TryLoadPlayDataAsync();
    }

    /// <summary>
    /// 设置播放数据.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task SetPlayDataAsync(string? videoUrl, string? audioUrl, bool isAutoPlay)
    {
        _videoUrl = videoUrl;
        _audioUrl = audioUrl;
        _autoPlay = isAutoPlay;

        if (_isInitialized)
        {
            await TryLoadPlayDataAsync();
        }
    }

    /// <summary>
    /// 关闭播放器.
    /// </summary>
    public void Close()
        => Player?.Dispose();

    private async Task TryLoadPlayDataAsync()
    {
        if (string.IsNullOrEmpty(_videoUrl) && string.IsNullOrEmpty(_audioUrl))
        {
            return;
        }

        if (!string.IsNullOrEmpty(_videoUrl))
        {
            await Player.Client.ExecuteAsync(["loadfile", _videoUrl, "replace"]);

            if (!string.IsNullOrEmpty(_audioUrl))
            {
                await Player.Client.ExecuteAsync(["audio-add", _audioUrl]);
            }
        }
        else if (!string.IsNullOrEmpty(_audioUrl))
        {
            await Player.Client.ExecuteAsync(["loadfile", _audioUrl, "replace"]);
        }

        if (_autoPlay)
        {
            Player.Play();
        }
        else
        {
            Player.Pause();
        }
    }
}
