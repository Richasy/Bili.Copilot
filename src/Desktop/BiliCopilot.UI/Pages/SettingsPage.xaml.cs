// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using Microsoft.UI.Xaml.Navigation;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.Pages;

/// <summary>
/// 设置页面.
/// </summary>
public sealed partial class SettingsPage : Page
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPage"/> class.
    /// </summary>
    public SettingsPage()
    {
        InitializeComponent();
    }

    /// <inheritdoc/>
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is VideoInformation video)
        {
            await LoadOnlineVideoAsync(video);
        }
    }

    private async void OnButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var file = await FileToolkit.PickFileAsync(".mp4", this.Get<AppViewModel>().ActivatedWindow);
        if (file is not null)
        {
            await MpvPlayer.OpenAsync(file);
        }
    }

    private async Task LoadOnlineVideoAsync(VideoInformation info)
    {
        var pageView = await this.Get<IPlayerService>().GetVideoPageDetailAsync(info.Identifier);
        var cid = pageView.Parts.First().Identifier.Id;
        var mediaInfo = await this.Get<IPlayerService>().GetVideoPlayDetailAsync(info.Identifier, long.Parse(cid));

        var cookies = this.Get<IBiliCookiesResolver>().GetCookieString();
        var httpParams = $"--cookies --user-agent=\\\"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36 Edg/116.0.1938.69\\\" --http-header-fields=\\\"Cookie: {cookies}\\\" --http-header-fields=\\\"Referer: https://www.bilibili.com\\\"";

        var videoUrl = mediaInfo.Videos.First().BaseUrl;
        var audioUrl = mediaInfo.Audios.First().BaseUrl;

        var command = $"mpvnet {httpParams} --title=\\\"{info.Identifier.Title}\\\" \\\"{videoUrl}\\\"";
        if (!string.IsNullOrEmpty(audioUrl))
        {
            command += $" --audio-file=\\\"{audioUrl}\\\"";
        }

        // await Task.Run(() =>
        // {
        //     var startInfo = new ProcessStartInfo("powershell.exe", $"-Command \"{command}\"");
        //     var process = Process.Start(startInfo);
        // });
        await MpvPlayer.OpenAsync(mediaInfo);
    }
}
