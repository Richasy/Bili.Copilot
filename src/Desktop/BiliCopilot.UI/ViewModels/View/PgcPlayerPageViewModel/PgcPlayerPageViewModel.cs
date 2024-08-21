// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading;
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
/// PGC 播放器页面视图模型.
/// </summary>
public sealed partial class PgcPlayerPageViewModel : LayoutPageViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgcPlayerPageViewModel"/> class.
    /// </summary>
    public PgcPlayerPageViewModel(
        IPlayerService service,
        IFavoriteService favoriteService,
        IEntertainmentDiscoveryService discoveryService,
        ILogger<PgcPlayerPageViewModel> logger,
        MpvPlayerViewModel player,
        DanmakuViewModel danmaku,
        SubtitleViewModel subtitle,
        CommentMainViewModel comments)
    {
        _service = service;
        _favoriteService = favoriteService;
        _discoveryService = discoveryService;
        _comments = comments;
        _logger = logger;
        Player = player;
        Player.IsPgc = true;
        Danmaku = danmaku;
        Subtitle = subtitle;
        Player.SetProgressAction(PlayerProgressChanged);
        Player.SetStateAction(PlayerStateChanged);
        Player.SetEndAction(PlayerMediaEnded);
    }

    /// <inheritdoc/>
    protected override string GetPageKey()
        => nameof(PgcPlayerPage);

    /// <inheritdoc/>
    protected override double GetDefaultNavColumnWidth()
        => 360d;

    [RelayCommand]
    private async Task InitializePageAsync(MediaIdentifier identifier)
    {
        if (IsPageLoading)
        {
            CancelPageLoad();
        }

        IsPageLoading = true;
        try
        {
            ClearView();
            _pageLoadCancellationTokenSource = new CancellationTokenSource();
            var id = identifier.Id;
            var isEpisode = id.StartsWith("ep");
            id = id[3..];
            var seasonId = isEpisode ? default : id;
            var episodeId = isEpisode ? id : default;
            var view = await _service.GetPgcPageDetailAsync(seasonId, episodeId, cancellationToken: _pageLoadCancellationTokenSource.Token);
            InitializeView(view);
            var initialEpisode = FindInitialEpisode(episodeId);
            ChangeEpisode(initialEpisode);
            InitializeSections();
            ViewInitialized?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            if (ex is not TaskCanceledException)
            {
                IsPageLoadFailed = true;
                _logger.LogError(ex, $"尝试获取剧集 {identifier.Id} 详情时失败.");
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
    private async Task InitializeDashMediaAsync(EpisodeInformation episode)
    {
        if (IsMediaLoading)
        {
            CancelDashLoad();
        }

        IsMediaLoading = true;
        try
        {
            _dashLoadCancellationTokenSource = new CancellationTokenSource();
            var cid = episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Cid);
            var seasonType = episode.GetExtensionIfNotNull<string>(EpisodeExtensionDataId.SeasonType);
            var info = await _service.GetPgcPlayDetailAsync(cid.ToString(), episode.Identifier.Id, seasonType, _dashLoadCancellationTokenSource.Token);
            InitializeDash(info);
        }
        catch (Exception ex)
        {
            if (ex is not TaskCanceledException)
            {
                IsMediaLoadFailed = true;
                _logger.LogError(ex, $"尝试获取视频 {_view.Information.Identifier.Id} 的第 {episode.Index} 分集时失败.");
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

        // 切换清晰度时，如果播放器已经加载了媒体，那么就保持当前的播放进度.
        if (_initialProgress == 0 && Player.Position != 0)
        {
            _initialProgress = Player.Position;
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedPgcQuality, vm.Data.Quality);
        await Player.SetPlayDataAsync(videoUrl, audioUrl, isAutoPlay, _initialProgress);
        Danmaku?.Redraw();

        // 重置初始进度，避免影响其它视频.
        _initialProgress = 0;
    }

    [RelayCommand]
    private async Task CleanAsync()
    {
        await ReportProgressAsync(Player.Position);
        ClearView();
        Danmaku.ClearAll();
        Subtitle.ClearAll();
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
        if (!shouldReport || _view is null || _episode is null || _view.Information is null || progress == 0 || Player.IsPlayerDataLoading)
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

    partial void OnPlayerWidthChanged(double value)
        => CalcPlayerHeight();
}
