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
        => _playlist = items.Select(p => new Items.WebDavStorageItemViewModel(p)).ToList();

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
        if (_playlist is not null && !_playlist.Any(p => p.Data.Uri == video.Uri))
        {
            _playlist = default;
        }

        Player.IsSeparatorWindowPlayer = IsSeparatorWindowPlayer;
        _current = new Items.WebDavStorageItemViewModel(video);
        Title = _current.Data.DisplayName;
        await LoadPlayerAsync();
    }

    [RelayCommand]
    private async Task CleanAsync()
    {
        _playlist = default;
        await Player.CloseAsync();
    }

    private async Task LoadPlayerAsync()
    {
        if (_current is null)
        {
            return;
        }

        var config = this.Get<WebDavPageViewModel>().GetCurrentConfig();
        var url = AppToolkit.GetWebDavServer(config.Host, config.Port ?? 0, _current.Data.Uri) + _current.Data.Uri;
        Player.InjectWebDavConfig(config);
        await Player.SetPlayDataAsync(url, default, true);
    }
}
