// Copyright (c) Bili Copilot. All rights reserved.

using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security;
using BiliCopilot.UI.Toolkits;
using Microsoft.Graphics.Canvas;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.WinUI.Share.Base;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.Streaming.Adaptive;
using Windows.Storage;
using Windows.Storage.Streams;
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
        if (Player is null)
        {
            return Task.CompletedTask;
        }

        _isDisposed = true;
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
        => !_isDisposed && Player?.PlaybackSession?.NaturalDuration.Ticks > 0;

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
    protected override Task ForcePlayAsync()
    {
        if (IsMediaLoaded())
        {
            Player.Play();
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override Task OnSeekAsync(TimeSpan position)
    {
        if (!Player?.CanSeek ?? false)
        {
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.CanNotSeek), InfoType.Warning));
            return Task.CompletedTask;
        }

        Player.Position = position;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override void OnSetVolume(int value)
    {
        if (Player is null)
        {
            return;
        }

        Player.Volume = value / 100.0;
    }

    /// <inheritdoc/>
    protected override void OnSetSpeed(double speed)
    {
        if (Player is null)
        {
            return;
        }

        Player.PlaybackRate = speed;
        _speedAction?.Invoke(speed);
    }

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
        else if (IsWebDav)
        {
            await LoadWebDavAsync();
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

    private HttpClient GetWebDavClient()
    {
        var httpClient = new HttpClient();
        var auth = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_webDavConfig.UserName}:{_webDavConfig.Password}"))}";
        httpClient.DefaultRequestHeaders.Add("Authorization", auth);
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
        source.MediaSource.AdvancedSettings.AllSegmentsIndependent = false;
        Debug.Assert(source.Status == AdaptiveMediaSourceCreationStatus.Success, "解析MPD失败");
        var videoSource = MediaSource.CreateFromAdaptiveMediaSource(source.MediaSource);
        Player.Source = new MediaPlaybackItem(videoSource);
    }

    private async Task LoadWebDavAsync()
    {
        var stream = await HttpRandomAccessStream.CreateAsync(GetWebDavClient(), new Uri(_videoUrl));
        var source = MediaSource.CreateFromStream(stream, _contentType);
        Player.Source = new MediaPlaybackItem(source);
    }

    private void LoadLiveSource(string url)
    {
        var source = MediaSource.CreateFromUri(new Uri(url));
        Player.Source = new MediaPlaybackItem(source);
    }

    internal sealed partial class HttpRandomAccessStream : IRandomAccessStreamWithContentType
    {
        private readonly Uri _requestedUri;
        private HttpClient _client;
        private IInputStream _inputStream;
        private ulong _size;
        private string _etagHeader;
        private bool _isDisposing;

        // No public constructor, factory methods instead to handle async tasks.
        private HttpRandomAccessStream(HttpClient client, Uri uri)
        {
            _client = client;
            _requestedUri = uri;
            Position = 0;
        }

        public ulong Position { get; private set; }

        public string ContentType { get; private set; } = string.Empty;

        public bool CanRead => true;

        public bool CanWrite => false;

        public ulong Size
        {
            get => (ulong)_size;
            set => throw new NotImplementedException();
        }

        public static Task<HttpRandomAccessStream> CreateAsync(HttpClient client, Uri uri)
        {
            var randomStream = new HttpRandomAccessStream(client, uri);

            return AsyncInfo.Run(async (cancellationToken) =>
            {
                await randomStream.SendRequesAsync();
                return randomStream;
            }).AsTask();
        }

        public IRandomAccessStream CloneStream() => this;

        public IInputStream GetInputStreamAt(ulong position)
            => _inputStream;

        public IOutputStream GetOutputStreamAt(ulong position)
            => throw new NotImplementedException();

        public void Seek(ulong position)
        {
            if (Position != position)
            {
                if (_inputStream != null)
                {
                    _inputStream.Dispose();
                    _inputStream = null;
                }

                Debug.WriteLine("Seek: {0:N0} -> {1:N0}", Position, position);
                Position = position;
            }
        }

        public void Dispose()
        {
            if (_isDisposing)
            {
                return;
            }

            _isDisposing = true;
            if (_inputStream != null)
            {
                _inputStream.Dispose();
                _inputStream = null;
            }

            if (_client != null)
            {
                _client?.Dispose();
                _client = null;
            }
        }

        public IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(IBuffer buffer, uint count, InputStreamOptions options)
        {
            return AsyncInfo.Run<IBuffer, uint>(async (cancellationToken, progress) =>
            {
                if (_isDisposing)
                {
                    return default;
                }

                progress.Report(0);

                try
                {
                    if (_inputStream == null)
                    {
                        await SendRequesAsync();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                if (_inputStream != null)
                {
                    var result = await _inputStream.ReadAsync(buffer, count, options).AsTask(cancellationToken, progress).ConfigureAwait(false);

                    // Move position forward.
                    Position += result.Length;
                    Debug.WriteLine("requestedPosition = {0:N0}", Position);
                    return result;
                }

                return default;
            });
        }

        public IAsyncOperation<bool> FlushAsync()
            => throw new NotImplementedException();

        public IAsyncOperationWithProgress<uint, uint> WriteAsync(IBuffer buffer)
            => throw new NotImplementedException();

        private async Task SendRequesAsync()
        {
            if (_isDisposing)
            {
                return;
            }

            var request = new HttpRequestMessage(HttpMethod.Get, _requestedUri);

            request.Headers.Add("Range", string.Format("bytes={0}-", Position));
            request.Headers.Add("Connection", "Keep-Alive");

            if (!string.IsNullOrEmpty(_etagHeader))
            {
                request.Headers.Add("If-Match", _etagHeader);
            }

            if (_client == null)
            {
                return;
            }

            var response = await _client.SendRequestAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead).AsTask().ConfigureAwait(false);

            if (response.Content.Headers.ContentType != null)
            {
                ContentType = response.Content.Headers.ContentType.MediaType;
            }

            _size = response.Content?.Headers?.ContentLength ?? 0;

            if (string.IsNullOrEmpty(_etagHeader) && response.Headers.ContainsKey("ETag"))
            {
                _etagHeader = response.Headers["ETag"];
            }

            if (response.Content.Headers.ContainsKey("Content-Type"))
            {
                ContentType = response.Content.Headers["Content-Type"];
            }

            _inputStream = await response.Content.ReadAsInputStreamAsync().AsTask().ConfigureAwait(false);
        }
    }
}
