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
/// 直播播放页视图模型.
/// </summary>
public sealed partial class LivePlayerPageViewModel : PlayerPageViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LivePlayerPageViewModel"/> class.
    /// </summary>
    public LivePlayerPageViewModel(
        IPlayerService service,
        IDanmakuService danmakuService,
        IRelationshipService relationshipService,
        ILogger<LivePlayerPageViewModel> logger,
        DanmakuViewModel danmaku)
    {
        _service = service;
        _danmakuService = danmakuService;
        _relationshipService = relationshipService;
        _logger = logger;
        Chat = new LiveChatSectionDetailViewModel(_service, this);
        Danmaku = danmaku;
        Player.IsLive = true;
        Player.SetProgressAction(PlayerProgressChanged);
        Player.SetReloadAction(ReloadFormat);
        Player.SetWindowStateChangeAction(ScrollMessagesToBottom);
    }

    /// <inheritdoc/>
    protected override string GetPageKey()
        => nameof(LivePlayerPage);

    /// <inheritdoc/>
    protected override double GetDefaultNavColumnWidth()
        => 300d;

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
            Player.IsSeparatorWindowPlayer = IsSeparatorWindowPlayer;
            ClearView();
            Danmaku.ResetData();
            _pageLoadCancellationTokenSource = new CancellationTokenSource();
            var view = await _service.GetLivePageDetailAsync(live, _pageLoadCancellationTokenSource.Token);
            InitializeView(view);
            LoadRelationshipCommand.Execute(default);
            ViewInitialized?.Invoke(this, EventArgs.Empty);
            await ChangeFormatAsync(default);
            await Chat.StartAsync(live.Id, DisplayDanmaku, SendDanmakuAsync);
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
            var quality = 0;
            if (vm is not null)
            {
                quality = vm.Data.Quality;
                SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedLiveQuality, vm.Data.Quality);
            }
            else
            {
                quality = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedLiveQuality, 400);
            }

            var isAudioOnly = SettingsToolkit.ReadLocalSetting(SettingNames.IsLiveAudioOnly, false);
            var info = await _service.GetLivePlayDetailAsync(_view.Information.Identifier, quality, isAudioOnly, _playLoadCancellationTokenSource.Token)
                ?? throw new Exception("直播播放信息为空");
            InitializeLiveMedia(info);

            // 我们更偏好 http_hls 的直播源，其格式为 m3u8.
            var preferLine = Lines.FirstOrDefault(p => p.Urls.FirstOrDefault()?.Protocol == "http_hls") ?? Lines.FirstOrDefault();
            await ChangeLineAsync(preferLine);
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

    [RelayCommand]
    private async Task LoadRelationshipAsync()
    {
        var relationship = await _relationshipService.GetRelationshipAsync(_view.Information.User.Id);
        IsFollow = relationship != Richasy.BiliKernel.Models.User.UserRelationStatus.Unknown && relationship != Richasy.BiliKernel.Models.User.UserRelationStatus.Unfollow;
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
        Player.InitializeSmtc(_view.Information.Identifier.Cover.SourceUri.ToString(), Title, UpName);
    }

    [RelayCommand]
    private async Task CleanAsync()
    {
        ClearView();
        await Chat?.CloseAsync();
        await Player?.CloseAsync();
    }

    partial void OnPlayerWidthChanged(double value)
        => CalcPlayerHeight();
}
