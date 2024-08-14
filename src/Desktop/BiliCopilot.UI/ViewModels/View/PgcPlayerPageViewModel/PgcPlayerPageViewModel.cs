// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// PGC 播放器页面视图模型.
/// </summary>
public sealed partial class PgcPlayerPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgcPlayerPageViewModel"/> class.
    /// </summary>
    public PgcPlayerPageViewModel(
        IPlayerService service,
        ILogger<PgcPlayerPageViewModel> logger,
        PlayerViewModel player)
    {
        _service = service;
        _logger = logger;
        Player = player;
        Player.IsPgc = true;
    }

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
            if (initialEpisode is null)
            {
                // show error message.
            }
            else
            {
                InitializeDashMediaCommand.Execute(initialEpisode);
            }
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

    private void InitializeView(PgcPlayerView view)
    {
        _view = view;
        Cover = view.Information.Identifier.Cover.SourceUri;
        Title = view.Information.Identifier.Title;
    }

    private void InitializeDash(DashMediaInformation info)
    {
        _videoSegments = info.Videos;
        _audioSegments = info.Audios;
        Formats = info.Formats.Select(p => new PlayerFormatItemViewModel(p)).ToList();

        var preferFormatSetting = SettingsToolkit.ReadLocalSetting(SettingNames.PreferQuality, PreferQualityType.Auto);
        var availableFormats = Formats.Where(p => p.IsEnabled).ToList();
        PlayerFormatItemViewModel? selectedFormat = default;
        if (preferFormatSetting == PreferQualityType.Auto)
        {
            var lastSelectedFormat = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedPgcQuality, 0);
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

    private EpisodeInformation? FindInitialEpisode(string? initialEpisodeId)
    {
        EpisodeInformation? playEpisode = default;
        if (!string.IsNullOrEmpty(initialEpisodeId))
        {
            playEpisode = _view.Episodes.FirstOrDefault(p => p.Identifier.Id == initialEpisodeId);
        }

        if (playEpisode == null)
        {
            var historyEpisodeId = _view.Progress?.Cid;
            if (!string.IsNullOrEmpty(historyEpisodeId))
            {
                playEpisode = _view.Episodes.FirstOrDefault(p => p.Identifier.Id == historyEpisodeId);
            }
        }

        return playEpisode ?? _view.Episodes.FirstOrDefault();
    }

    private void ClearView()
    {
        IsPageLoadFailed = false;
        _view = default;
        _videoSegments = default;
        _audioSegments = default;
        Cover = default;
        Title = default;

        Formats = default;
        SelectedFormat = default;
    }
}
