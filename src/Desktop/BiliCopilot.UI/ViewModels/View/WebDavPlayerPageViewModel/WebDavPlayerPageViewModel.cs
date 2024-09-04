// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using WebDav;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// WebDav 播放页面视图模型.
/// </summary>
public sealed partial class WebDavPlayerPageViewModel : PlayerPageViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavPlayerPageViewModel"/> class.
    /// </summary>
    public WebDavPlayerPageViewModel(
        ILogger<WebDavPlayerPageViewModel> logger)
    {
        _logger = logger;
        Player.IsWebDav = true;
        Player.SetEndAction(PlayerMediaEnded);
    }

    /// <summary>
    /// 注入播放列表.
    /// </summary>
    public void InjectPlaylist(IList<WebDavResource> items)
        => Playlist = items.Select(p => new Items.WebDavStorageItemViewModel(p)).ToList();

    /// <inheritdoc/>
    protected override string GetPageKey()
        => nameof(WebDavPlayerPage);

    /// <inheritdoc/>
    protected override double GetDefaultNavColumnWidth()
        => 360d;

    [RelayCommand]
    private async Task InitializeAsync(WebDavResource video)
    {
        Player.CancelNotification();

        if (Playlist is not null && !Playlist.Any(p => p.Data.Uri == video.Uri))
        {
            Playlist = default;
        }

        Player.IsSeparatorWindowPlayer = IsSeparatorWindowPlayer;
        Current = Playlist.FirstOrDefault(p => p.Data.Uri == video.Uri);
        Title = Current.Data.DisplayName;
        InitializeNextVideo();
        VideoSelectionChanged?.Invoke(this, EventArgs.Empty);
        await LoadPlayerAsync();
    }

    [RelayCommand]
    private async Task CleanAsync()
    {
        Playlist = default;
        await Player.CloseAsync();
    }

    private async Task LoadPlayerAsync()
    {
        if (Current is null)
        {
            return;
        }

        var config = this.Get<WebDavPageViewModel>().GetCurrentConfig();
        var url = GetVideoUrl();
        Player.InjectWebDavConfig(config);
        await Player.SetPlayDataAsync(url, default, true, _initialProgress, Current.Data.ContentType);
        Player.InitializeSmtc(default, Current.Data.DisplayName, Uri.UnescapeDataString(Current.Data.Uri));
        _initialProgress = 0;
    }

    private void InitializeNextVideo()
    {
        var next = FindNextVideo();
        HasNextVideo = next is not null;
        if (HasNextVideo)
        {
            NextVideoTip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.PlayNextVideoTipTemplate), next.Data.DisplayName);
        }
    }

    private void Reload()
    {
        if (Current is null)
        {
            return;
        }

        if (Player.Position > 0)
        {
            _initialProgress = Player.Position;
        }

        InitializeCommand.Execute(Current.Data);
    }
}
