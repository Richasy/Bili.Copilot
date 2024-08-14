// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Bili.Media;
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
        ILogger<VideoPlayerPageViewModel> logger,
        PlayerViewModel player)
    {
        _service = service;
        _logger = logger;
        Player = player;
    }

    /// <inheritdoc/>
    protected override string GetPageKey()
        => nameof(VideoPlayerPage);

    [RelayCommand]
    private async Task InitializePageAsync(MediaIdentifier video)
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
            var view = await _service.GetVideoPageDetailAsync(video, _pageLoadCancellationTokenSource.Token);
            InitializeView(view);
            InitializeDashMediaCommand.Execute(view.Parts.First());
        }
        catch (Exception ex)
        {
            if (ex is not TaskCanceledException)
            {
                IsPageLoadFailed = true;
                _logger.LogError(ex, $"尝试获取视频 {video.Id} 详情时失败.");
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

    private void InitializeView(VideoPlayerView view)
    {
        _view = view;
        Cover = view.Information.Identifier.Cover.SourceUri;
        Title = view.Information.Identifier.Title;
        IsMyVideo = _view.Information.Publisher.User.Id == this.Get<IBiliTokenResolver>().GetToken().UserId.ToString();
        CalcPlayerHeight();
    }

    private void InitializeDash(DashMediaInformation info)
    {
        _videoSegments = info.Videos;
        _audioSegments = info.Audios;
        Formats = info.Formats.Select(p => new PlayerFormatItemViewModel(p)).ToList();

        // 用户个人视频无需会员即可观看最高画质.
        if (IsMyVideo)
        {
            foreach (var format in Formats)
            {
                format.IsEnabled = true;
            }
        }

        var preferFormatSetting = SettingsToolkit.ReadLocalSetting(SettingNames.PreferQuality, PreferQualityType.Auto);
        var availableFormats = Formats.Where(p => p.IsEnabled).ToList();
        PlayerFormatItemViewModel? selectedFormat = default;
        if (preferFormatSetting == PreferQualityType.Auto)
        {
            var lastSelectedFormat = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedVideoQuality, 0);
            selectedFormat = availableFormats.Find(p => p.Data.Quality == lastSelectedFormat);
        }
        else if (preferFormatSetting == PreferQualityType.FourK)
        {
            selectedFormat = availableFormats.Find(p => p.Data.Quality == 120);
        }
        else if (preferFormatSetting == PreferQualityType.HD)
        {
            selectedFormat = availableFormats.Find(p => p.Data.Quality == 80);
        }

        if (selectedFormat is null)
        {
            var maxQuality = availableFormats.Max(p => p.Data.Quality);
            selectedFormat = availableFormats.Find(p => p.Data.Quality == maxQuality);
        }

        ChangeFormatCommand.Execute(selectedFormat);
    }

    private void ClearView()
    {
        IsPageLoadFailed = false;
        _view = default;
        _videoSegments = default;
        _audioSegments = default;
        Cover = default;
        Title = default;
        IsMyVideo = false;

        Formats = default;
        SelectedFormat = default;
    }

    private void CalcPlayerHeight()
    {
        if (PlayerWidth <= 0 || _view?.AspectRatio is null)
        {
            return;
        }

        PlayerHeight = (double)(PlayerWidth * _view.AspectRatio.Value.Height / _view.AspectRatio.Value.Width);
    }

    partial void OnPlayerWidthChanged(double value)
        => CalcPlayerHeight();
}
