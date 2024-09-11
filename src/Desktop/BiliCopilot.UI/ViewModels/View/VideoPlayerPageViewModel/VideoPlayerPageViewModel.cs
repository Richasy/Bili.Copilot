// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 视频播放器页面视图模型.
/// </summary>
public sealed partial class VideoPlayerPageViewModel : PlayerPageViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPlayerPageViewModel"/> class.
    /// </summary>
    public VideoPlayerPageViewModel(
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
        Player.SetProgressAction(PlayerProgressChanged);
        Player.SetStateAction(PlayerStateChanged);
        Player.SetEndAction(PlayerMediaEnded);
        Player.SetReloadAction(ReloadPart);
        Player.SetSpeedAction(PlayerSpeedChanged);
    }

    /// <summary>
    /// 注入播放列表.
    /// </summary>
    public void InjectPlaylist(IList<VideoInformation> playlist)
        => _playlist = playlist;

    /// <inheritdoc/>
    protected override string GetPageKey()
        => nameof(VideoPlayerPage);

    /// <inheritdoc/>
    protected override double GetDefaultNavColumnWidth()
        => 360d;

    [RelayCommand]
    private async Task InitializePageAsync(VideoSnapshot snapshot)
    {
        if (IsPageLoading)
        {
            CancelPageLoad();
        }

        IsPageLoading = true;
        var video = snapshot.Video;
        IsPrivatePlay = snapshot.IsPrivate;
        try
        {
            if (_view is not null && _view.Information.Identifier.Id != video.Identifier.Id)
            {
                // 记录上一个视频的播放进度.
                await ReportProgressAsync(Player.Position);
            }

            ClearView();
            Player.CancelNotification();
            if (_playlist is not null && !_playlist.Any(p => p.Identifier.Id == video.Identifier.Id))
            {
                _playlist = default;
            }

            Player.IsSeparatorWindowPlayer = IsSeparatorWindowPlayer;
            _pageLoadCancellationTokenSource = new CancellationTokenSource();
            var view = await _service.GetVideoPageDetailAsync(video.Identifier, _pageLoadCancellationTokenSource.Token);
            InitializeView(view);
            var initialPart = FindInitialPart(default) ?? throw new Exception("无法找到视频的分集信息.");
            _part = initialPart;
            LoadInitialProgress();
            InitializeSections();
            ChangePart(initialPart);
            ViewInitialized?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            if (ex is not TaskCanceledException)
            {
                IsPageLoadFailed = true;
                _logger.LogError(ex, $"尝试获取视频 {video.Identifier.Id} 详情时失败.");
            }
            else
            {
                return;
            }
        }
        finally
        {
            IsPageLoading = false;
        }
    }

    [RelayCommand]
    private async Task InitializeDashMediaAsync(VideoPart part)
    {
        if (IsMediaLoading)
        {
            CancelDashLoad();
        }

        IsMediaLoading = true;
        try
        {
            _dashLoadCancellationTokenSource = new CancellationTokenSource();
            var info = await _service.GetVideoPlayDetailAsync(_view.Information.Identifier, Convert.ToInt64(part.Identifier.Id), _dashLoadCancellationTokenSource.Token);
            InitializeDash(info);
            var onlineCount = await _service.GetOnlineViewerAsync(_view.Information.Identifier.Id, part.Identifier.Id, _dashLoadCancellationTokenSource.Token);
            OnlineCountText = onlineCount.Text;
        }
        catch (Exception ex)
        {
            if (ex is not TaskCanceledException)
            {
                IsMediaLoadFailed = true;
                _logger.LogError(ex, $"尝试获取视频 {_view.Information.Identifier.Id} 的第 {part.Index} 分集时失败.");
            }
            else
            {
                return;
            }
        }
        finally
        {
            IsMediaLoading = false;
        }
    }

    [RelayCommand]
    private void CancelPageLoad()
    {
        if (_pageLoadCancellationTokenSource is not null)
        {
            _pageLoadCancellationTokenSource.Cancel();
            _pageLoadCancellationTokenSource.Dispose();
            _pageLoadCancellationTokenSource = null;
            IsPageLoading = false;
        }
    }

    [RelayCommand]
    private void CancelDashLoad()
    {
        if (_dashLoadCancellationTokenSource is not null)
        {
            _dashLoadCancellationTokenSource.Cancel();
            _dashLoadCancellationTokenSource.Dispose();
            _dashLoadCancellationTokenSource = null;
            IsMediaLoading = false;
        }
    }

    [RelayCommand]
    private async Task ChangeFormatAsync(PlayerFormatItemViewModel vm)
    {
        if (vm is null || vm == SelectedFormat)
        {
            return;
        }

        _isFormatChanging = true;
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
        if (_initialProgress == 0 && Player.Position != 0)
        {
            _initialProgress = Player.Position;
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedVideoQuality, vm.Data.Quality);
        if (Player is NativePlayerViewModel npvm)
        {
            npvm.InjectSegments(vSeg, aSeg);
        }

        await Player.SetPlayDataAsync(videoUrl, audioUrl, isAutoPlay, _initialProgress < 0 ? 0 : _initialProgress, extraOptions: _part.Identifier.Id);
        Player.InitializeSmtc(_view.Information.Identifier.Cover.SourceUri.ToString(), Title, UpName);
        Danmaku?.ClearAll();
        Danmaku?.ResetData(_view.Information.Identifier.Id, _part.Identifier.Id);

        // 重置初始进度，避免影响其它视频.
        _initialProgress = 0;
        _isFormatChanging = false;
    }

    [RelayCommand]
    private async Task CleanAsync()
    {
        await ReportProgressAsync(Player.Position);
        ClearView();
        Subtitle?.ClearAll();
        Danmaku.ClearAll();
        await Player?.CloseAsync();
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
    private async Task ReportProgressAsync(int progress)
    {
        var shouldReport = SettingsToolkit.ReadLocalSetting(SettingNames.ShouldReportProgress, true);
        if (!shouldReport || IsPrivatePlay || _view is null || _view.Information is null || progress == 0 || Player.IsPlayerDataLoading)
        {
            return;
        }

        try
        {
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            await _service.ReportVideoProgressAsync(AvId, _part.Identifier.Id, progress, cancellationToken.Token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试上报播放进度时失败.");
        }
    }

    partial void OnPlayerWidthChanged(double value)
        => CalcPlayerHeight();
}
