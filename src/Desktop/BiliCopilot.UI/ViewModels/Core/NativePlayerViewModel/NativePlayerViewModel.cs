// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Models.Media;
using Windows.Media.Playback;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 本地播放器视图模型.
/// </summary>
public sealed partial class NativePlayerViewModel : PlayerViewModelBase
{
    private MediaPlayerElement? _element;
    private DashSegmentInformation? _videoSegment;
    private DashSegmentInformation? _audioSegment;
    private WebDavConfig? _webDavConfig;
    private bool _isDisposed;

    /// <summary>
    /// 媒体播放器.
    /// </summary>
    public MediaPlayer? Player { get; private set; }

    /// <summary>
    /// 初始化.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task InitializeAsync(MediaPlayerElement element)
    {
        _element = element;
        IsPlayerInitializing = true;
        if (Player is not null)
        {
            await CloseAsync();
        }

        IsPlayerInitializing = false;
        _isInitialized = true;
        await TryLoadPlayDataAsync();
    }

    /// <summary>
    /// 注入播放片段.
    /// </summary>
    public void InjectSegments(DashSegmentInformation? video, DashSegmentInformation? audio)
    {
        _videoSegment = video;
        _audioSegment = audio;
        UpdateState(PlayerState.None);
    }

    /// <inheritdoc/>
    protected override void SetWebDavConfig(WebDavConfig config)
    {
        _webDavConfig = config;
    }

    private MediaPlayer CreatePlayer()
    {
        _isDisposed = false;
        var player = new MediaPlayer();
        player.CommandManager.IsEnabled = false;
        player.MediaOpened += OnMediaPlayerOpened;
        player.CurrentStateChanged += OnMediaPlayerStateChanged;
        player.MediaFailed += OnMediaPlayerFailed;
        player.MediaEnded += OnMediaPlayerEnded;
        player.Volume = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerVolume, 100) / 100.0;

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
            if (!IsMediaLoaded())
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

            UpdateState(PlayerState.Failed);
            _logger.LogError($"播放失败: {args.Error} | {args.ErrorMessage} | {args.ExtendedErrorCode}");
        });
    }

    private void OnMediaPlayerEnded(MediaPlayer sender, object args)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (!IsMediaLoaded())
            {
                return;
            }

            ReachEnd();
        });
    }

    private void OnMediaPlayerStateChanged(MediaPlayer sender, object args)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (!IsMediaLoaded())
            {
                return;
            }

            var state = sender.PlaybackSession.PlaybackState switch
            {
                MediaPlaybackState.Opening => PlayerState.Opening,
                MediaPlaybackState.Buffering => PlayerState.Buffering,
                MediaPlaybackState.Playing => PlayerState.Playing,
                MediaPlaybackState.Paused => PlayerState.Paused,
                _ => PlayerState.None,
            };

            UpdateState(state);
        });
    }

    private void OnMediaPlayerOpened(MediaPlayer sender, object args)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            var session = sender.PlaybackSession;
            if (session is null)
            {
                return;
            }

            sender.PlaybackSession.PlaybackRate = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerSpeed, 1d);

            if (Position > 0)
            {
                session.Position = TimeSpan.FromSeconds(Position);
            }

            session.PositionChanged -= OnMediaPositionChanged;
            session.PositionChanged += OnMediaPositionChanged;
        });
    }

    private void OnMediaPositionChanged(MediaPlaybackSession sender, object args)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (!IsMediaLoaded())
            {
                return;
            }

            var duration = sender.NaturalDuration.TotalSeconds;
            var position = sender.Position.TotalSeconds;
            if (position > duration)
            {
                return;
            }

            if (!IsLive)
            {
                UpdatePosition(Convert.ToInt32(position), Convert.ToInt32(duration));
            }
        });
    }
}
