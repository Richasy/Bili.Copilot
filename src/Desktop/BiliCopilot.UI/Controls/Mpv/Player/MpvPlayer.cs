﻿// Copyright (c) Bili Copilot. All rights reserved.
// <auto-generated />

using System.Diagnostics;
using BiliCopilot.UI.Controls.Mpv.Common;
using Mpv.Core;
using Mpv.Core.Args;
using Mpv.Core.Enums.Client;
using Mpv.Core.Enums.Player;
using OpenTK.Graphics.OpenGL;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Models.Media;
using Windows.Storage;

namespace BiliCopilot.UI.Controls.Mpv;

/// <summary>
/// MPV 播放器.
/// </summary>
public sealed partial class MpvPlayer : Control
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MpvPlayer"/> class.
    /// </summary>
    public MpvPlayer()
    {
        DefaultStyleKey = typeof(MpvPlayer);
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _positionBlock = (TextBlock)GetTemplateChild("PositionBlock");
        _renderControl = (RenderControl)GetTemplateChild("RenderControl");
        _playPauseButton = (Button)GetTemplateChild("PlayPauseButton");
        _skipForwardButton = (Button)GetTemplateChild("SkipForwardButton");
        _skipBackwardButton = (Button)GetTemplateChild("SkipBackwardButton");
        _playRateComboBox = (ComboBox)GetTemplateChild("PlayRateComboBox");
        _renderControl.Setting = new ContextSettings()
        {
            MajorVersion = 4,
            MinorVersion = 6,
            GraphicsProfile = OpenTK.Windowing.Common.ContextProfile.Compatability,
        };
        _renderControl.Render += OnRender;
        _playPauseButton.Click += OnPlayPauseButtonClick;
        _skipForwardButton.Click += OnSkipForwardButtonClick;
        _skipBackwardButton.Click += OnSkipBackwardButtonClick;
        _playRateComboBox.SelectionChanged += OnPlayRateSelectionChanged;
    }

    private void OnPlayRateSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = _playRateComboBox.SelectedItem as ComboBoxItem;
        var rate = Convert.ToDouble(item.Content);
        Player.SetPlayRate(rate);
    }

    private void OnSkipBackwardButtonClick(object sender, RoutedEventArgs e)
    {
        if (Player.Position == null)
        {
            return;
        }
        Player?.Seek(Player.Position!.Value.Add(TimeSpan.FromSeconds(-10)));
    }

    private void OnSkipForwardButtonClick(object sender, RoutedEventArgs e)
    {
        if (Player.Position == null)
        {
            return;
        }
        Player?.Seek(Player.Position!.Value.Add(TimeSpan.FromSeconds(30)));
    }

    private void OnPlayPauseButtonClick(object sender, RoutedEventArgs e)
    {
        Player?.TogglePlayPause();
    }

    private void OnRender(TimeSpan e)
    {
        Render();
    }

    /// <summary>
    /// 打开文件.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public async Task OpenAsync(StorageFile file)
    {
        Player ??= new Player();

        if (!Player.Client.IsInitialized)
        {
            Player.PlaybackPositionChanged += OnPositionChanged;
            Player.PlaybackStateChanged += OnStateChanged;
            _renderControl.Initialize();
            Player.Client.SetProperty("vo", "libmpv");
            Player.Client.RequestLogMessage(MpvLogLevel.V);
            Player.LogMessageReceived += OnLogMessageReceived;
            var args = new InitializeArgument(default, func: RenderContext.GetProcAddress);
            await Player.InitializeAsync(args);
        }

        await Player.OpenAsync(file.Path);
    }

    /// <summary>
    /// 打开网络媒体.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task OpenAsync(DashMediaInformation info)
    {
        Player ??= new Player();
        if (!Player.Client.IsInitialized)
        {
            Player.PlaybackPositionChanged += OnPositionChanged;
            Player.PlaybackStateChanged += OnStateChanged;
            _renderControl.Initialize();
            Player.Client.SetProperty("vo", "libmpv");
            Player.Client.RequestLogMessage(MpvLogLevel.V);
            Player.LogMessageReceived += OnLogMessageReceived;
            var args = new InitializeArgument(default, func: RenderContext.GetProcAddress);
            await Player.InitializeAsync(args);
        }

        var cookies = this.Get<IBiliCookiesResolver>().GetCookieString();
        var maxQuality = info.Formats.Max(p => p.Quality);
        var video = info.Videos.First(p => p.Id == maxQuality.ToString());
        var videoUrls = new List<string> { video.BaseUrl }.Concat(video.BackupUrls).Select(p => new Uri(p)).ToList();
        var videoUrl = videoUrls.First(p => p.Port == 443 || p.Port == 80).ToString();

        var audio = info.Audios.OrderByDescending(p => Convert.ToInt32(p.Id)).First();
        var audioUrls = new List<string> { audio.BaseUrl }.Concat(audio.BackupUrls).Select(p => new Uri(p)).ToList();
        var audioUrl = audioUrls.First(p => p.Port == 443 || p.Port == 80).ToString();
        // await Player.Client.ExecuteAsync("--cookies");
        //await Player.Client.ExecuteAsync($"--http-header-fields=\"Cookie: {cookies}\"");
        //await Player.Client.ExecuteAsync($"--http-header-fields=\"Referer: https://www.bilibili.com\"");
        //await Player.Client.ExecuteAsync($"--audio-file=\"{audioUrl}\"");
        var cookieStr = $"Cookie: {cookies}";
        var refererStr = $"Referer: https://www.bilibili.com";
        Player.Client.SetOption("cookies", "yes");
        Player.Client.SetOption("user-agent", "bili-universal/80800100 os/ios model/iPhone 14 Pro mobi_app/iphone osVer/17.6.1 network/2 grpc-objc-cronet/1.47.0 grpc-c/25.0.0 (ios; cronet_http)");
        Player.Client.SetOption("http-header-fields", $"{cookieStr}\n{refererStr}");
        // Player.Client.SetOption("audio-add", audioUrl);
        await Player.Client.ExecuteAsync(new string[] { "loadfile", videoUrl, "replace" });
        await Player.Client.ExecuteAsync(new string[] { "audio-add", audioUrl });
    }

    /// <summary>
    /// 释放资源.
    /// </summary>
    public void Close()
    {
        Player?.Dispose();
    }

    private void OnStateChanged(object sender, PlaybackStateChangedEventArgs e)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            if (e.NewState == PlaybackState.Playing)
            {
                VisualStateManager.GoToState(this, "PlayState", false);
            }
            else if (e.NewState == PlaybackState.Paused || e.NewState == PlaybackState.None)
            {
                VisualStateManager.GoToState(this, "PauseState", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "LoadingState", false);
            }
        });
    }

    private void OnPositionChanged(object sender, PlaybackPositionChangedEventArgs e)
    {
        var duration = TimeSpan.FromSeconds(e.Duration);
        var position = TimeSpan.FromSeconds(e.Position);
        DispatcherQueue.TryEnqueue(() =>
        {
            _positionBlock.Text = $"{position:mm\\:ss} / {duration:mm\\:ss}";
        });
    }

    /// <summary>
    /// 播放.
    /// </summary>
    public void Play()
        => Player?.Play();

    /// <summary>
    /// 暂停.
    /// </summary>
    public void Pause()
        => Player?.Pause();

    private void OnLogMessageReceived(object sender, LogMessageReceivedEventArgs e)
    {
        Debug.WriteLine($"[{e.Level}]\t{e.Prefix}: {e.Message}");
    }

    private void Render()
    {
        if (Player?.Client?.IsInitialized is not true)
        {
            return;
        }

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        Player.RenderGL((int)(ActualWidth * _renderControl.ScaleX), (int)(ActualHeight * _renderControl.ScaleY), _renderControl.GetBufferHandle());
    }
}
