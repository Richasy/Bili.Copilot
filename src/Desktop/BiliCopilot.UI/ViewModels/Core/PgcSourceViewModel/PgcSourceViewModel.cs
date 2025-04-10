// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.Media;
using Richasy.MpvKernel.Core.Enums;
using Richasy.MpvKernel.Core.Models;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// PGC 源视图模型.
/// </summary>
public sealed partial class PgcSourceViewModel : ViewModelBase, IMediaSourceResolver
{
    public PgcSourceViewModel(
        IPlayerService service,
        IFavoriteService favoriteService,
        IEntertainmentDiscoveryService discoveryService,
        ILogger<PgcSourceViewModel> logger,
        DanmakuViewModel danmaku,
        SubtitleViewModel subtitle,
        CommentMainViewModel comments,
        DownloadViewModel downloader)
    {
        _service = service;
        _favoriteService = favoriteService;
        _discoveryService = discoveryService;
        _logger = logger;
        Danmaku = danmaku;
        Subtitle = subtitle;
        Downloader = downloader;
        CommentSection = comments;
    }

    public event EventHandler RequestReload;

    public event EventHandler RequestClear;

    public void InjectMedia(MediaIdentifier identifier)
        => _cachedMedia = identifier;

    public async Task InitializeAsync()
    {
        _isSeasonInitialized = false;
        _videoUrl = string.Empty;
        _audioUrl = string.Empty;
        Id = _cachedMedia?.Id ?? string.Empty;
        ErrorMessage = string.Empty;
        try
        {
            RequestClear?.Invoke(this, EventArgs.Empty);

            _initialProgress = 0;
            _lastPosition = 0;
            ClearView();
            var id = _cachedMedia!.Value.Id;
            var isEpisode = id.StartsWith("ep");
            id = id[3..];
            var seasonId = isEpisode ? default : id;
            var episodeId = isEpisode ? id : default;
            var view = await _service.GetPgcPageDetailAsync(seasonId, episodeId);
            InitializeView(view);
            var initialEpisode = FindInitialEpisode(episodeId);
            ChangeEpisode(initialEpisode);
            InitializeSections();
            InitializeEpisodeNavigation();
        }
        catch (Exception ex)
        {
            if (ex is not TaskCanceledException)
            {
                ErrorMessage = ex.Message;
                _logger.LogError(ex, $"尝试获取剧集 {_cachedMedia!.Value.Id} 详情时失败.");
            }
        }
    }

    public (string url, MpvPlayOptions options) GetSource()
    {
        if (_view is null)
        {
            return (string.Empty, default);
        }

        var options = new MpvPlayOptions();
        var headers = new Dictionary<string, string>();
        var cookies = this.Get<IBiliCookiesResolver>().GetCookieString();
        var referer = VideoReferer;
        var userAgent = VideoUserAgent;
        headers.Add("Cookie", cookies);
        headers.Add("Referer", referer);
        options.HttpHeaders = headers;
        options.UserAgent = userAgent;
        options.EnableCookies = true;
        options.StartPosition = _initialProgress;
        var fileUrl = string.Empty;
        if (!string.IsNullOrEmpty(_videoUrl))
        {
            fileUrl = _videoUrl;
            if (!string.IsNullOrEmpty(_audioUrl))
            {
                options.ExtraAudioUrl = _audioUrl;
            }
        }
        else if (!string.IsNullOrEmpty(_audioUrl))
        {
            fileUrl = _audioUrl;
        }

        return (fileUrl, options);
    }

    public string GetTitle()
        => _view.Information.Identifier.Title;

