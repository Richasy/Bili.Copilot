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
/// 视频播放器页面视图模型.
/// </summary>
public sealed partial class VideoPlayerPageViewModel : LayoutPageViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPlayerPageViewModel"/> class.
    /// </summary>
    public VideoPlayerPageViewModel(
        IPlayerService service,
        IRelationshipService relationshipService,
        IFavoriteService favoriteService,
        ILogger<VideoPlayerPageViewModel> logger,
        PlayerViewModel player,
        CommentMainViewModel comments)
    {
        _service = service;
        _relationshipService = relationshipService;
        _favoriteService = favoriteService;
        _logger = logger;
        _comments = comments;
        Player = player;
    }

    /// <summary>
    /// 注入播放列表.
    /// </summary>
    public void InjectPlaylist(IList<VideoInformation> playlist)
        => _playlist = playlist;

    /// <inheritdoc/>
    protected override string GetPageKey()
        => nameof(VideoPlayerPage);

    [RelayCommand]
    private async Task InitializePageAsync(VideoInformation video)
    {
        if (IsPageLoading)
        {
            CancelPageLoad();
        }

        IsPageLoading = true;
        try
        {
            ClearView();
            if (_playlist is not null && !_playlist.Any(p => p.Identifier.Id == video.Identifier.Id))
            {
                _playlist = default;
            }

            _pageLoadCancellationTokenSource = new CancellationTokenSource();
            var view = await _service.GetVideoPageDetailAsync(video.Identifier, _pageLoadCancellationTokenSource.Token);
            InitializeView(view);
            var initialPart = FindInitialPart(default);
            ChangePart(initialPart);
            InitializeSections();
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
        var isFirstSet = SelectedFormat == default;
        SelectedFormat = vm;
        var maxAudioQuality = _audioSegments?.Max(p => Convert.ToInt32(p.Id));
        var vSeg = _videoSegments?.FirstOrDefault(p => p.Id == vm.Data.Quality.ToString());
        var aSeg = _audioSegments?.FirstOrDefault(p => p.Id == maxAudioQuality.ToString());
        var videoUrl = vSeg?.BaseUrl;
        var audioUrl = aSeg?.BaseUrl;

        var isAutoPlay = !isFirstSet;
        if (isFirstSet)
        {
            isAutoPlay = SettingsToolkit.ReadLocalSetting(SettingNames.ShouldAutoPlay, true);
        }

        await Player.SetPlayDataAsync(videoUrl, audioUrl, isAutoPlay);
    }

    [RelayCommand]
    private async Task CleanAsync()
    {
        ClearView();
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

    partial void OnPlayerWidthChanged(double value)
        => CalcPlayerHeight();
}
