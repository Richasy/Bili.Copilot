// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using BiliCopilot.UI.ViewModels.View;
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

public sealed partial class VideoSourceViewModel : ViewModelBase, IMediaSourceResolver
{
    public VideoSourceViewModel(
        IPlayerService service,
        IRelationshipService relationshipService,
        IFavoriteService favoriteService,
        ILogger<VideoPlayerPageViewModel> logger,
        DanmakuViewModel danmaku,
        SubtitleViewModel subtitle,
        CommentMainViewModel comments,
        DownloadViewModel download,
        AIViewModel ai)
    {
        _service = service;
        _relationshipService = relationshipService;
        _favoriteService = favoriteService;
        _logger = logger;
        _comments = comments;
        Danmaku = danmaku;
        Subtitle = subtitle;
        Downloader = download;
        AI = ai;
        CurrentLoop = VideoLoopType.None;
        Subtitle.SetInitializedCallback(SyncDownloadAndSubtitle);
    }

    public event EventHandler RequestReload;

    /// <summary>
    /// 注入播放列表.
    /// </summary>
    public void InjectPlaylist(IList<VideoInformation> playlist)
        => _playlist = playlist;

    public void InjectSnapshot(VideoSnapshot snapshot)
        => _cachedSnapshot = snapshot;

    public async Task InitializeAsync()
    {
        var video = _cachedSnapshot.Video;
        IsAIOverlayOpened = false;
        IsPrivatePlay = _cachedSnapshot.IsPrivate;
        try
        {
            if (_view is not null && _view.Information.Identifier.Id != video.Identifier.Id)
            {
                // TODO: 记录上一个视频的播放进度.
                await ReportProgressAsync(_lastPosition);
            }

            _initialProgress = 0;
            _lastPosition = 0;
            ClearView();
            if (_playlist?.Any(p => p.Identifier.Id == video.Identifier.Id) == false)
            {
                _playlist = default;
            }

            var view = await _service.GetVideoPageDetailAsync(video.Identifier);
            InitializeView(view);
            var initialPart = FindInitialPart(default) ?? throw new Exception("无法找到视频的分集信息.");
            _part = initialPart;
            _comments.Initialize(AvId, Richasy.BiliKernel.Models.CommentTargetType.Video, Richasy.BiliKernel.Models.CommentSortType.Hot);
            LoadInitialProgress();
            InitializeSections();
            InitializeLoops();
            await ChangePartAsync(initialPart);
        }
        catch (Exception ex)
        {
            if (ex is not TaskCanceledException)
            {
                _logger.LogError(ex, $"尝试获取视频 {video.Identifier.Id} 详情时失败.");
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
                ReportProgressCommand.Execute(_lastPosition);
            }
            else if (state == MpvPlayerState.End || state == MpvPlayerState.Idle)
            {
                // 清除弹幕.
                Danmaku.ClearDanmaku();
                Subtitle.ClearSubtitle();

                if (state == MpvPlayerState.End)
                {
                    ReportProgressCommand.Execute(_lastPosition);

                    // 单视频循环.
                    if (CurrentLoop == VideoLoopType.Single)
                    {
                        _initialProgress = 0;
                        RequestReload?.Invoke(this, EventArgs.Empty);
                        return;
                    }

                    var autoNext = SettingsToolkit.ReadLocalSetting(SettingNames.AutoPlayNext, true);
                    // TODO: 返回默认显示模式.

                    var next = FindNextVideo();
                    if (next is null)
                    {
                        return;
                    }

                    var withoutTip = SettingsToolkit.ReadLocalSetting(SettingNames.PlayNextWithoutTip, false);
                    if (withoutTip)
                    {
                        PlayNextVideoCommand.Execute(default);
                    }
                    else
                    {
                        string tip = default;
                        if (next is VideoPart part)
                        {
                            tip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.NextVideoNotificationTemplate), part.Identifier.Title);
                        }
                        else if (next is VideoInformation video)
                        {
                            tip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.NextVideoNotificationTemplate), video.Identifier.Title);
                        }

                        var notification = new PlayerNotification(() => PlayNextVideoCommand.Execute(default), tip, 5);
                        // Player.ShowNotification(notification);
                    }
                }
            }
        }
    }

    public void HandleProgressChanged(double position, double duration)
    {
        _lastPosition = position;
        if (position < duration && position > 1 && (Danmaku?.IsEmpty() ?? false))
        {
            Danmaku?.ResetData(_view?.Information.Identifier.Id, _part?.Identifier.Id);
            Danmaku?.LoadDanmakusCommand.Execute(Convert.ToInt32(duration));
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
    private async Task InitializeDashMediaAsync(VideoPart part)
    {
        try
        {
            var info = await _service.GetVideoPlayDetailAsync(_view.Information.Identifier, Convert.ToInt64(part.Identifier.Id));
            if (_view is null)
            {
                return;
            }

            InitializeDash(info);
            var onlineCount = await _service.GetOnlineViewerAsync(_view.Information.Identifier.Id, part.Identifier.Id, CancellationToken.None);
            OnlineCountText = onlineCount.Text;
        }
        catch (Exception ex)
        {
            if (ex is not TaskCanceledException)
            {
                _logger.LogError(ex, $"尝试获取视频 {_view.Information.Identifier.Id} 的第 {part.Index} 分集时失败.");
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

        if (SettingsToolkit.ReadLocalSetting(SettingNames.PlayWithoutP2P, false))
        {
            if (vSeg?.BackupUrls is not null)
            {
                string[] videoUrls = [vSeg?.BaseUrl, .. vSeg?.BackupUrls];
                videoUrl = Array.Find(videoUrls, p => !AppToolkit.IsP2PUrl(p)) ?? vSeg?.BaseUrl;
            }

            if (aSeg?.BackupUrls is not null)
            {
                string[] audioUrls = [aSeg?.BaseUrl, .. aSeg?.BackupUrls];
                audioUrl = Array.Find(audioUrls, p => !AppToolkit.IsP2PUrl(p)) ?? aSeg?.BaseUrl;
            }
        }

        var isAutoPlay = !isFirstSet;
        if (isFirstSet)
        {
            isAutoPlay = SettingsToolkit.ReadLocalSetting(SettingNames.ShouldAutoPlay, true);
        }

        // 切换清晰度时，如果播放器已经加载了媒体，那么就保持当前的播放进度.
        _initialProgress = _lastPosition;

        SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedVideoQuality, vm.Data.Quality);

        Danmaku.ClearAll();
        _videoUrl = videoUrl;
        _audioUrl = audioUrl;
        RequestReload?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Clean()
    {
        ClearView();
        Subtitle?.ClearAll();
        Danmaku.ClearAll();
    }

    [RelayCommand]
    private void SelectSection(IPlayerSectionDetailViewModel section)
    {
        if (section is null || section == SelectedSection)
        {
            return;
        }

        SelectedSection = section;
        SelectedSection.TryFirstLoadCommand.Execute(default);
    }

    [RelayCommand]
    private async Task ReportProgressAsync(double progress)
    {
        var shouldReport = SettingsToolkit.ReadLocalSetting(SettingNames.ShouldReportProgress, true);
        if (!shouldReport || IsPrivatePlay || _view is null || _view.Information is null || progress == 0)
        {
            return;
        }

        try
        {
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            await _service.ReportVideoProgressAsync(AvId, _part.Identifier.Id, Convert.ToInt32(progress), cancellationToken.Token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试上报播放进度时失败.");
        }
    }

    partial void OnCurrentLoopChanged(VideoLoopType value)
        => InitializeNextVideo();
}