    public void HandlePlayerStateChanged(MpvPlayerState state)
    {
        if (_view == null)
        {
            return;
        }

        if (state == MpvPlayerState.Playing)
        {
            Danmaku?.Resume();
        }
        else
        {
            Danmaku?.Pause();
            if (state == MpvPlayerState.Paused)
            {
                // 记录播放进度.
                ReportProgressCommand.Execute(Convert.ToInt32(_lastPosition));
            }
            else if (state == MpvPlayerState.End || state == MpvPlayerState.Idle)
            {
                // 清除弹幕.
                Danmaku.ClearDanmaku();
                Subtitle.ClearSubtitle();

                if (state == MpvPlayerState.End)
                {
                    ReportProgressCommand.Execute(Convert.ToInt32(_lastPosition));

                    var autoNext = SettingsToolkit.ReadLocalSetting(SettingNames.AutoPlayNext, true);
                    if (!autoNext)
                    {
                        return;
                    }

                    var next = FindNextEpisode();
                    if (next is null)
                    {
                        return;
                    }

                    PlayNextEpisodeCommand.Execute(default);
                }
            }
        }
    }

    public void HandleProgressChanged(double position, double duration)
    {
        _lastPosition = position;
        if (position < duration && position > 1 && (Danmaku?.IsEmpty() ?? false))
        {
            var aid = _episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Aid).ToString();
            var cid = _episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Cid).ToString();
            Danmaku.ResetData(aid, cid);
            Danmaku.LoadDanmakusCommand.Execute(Convert.ToInt32(duration));
        }

        Danmaku?.UpdatePosition(Convert.ToInt32(position));
        Subtitle?.UpdatePosition(Convert.ToInt32(position));
    }

    public void HandleSpeedChanged(double speed)
    {
        if (Danmaku is not null)
        {
            Danmaku.ExtraSpeed = speed;
        }
    }

    [RelayCommand]
    private async Task InitializeDashMediaAsync(EpisodeInformation episode)
    {
        try
        {
            ErrorMessage = string.Empty;
            var cid = episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Cid);
            var seasonType = episode.GetExtensionIfNotNull<string>(EpisodeExtensionDataId.SeasonType);
            var info = await _service.GetPgcPlayDetailAsync(cid.ToString(), episode.Identifier.Id, seasonType);
            InitializeDash(info);
        }
        catch (Exception ex)
        {
            if (ex is not TaskCanceledException)
            {
                ErrorMessage = ex.Message;
                _logger.LogError(ex, $"尝试获取视频 {_view.Information.Identifier.Id} 的第 {episode.Index} 分集时失败.");
            }
        }
    }

    [RelayCommand]
    private void ChangeFormat(PlayerFormatItemViewModel vm)
    {
        if (vm is null || vm == SelectedFormat)
        {
            return;
        }

        var isFirstSet = SelectedFormat == default;
        SelectedFormat = vm;
        var maxAudioQuality = _audioSegments?.Max(p => Convert.ToInt32(p.Id));
        var preferCodec = AppToolkit.GetPreferCodecId();
        var vSeg = _videoSegments?.FirstOrDefault(p => p.Id == vm.Data.Quality.ToString() && p.Codecs.Contains(preferCodec))
            ?? _videoSegments?.FirstOrDefault(p => p.Id == vm.Data.Quality.ToString());
        var aSeg = _audioSegments?.FirstOrDefault(p => p.Id == maxAudioQuality.ToString());
        var videoUrl = vSeg?.BaseUrl;
        var audioUrl = aSeg?.BaseUrl;

        var isAutoPlay = !isFirstSet;
        if (isFirstSet)
        {
            isAutoPlay = SettingsToolkit.ReadLocalSetting(SettingNames.ShouldAutoPlay, true);
        }
        else
        {
            _initialProgress = _lastPosition;
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedPgcQuality, vm.Data.Quality);

        Danmaku.ClearAll();
        _videoUrl = videoUrl;
        _audioUrl = audioUrl;
        RequestReload?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task ReportProgressAsync(int progress)
    {
        var shouldReport = SettingsToolkit.ReadLocalSetting(SettingNames.ShouldReportProgress, true);
        if (!shouldReport || _view is null || _episode is null || _view.Information is null || progress == 0)
        {
            return;
        }

        try
        {
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            var aid = _episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Aid);
            var cid = _episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Cid);
            await _service.ReportEpisodeProgressAsync(aid.ToString(), cid.ToString(), _episode.Identifier.Id, _view.Information.Identifier.Id, progress, cancellationToken.Token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试上报播放进度时失败.");
        }
    }
}
