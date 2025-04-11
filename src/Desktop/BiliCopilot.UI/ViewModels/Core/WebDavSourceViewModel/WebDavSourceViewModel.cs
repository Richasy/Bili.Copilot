// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using BiliCopilot.UI.ViewModels.View;
using Microsoft.Extensions.Logging;
using Richasy.MpvKernel.Core.Enums;
using Richasy.MpvKernel.Core.Models;
using Richasy.WinUIKernel.Share.ViewModels;
using WebDav;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// WEB DAV 源视图模型.
/// </summary>
public sealed partial class WebDavSourceViewModel : ViewModelBase, IMediaSourceResolver
{
    public WebDavSourceViewModel(
        ILogger<WebDavSourceViewModel> logger)
    {
        _logger = logger;
    }

    public event EventHandler RequestReload;

    public event EventHandler RequestClear;

    public void InjectPlaylist(WebDavResource video, List<WebDavResource>? items = null)
    {
        _cachedVideo = video;
        if (items is not null)
        {
            Playlist = items.ConvertAll(p => new WebDavStorageItemViewModel(p));
        }
    }

    public async Task InitializeAsync()
    {
        _videoUrl = string.Empty;
        Id = _cachedVideo!.Uri;
        ErrorMessage = string.Empty;

        try
        {
            RequestClear?.Invoke(this, EventArgs.Empty);
            _initialProgress = 0;
            _lastPosition = 0;

            if (Playlist is not null && !Playlist.Any(p => p.Data.Uri == _cachedVideo.Uri))
            {
                Playlist = default;
            }

            Current = Playlist?.FirstOrDefault(p => p.Data.Uri == _cachedVideo.Uri);
            Title = Current?.Data.DisplayName;
            InitializeVideoNavigation();
            await Task.CompletedTask;
            VideoSelectionChanged?.Invoke(this, EventArgs.Empty);
            RequestReload?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            if (ex is not TaskCanceledException)
            {
                ErrorMessage = ex.Message;
                _logger.LogError(ex, $"尝试播放视频 {_cachedVideo.DisplayName} 详情时失败.");
            }
        }
    }

    public (string url, MpvPlayOptions options) GetSource()
    {
        var options = new MpvPlayOptions();
        var headers = new Dictionary<string, string>();
        var config = this.Get<WebDavPageViewModel>().GetCurrentConfig();
        var auth = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{config.UserName}:{config.Password}"))}";
        headers.Add("Authorization", auth);
        options.HttpHeaders = headers;
        options.EnableYtdl = false;
        options.EnableCookies = false;
        options.StartPosition = _initialProgress;

        return (GetVideoUrl(), options);
    }

    public void HandlePlayerStateChanged(MpvPlayerState state)
    {
        if (state == MpvPlayerState.End)
        {
            var autoNext = SettingsToolkit.ReadLocalSetting(SettingNames.AutoPlayNext, true);
            if (!autoNext || !HasNextVideo)
            {
                return;
            }

            var next = FindNextVideo();
            if (next is null)
            {
                return;
            }

            PlayNextVideoCommand.Execute(default);
        }
    }

    public void HandleProgressChanged(double position, double duration)
    {
        _lastPosition = position;
    }

    public void HandleSpeedChanged(double speed)
    {
    }

    public string GetTitle()
        => Title;

    private void InitializeVideoNavigation()
    {
        var next = FindNextVideo();
        var prev = FindPrevVideo();
        HasNextVideo = next is not null;
        HasPrevVideo = prev is not null;
        CanVideoNavigate = HasNextVideo || HasPrevVideo;

        if (next != null)
        {
            NextVideoTip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.PlayNextVideoTipTemplate), next.Data.DisplayName);
        }

        if (prev != null)
        {
            PrevVideoTip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.PlayPrevVideoTipTemplate), prev.Data.DisplayName);
        }
    }
}
