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
/// 直播源视图模型.
/// </summary>
public sealed partial class LiveSourceViewModel : ViewModelBase, IMediaSourceResolver
{
    public LiveSourceViewModel(
        IPlayerService service,
        IDanmakuService danmakuService,
        IRelationshipService relationshipService,
        ILogger<LiveSourceViewModel> logger,
        DanmakuViewModel danmaku)
    {
        _service = service;
        _danmakuService = danmakuService;
        _relationshipService = relationshipService;
        _logger = logger;
        Chat = new LiveChatSectionDetailViewModel(_service, this);
        Danmaku = danmaku;
    }

    public event EventHandler RequestReload;

    public event EventHandler RequestClear;

    public void InjectMedia(MediaIdentifier identifier)
        => _cachedMedia = identifier;

    public async Task InitializeAsync()
    {
        _liveUrl = string.Empty;
        Id = _cachedMedia?.Id ?? string.Empty;
        ErrorMessage = string.Empty;
        try
        {
            RequestClear?.Invoke(this, EventArgs.Empty);
            ClearView();
            Danmaku.ResetData();
            var view = await _service.GetLivePageDetailAsync(_cachedMedia!.Value);
            InitializeView(view);
            LoadRelationshipCommand.Execute(default);
            await ChangeFormatAsync(default);
            await Chat.StartAsync(_cachedMedia!.Value.Id, DisplayDanmaku, SendDanmakuAsync);
        }
        catch (Exception ex)
        {
            if (ex is not TaskCanceledException)
            {
                ErrorMessage = ex.Message;
                _logger.LogError(ex, $"尝试获取直播 {_cachedMedia?.Id} 详情时失败.");
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
        var referer = LiveReferer;
        var userAgent = LiveUserAgent;
        headers.Add("Cookie", cookies);
        headers.Add("Referer", referer);
        options.HttpHeaders = headers;
        options.UserAgent = userAgent;
        options.EnableCookies = true;
        options.EnableYtdl = false;

        return (_liveUrl, options);
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
        else if (state == MpvPlayerState.Paused)
        {
            Danmaku?.Pause();
        }
    }

    public void HandleProgressChanged(double position, double duration)
    {
        Duration = Convert.ToInt32((DateTimeOffset.Now - StartTime).TotalSeconds);
    }

    public void HandleSpeedChanged(double speed)
    {
        if (Danmaku is not null)
        {
            Danmaku.ExtraSpeed = speed;
        }
    }

    [RelayCommand]
    private async Task ChangeFormatAsync(PlayerFormatItemViewModel? vm)
    {
        try
        {
            Lines = default;
            SelectedLine = default;
            Formats = default;
            SelectedFormat = default;
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
            var info = await _service.GetLivePlayDetailAsync(_view.Information.Identifier, quality, isAudioOnly)
                ?? throw new Exception("直播播放信息为空");
            InitializeLiveMedia(info);

            // 我们更偏好 http_hls 的直播源，其格式为 m3u8.
            var preferLine = Lines.Find(p => p.Urls.FirstOrDefault()?.Protocol == "http_hls") ?? Lines.FirstOrDefault();
            ChangeLine(preferLine);
        }
        catch (Exception ex)
        {
            if (ex is not TaskCanceledException)
            {
                ErrorMessage = ex.Message;
                _logger.LogError(ex, $"尝试获取直播 {_view.Information.Identifier.Id} 的播放详情时失败.");
            }
        }
    }

    [RelayCommand]
    private async Task LoadRelationshipAsync()
    {
        var relationship = await _relationshipService.GetRelationshipAsync(_view.Information.User.Id);
        IsFollow = relationship != Richasy.BiliKernel.Models.User.UserRelationStatus.Unknown && relationship != Richasy.BiliKernel.Models.User.UserRelationStatus.Unfollow;
    }

    private void ChangeLine(LiveLineInformation line)
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

        _liveUrl = url;
        RequestReload?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task CleanAsync()
    {
        ClearView();
        await Chat?.CloseAsync();
    }
}
