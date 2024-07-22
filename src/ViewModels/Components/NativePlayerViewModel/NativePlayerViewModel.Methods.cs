// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Player;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Graphics.Canvas;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.Streaming.Adaptive;
using Windows.Storage;
using Windows.System;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 原生播放器视图模型.
/// </summary>
public sealed partial class NativePlayerViewModel
{
    /// <inheritdoc/>
    public MediaStats GetMediaInformation() => default;

    [RelayCommand]
    private static void StartRecording()
    {
        // 暂不支持录制.
        return;
    }

    [RelayCommand]
    private static Task StopRecordingAsync()
        => Task.CompletedTask;

    private static HttpClient GetVideoClient()
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Referer", "https://www.bilibili.com");
        httpClient.DefaultRequestHeaders.Add("User-Agent", ServiceConstants.DefaultUserAgentString);
        httpClient.DefaultRequestHeaders.Add("Cookie", AuthorizeProvider.GetCookieString());
        return httpClient;
    }

    private HttpClient GetWebDavHttpClient()
    {
        var handler = new HttpBaseProtocolFilter();
        handler.AutomaticDecompression = true;
        var httpClient = new HttpClient(handler);
        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_webDavVideo.Config.UserName}:{_webDavVideo.Config.Password}"));
        httpClient.DefaultRequestHeaders.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("Basic", token);
        return httpClient;
    }

    [RelayCommand]
    private async Task TakeScreenshotAsync()
    {
        if (Player == null)
        {
            return;
        }

        var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "哔哩截图");
        if (!Directory.Exists(folderPath))
        {
            _ = Directory.CreateDirectory(folderPath);
        }

        var fileName = $"{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.png";
        var path = Path.Combine(folderPath, fileName);
        var player = (MediaPlayer)Player;
        var renderTarget = new CanvasRenderTarget(
                    CanvasDevice.GetSharedDevice(),
                    player.PlaybackSession.NaturalVideoWidth,
                    player.PlaybackSession.NaturalVideoHeight,
                    96);
        player.CopyFrameToVideoSurface(renderTarget);
        await renderTarget.SaveAsync(path, CanvasBitmapFileFormat.Png);
        _ = _dispatcherQueue.TryEnqueue(async () =>
        {
            if (File.Exists(path))
            {
                var storageFile = await StorageFile.GetFileFromPathAsync(path).AsTask();
                _ = await Launcher.LaunchFileAsync(storageFile);
            }
        });
    }

    private async Task LoadDashVideoSourceAsync(bool audioOnly)
    {
        var httpClient = GetVideoClient();
        var mpdFilePath =
            audioOnly
            ? AppConstants.DashAudioWithoutVideoMPDFile
            : _audio == null
                ? AppConstants.DashVideoWithoutAudioMPDFile
                : AppConstants.DashVideoMPDFile;
        var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(mpdFilePath));
        var mpdStr = await FileIO.ReadTextAsync(file);

        var videoUrl = await PlayerProvider.GetAvailableUrlAsync([_video.BaseUrl, .._video.BackupUrls]);
        var videoStr =
                $@"<Representation bandwidth=""{_video.Bandwidth}"" codecs=""{_video.Codecs}"" height=""{_video.Height}"" mimeType=""{_video.MimeType}"" id=""{_video.Id}"" width=""{_video.Width}"" startWithSap=""{_video.StartWithSap}"">
                               <BaseURL>{SecurityElement.Escape(videoUrl)}</BaseURL>
                               <SegmentBase indexRange=""{_video.IndexRange}"">
                                   <Initialization range=""{_video.Initialization}"" />
                               </SegmentBase>
                           </Representation>";

        var audioStr = string.Empty;

        if (_audio != null)
        {
            var audioUrl = await PlayerProvider.GetAvailableUrlAsync([_audio.BaseUrl, .._audio.BackupUrls]);
            audioStr =
                    $@"<Representation bandwidth=""{_audio.Bandwidth}"" codecs=""{_audio.Codecs}"" mimeType=""{_audio.MimeType}"" id=""{_audio.Id}"">
                               <BaseURL>{SecurityElement.Escape(audioUrl)}</BaseURL>
                               <SegmentBase indexRange=""{_audio.IndexRange}"">
                                   <Initialization range=""{_audio.Initialization}"" />
                               </SegmentBase>
                           </Representation>";
        }

        videoStr = videoStr.Trim();
        audioStr = audioStr.Trim();
        mpdStr = mpdStr.Replace("{video}", videoStr)
                       .Replace("{audio}", audioStr)
                       .Replace("{bufferTime}", $"PT4S");

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(mpdStr)).AsInputStream();
        var source = await AdaptiveMediaSource.CreateFromStreamAsync(stream, new Uri(_video.BaseUrl), "application/dash+xml", httpClient);
        source.MediaSource.AdvancedSettings.AllSegmentsIndependent = true;
        Debug.Assert(source.Status == AdaptiveMediaSourceCreationStatus.Success, "解析MPD失败");
        var videoSource = MediaSource.CreateFromAdaptiveMediaSource(source.MediaSource);
        var videoPlaybackItem = new MediaPlaybackItem(videoSource);

        Player ??= GetVideoPlayer();
        var player = Player as MediaPlayer;
        player.Source = videoPlaybackItem;
    }

    private Task LoadLiveSourceAsync(string url)
    {
        var videoSource = MediaSource.CreateFromUri(new Uri(url));
        var videoPlaybackItem = new MediaPlaybackItem(videoSource);

        var player = GetVideoPlayer();
        player.Source = videoPlaybackItem;
        Player = player;
        return Task.CompletedTask;
    }

    private async Task LoadWebDavVideoAsync()
    {
        var httpClient = GetWebDavHttpClient();
        var server = AppToolkit.GetWebDavServer(_webDavVideo.Config.Host, _webDavVideo.Config.Port, _webDavVideo.Href);
        var stream = await HttpRandomAccessStream.CreateAsync(httpClient, new Uri(server + _webDavVideo.Href));
        var videoSource = MediaSource.CreateFromStream(stream, _webDavVideo.ContentType);
        var videoPlaybackItem = new MediaPlaybackItem(videoSource);

        Player ??= GetVideoPlayer();
        var player = Player as MediaPlayer;
        player.Source = videoPlaybackItem;
    }

    private MediaPlayer GetVideoPlayer()
    {
        var player = new MediaPlayer();
        player.CommandManager.IsEnabled = false;
        player.MediaOpened += OnMediaPlayerOpened;
        player.CurrentStateChanged += OnMediaPlayerCurrentStateChanged;
        player.MediaEnded += OnMediaPlayerEnded;
        player.MediaFailed += OnMediaPlayerFailed;
        return player;
    }

    private void OnMediaPlayerFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
    {
        if (args.ExtendedErrorCode?.HResult == -1072873851 || args.Error == MediaPlayerError.Unknown)
        {
            // 不处理 Shutdown 造成的错误.
            return;
        }

        _dispatcherQueue.TryEnqueue(() =>
        {
            // 在视频未加载时不对报错进行处理.
            if (Status == PlayerStatus.NotLoad)
            {
                return;
            }

            var message = string.Empty;
            switch (args.Error)
            {
                case MediaPlayerError.Aborted:
                    message = ResourceToolkit.GetLocalizedString(StringNames.Aborted);
                    break;
                case MediaPlayerError.NetworkError:
                    message = ResourceToolkit.GetLocalizedString(StringNames.NetworkError);
                    break;
                case MediaPlayerError.DecodingError:
                    message = ResourceToolkit.GetLocalizedString(StringNames.DecodingError);
                    break;
                case MediaPlayerError.SourceNotSupported:
                    message = ResourceToolkit.GetLocalizedString(StringNames.SourceNotSupported);
                    break;
                default:
                    break;
            }

            Status = PlayerStatus.Failed;
            var arg = new MediaStateChangedEventArgs(Status, args.ErrorMessage);
            StateChanged?.Invoke(this, arg);
            LogException(new Exception($"播放失败: {args.Error} | {args.ErrorMessage} | {args.ExtendedErrorCode}"));
        });
    }

    private void OnMediaPlayerEnded(MediaPlayer sender, object args)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            Status = PlayerStatus.End;
            MediaEnded?.Invoke(this, EventArgs.Empty);
        });
    }

    private void OnMediaPlayerCurrentStateChanged(MediaPlayer sender, object args)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            try
            {
                Status = sender.PlaybackSession.PlaybackState switch
                {
                    MediaPlaybackState.Opening => PlayerStatus.Opened,
                    MediaPlaybackState.Playing => PlayerStatus.Playing,
                    MediaPlaybackState.Buffering => PlayerStatus.Buffering,
                    MediaPlaybackState.Paused => PlayerStatus.Pause,
                    _ => PlayerStatus.NotLoad,
                };
            }
            catch (Exception)
            {
                Status = PlayerStatus.Failed;
            }

            OnPropertyChanged(nameof(IsPlayerReady));
            StateChanged?.Invoke(this, new MediaStateChangedEventArgs(Status, string.Empty));
        });
    }

    private void OnMediaPlayerOpened(MediaPlayer sender, object args)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            var session = sender.PlaybackSession;
            if (session != null)
            {
                session.PositionChanged -= OnPlayerPositionChanged;

                if (_video != null || _webDavVideo != null)
                {
                    session.PositionChanged += OnPlayerPositionChanged;
                }

                if (sender.Source is MediaPlaybackItem item && _webDavVideo != null)
                {
                    LoadWebDavTimedMetadataTracks(item);
                }
            }

            MediaOpened?.Invoke(this, EventArgs.Empty);
        });
    }

    private void OnPlayerPositionChanged(MediaPlaybackSession sender, object args)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            try
            {
                var duration = sender.NaturalDuration.TotalSeconds;
                var progress = sender.Position.TotalSeconds;
                if (progress > duration)
                {
                    Pause();
                    return;
                }

                PositionChanged?.Invoke(this, new MediaPositionChangedEventArgs(sender.Position, sender.NaturalDuration));
            }
            catch (Exception)
            {
            }
        });
    }

    private void LoadWebDavTimedMetadataTracks(MediaPlaybackItem sender)
    {
        var list = sender.TimedMetadataTracks.Where(p => p.TimedMetadataKind == TimedMetadataKind.Subtitle).ToList();
        var metaList = new List<SubtitleMeta>();
        foreach (var item in list)
        {
            metaList.Add(new SubtitleMeta(item.Id, item.Label, item.Language));
            item.CueEntered += OnSubtitleEntered;
            item.CueExited += OnSubtitleExited;
        }

        var lan = FlyleafLib.Utils.GetSystemLanguages().First();
        var arg = new WebDavSubtitleListChangedEventArgs
        {
            Subtitles = metaList,
            SelectedMeta = metaList.FirstOrDefault(p => p.Url.StartsWith(lan.TopCulture.IetfLanguageTag)) ?? metaList.FirstOrDefault(),
        };

        WebDavSubtitleListChanged?.Invoke(this, arg);

        void OnSubtitleEntered(TimedMetadataTrack sender, MediaCueEventArgs args)
        {
            if (args.Cue is TimedTextCue cue)
            {
                var text = string.Join("\n", cue.Lines.Select(p => p.Text));
                WebDavSubtitleChanged?.Invoke(this, text);
            }
        }

        void OnSubtitleExited(TimedMetadataTrack sender, MediaCueEventArgs args)
        {
            WebDavSubtitleChanged?.Invoke(this, string.Empty);
        }
    }

    [RelayCommand]
    private void Clear()
    {
        if (Player is not MediaPlayer player)
        {
            return;
        }

        if (player.PlaybackSession != null)
        {
            player.PlaybackSession.PositionChanged -= OnPlayerPositionChanged;
            player.CommandManager.IsEnabled = true;
        }

        player.MediaOpened -= OnMediaPlayerOpened;
        player.CurrentStateChanged -= OnMediaPlayerCurrentStateChanged;
        player.MediaEnded -= OnMediaPlayerEnded;
        player.MediaFailed -= OnMediaPlayerFailed;

        if (player.Source is MediaPlaybackItem playbackItem
            && playbackItem.Source is not null)
        {
            playbackItem.Source.Dispose();
        }

        player.Source = null;
    }
}
