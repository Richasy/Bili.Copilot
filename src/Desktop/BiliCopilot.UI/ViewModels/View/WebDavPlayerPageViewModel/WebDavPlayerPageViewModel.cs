// Copyright (c) Bili Copilot. All rights reserved.

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
        var url = AppToolkit.GetWebDavServer(config.Host, config.Port ?? 0, Current.Data.Uri) + Current.Data.Uri;
        Player.InjectWebDavConfig(config);
        await Player.SetPlayDataAsync(url, default, true);
    }
}
