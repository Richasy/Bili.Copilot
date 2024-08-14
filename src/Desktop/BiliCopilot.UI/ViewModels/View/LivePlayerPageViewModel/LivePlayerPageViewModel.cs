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
/// 直播播放页视图模型.
/// </summary>
public sealed partial class LivePlayerPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LivePlayerPageViewModel"/> class.
    /// </summary>
    public LivePlayerPageViewModel(
        IPlayerService service,
        ILogger<LivePlayerPageViewModel> logger,
        PlayerViewModel player)
    {
        _service = service;
        _logger = logger;
        Player = player;
        Player.IsLive = true;
    }

    [RelayCommand]
    private async Task InitializePageAsync(MediaIdentifier live)
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
            var view = await _service.GetLivePageDetailAsync(live, _pageLoadCancellationTokenSource.Token);
            InitializeView(view);
            await ChangeFormatAsync(default);
        }
        catch (Exception ex)
        {
            if (ex is not TaskCanceledException)
            {
                IsPageLoadFailed = true;
                _logger.LogError(ex, $"尝试获取直播 {live.Id} 详情时失败.");
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
        if (_playLoadCancellationTokenSource is not null)
        {
            _playLoadCancellationTokenSource.Cancel();
            _playLoadCancellationTokenSource.Dispose();
            _playLoadCancellationTokenSource = null;
            IsMediaLoading = false;
        }
    }

    [RelayCommand]
    private async Task ChangeFormatAsync(PlayerFormatItemViewModel? vm)
    {
        try
        {
            IsMediaLoading = true;
            Lines = default;
            SelectedLine = default;
            Formats = default;
            SelectedFormat = default;
            _playLoadCancellationTokenSource = new CancellationTokenSource();
            var isAudioOnly = SettingsToolkit.ReadLocalSetting(SettingNames.IsLiveAudioOnly, false);
            var info = await _service.GetLivePlayDetailAsync(_view.Information.Identifier, vm?.Data.Quality ?? SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedLiveQuality, 400), isAudioOnly, _playLoadCancellationTokenSource.Token);
            InitializeLiveMedia(info);
            await ChangeLineAsync(Lines.FirstOrDefault());
        }
        catch (Exception ex)
        {
            if (ex is not TaskCanceledException)
            {
                _logger.LogError(ex, $"尝试获取直播 {_view.Information.Identifier.Id} 的播放详情时失败.");
                IsMediaLoadFailed = true;
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

    private async Task ChangeLineAsync(LiveLineInformation line)
    {
        if (line is null || line == SelectedLine)
        {
            return;
        }

        var isFirstSet = SelectedLine is null;
        SelectedLine = line;
        var url = line.Urls.First().ToString();
        var autoPlay = SettingsToolkit.ReadLocalSetting(SettingNames.ShouldAutoPlay, true);
        if (!isFirstSet)
        {
            autoPlay = true;
        }

        await Player.SetPlayDataAsync(url, default, autoPlay);
    }

    [RelayCommand]
    private async Task CleanAsync()
    {
        ClearView();
        await Player?.CloseAsync();
    }

    private void InitializeView(LivePlayerView view)
    {
        _view = view;
        Cover = view.Information.Identifier.Cover.SourceUri;
        Title = view.Information.Identifier.Title;
    }

    private void InitializeLiveMedia(LiveMediaInformation info)
    {
        Formats = info.Formats.Select(p => new PlayerFormatItemViewModel(p)).ToList();
        var lines = info.Lines.ToList()
            .Where(p => p.Urls.FirstOrDefault()?.Protocol == "http_hls" || p.Urls.FirstOrDefault()?.Protocol == "http_stream")
            .ToList();
        Lines = lines;

        var currentQuality = info.Lines.FirstOrDefault().Quality;
        var format = Formats.FirstOrDefault(p => p.Data.Quality == currentQuality);
        SelectedFormat = format ?? Formats.FirstOrDefault();
    }

    private void ClearView()
    {
        IsPageLoadFailed = false;
        _view = default;
        Cover = default;
        Title = default;

        Formats = default;
        SelectedFormat = default;
        Lines = default;
        SelectedLine = default;
    }
}
