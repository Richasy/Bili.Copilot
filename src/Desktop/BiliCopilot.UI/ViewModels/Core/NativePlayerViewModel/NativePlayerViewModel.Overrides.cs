// Copyright (c) Bili Copilot. All rights reserved.

using System.Diagnostics;
using System.Security;
using Microsoft.Graphics.Canvas;
using Richasy.BiliKernel.Bili.Authorization;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.Streaming.Adaptive;
using Windows.Storage;
using Windows.Web.Http;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 本地播放器视图模型.
/// </summary>
public sealed partial class NativePlayerViewModel
{
    /// <inheritdoc/>
    protected override Task OnCloseAsync()
    {
        _element.SetMediaPlayer(default);
        if (Player?.PlaybackSession != null)
        {
            Player.PlaybackSession.PositionChanged -= OnMediaPositionChanged;
        }

        Player.MediaOpened -= OnMediaPlayerOpened;
        Player.CurrentStateChanged -= OnMediaPlayerStateChanged;
        Player.MediaFailed -= OnMediaPlayerFailed;
        Player.MediaEnded -= OnMediaPlayerEnded;
        Player?.Dispose();
        Player = null;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override bool IsMediaLoaded()
        => Player?.PlaybackSession?.NaturalDuration.Ticks > 0;

    /// <inheritdoc/>
    protected override Task OnTogglePlayPauseAsync()
    {
        if (!IsMediaLoaded())
        {
            return Task.CompletedTask;
        }

        if (Player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
        {
            Player.Pause();
        }
        else
        {
            Player.Play();
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override Task OnSeekAsync(TimeSpan position)
    {
        Player.Position = position;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override void OnSetVolume(int value)
        => Player.Volume = value / 100.0;

    /// <inheritdoc/>
    protected override void OnSetSpeed(double speed)
        => Player.PlaybackRate = speed;

    /// <inheritdoc/>
    protected override async Task OnLoadPlayDataAsync()
    {
        if (Player is not null)
        {
            await OnCloseAsync();
        }

        Player = CreatePlayer();
        Player.AutoPlay = _autoPlay;

        if (IsLive)
        {
            LoadLiveSource(_videoUrl);
        }
        else
        {
            await LoadDashVideoSourceAsync();
        }

        _element.SetMediaPlayer(Player);
    }

    /// <inheritdoc/>
    protected override async Task OnTakeScreenshotAsync(string path)
    {
        var renderTarget = new CanvasRenderTarget(
                    CanvasDevice.GetSharedDevice(),
                    Player.PlaybackSession.NaturalVideoWidth,
                    Player.PlaybackSession.NaturalVideoHeight,
                    96);
        Player.CopyFrameToVideoSurface(renderTarget);
        await renderTarget.SaveAsync(path, CanvasBitmapFileFormat.Png);
    }

    private HttpClient GetVideoClient()
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Referer", "https://www.bilibili.com/");
        httpClient.DefaultRequestHeaders.Add("User-Agent", VideoUserAgent);
        httpClient.DefaultRequestHeaders.Add("Cookie", this.Get<IBiliCookiesResolver>().GetCookieString());
        return httpClient;
    }

    private async Task LoadDashVideoSourceAsync()
    {
        var httpClient = GetVideoClient();
        var onlyVideo = !string.IsNullOrEmpty(_videoUrl) && string.IsNullOrEmpty(_audioUrl);
        var onlyAudio = string.IsNullOrEmpty(_videoUrl) && !string.IsNullOrEmpty(_audioUrl);
        var mpdFilePath = onlyVideo ? "ms-appx:///Assets/DashVideoWithoutAudioTemplate.xml" : onlyAudio ? "ms-appx:///Assets/DashAudioWithoutVideoTemplate.xml" : "ms-appx:///Assets/DashVideoTemplate.xml";
        var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(mpdFilePath));
        var mpdStr = await FileIO.ReadTextAsync(file);

        var videoStr =
                $"""
                <Representation bandwidth="{_videoSegment.Bandwidth}" codecs="{_videoSegment.Codecs}" height="{_videoSegment.Height}" mimeType="{_videoSegment.MimeType}" id="{_videoSegment.Id}" width="{_videoSegment.Width}" startWithSap="1">
                    <BaseURL>{SecurityElement.Escape(_videoUrl)}</BaseURL>
                    <SegmentBase indexRange="{_videoSegment.IndexRange}">
                        <Initialization range="{_videoSegment.Initialization}" />
                    </SegmentBase>
                </Representation>
                """;

        var audioStr = string.Empty;
        if (!string.IsNullOrEmpty(_audioUrl))
        {
            audioStr =
                    $"""
                    <Representation bandwidth="{_audioSegment.Bandwidth}" codecs="{_audioSegment.Codecs}" mimeType="{_audioSegment.MimeType}" id="{_audioSegment.Id}">
                        <BaseURL>{SecurityElement.Escape(_audioUrl)}</BaseURL>
                        <SegmentBase indexRange="{_audioSegment.IndexRange}">
                            <Initialization range="{_audioSegment.Initialization}" />
                        </SegmentBase>
                    </Representation>
                    """;
        }

        videoStr = videoStr.Trim();
        audioStr = audioStr.Trim();
        mpdStr = mpdStr.Replace("{video}", videoStr)
                       .Replace("{audio}", audioStr)
                       .Replace("{bufferTime}", $"PT4S");

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(mpdStr)).AsInputStream();
        var url = onlyAudio ? _audioUrl : _videoUrl;
        var source = await AdaptiveMediaSource.CreateFromStreamAsync(stream, new Uri(url), "application/dash+xml", httpClient);
        source.MediaSource.AdvancedSettings.AllSegmentsIndependent = true;
        Debug.Assert(source.Status == AdaptiveMediaSourceCreationStatus.Success, "解析MPD失败");
        var videoSource = MediaSource.CreateFromAdaptiveMediaSource(source.MediaSource);
        Player.Source = new MediaPlaybackItem(videoSource);
    }

    private void LoadLiveSource(string url)
    {
        var source = MediaSource.CreateFromUri(new Uri(url));
        Player.Source = new MediaPlaybackItem(source);
    }
}
