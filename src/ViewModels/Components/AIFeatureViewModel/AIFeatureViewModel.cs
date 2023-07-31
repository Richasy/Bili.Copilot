// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Player;
using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.Input;
using Windows.System;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// AI 功能视图模型.
/// </summary>
public sealed partial class AIFeatureViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AIFeatureViewModel"/> class.
    /// </summary>
    public AIFeatureViewModel()
    {
        Tips = new ObservableCollection<AppTipNotification>();
        AttachExceptionHandlerToAsyncCommand(ShowError, SummarizeVideoCommand);
    }

    private static bool IsFantasyCopilotRunning()
    {
        var process = Process.GetProcessesByName("App")
            .FirstOrDefault(p => p.MainWindowTitle == "Fantasy Copilot" || p.MainWindowTitle == "小幻助理");
        return process != null;
    }

    private async Task LaunchFantasyCopilotAsync()
    {
        var launchingInfo = new AppTipNotification(ResourceToolkit.GetLocalizedString(StringNames.LaunchingFantasyCopilot), InfoType.Information);
        Tips.Add(launchingInfo);
        bool isLaunched;
        do
        {
            ThrowIfCancelled();
            isLaunched = IsFantasyCopilotRunning();
            if (!isLaunched)
            {
                if (!_isTryLaunched)
                {
                    await Launcher.LaunchUriAsync(new Uri("fancop://"));
                    _isTryLaunched = true;
                }

                await Task.Delay(1000);
            }
        }
        while (!isLaunched);

        var launchedInfo = new AppTipNotification(ResourceToolkit.GetLocalizedString(StringNames.LaunchedFantasyCopilot), InfoType.Success);
        Tips.Add(launchedInfo);
    }

    private void ThrowIfCancelled()
    {
        if (_cancellationTokenSource == null)
        {
            return;
        }

        if (_cancellationTokenSource.IsCancellationRequested)
        {
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
            throw new TaskCanceledException("Task cancelled");
        }
    }

    private async Task<VideoPlayerView> GetVideoInformationAsync(VideoIdentifier info)
    {
        ThrowIfCancelled();
        var gettingInfo = new AppTipNotification(ResourceToolkit.GetLocalizedString(StringNames.GettingVideoInformation), InfoType.Information);
        Tips.Add(gettingInfo);
        var videoInfo = await PlayerProvider.GetVideoDetailAsync(info.Id);
        return videoInfo;
    }

    private async Task<IEnumerable<SubtitleMeta>> GetSubtitlesAsync(string aid, string cid)
    {
        ThrowIfCancelled();
        var gettingSubtitles = new AppTipNotification(ResourceToolkit.GetLocalizedString(StringNames.GettingSubtitles), InfoType.Information);
        Tips.Add(gettingSubtitles);
        var subtitles = await PlayerProvider.GetSubtitleIndexAsync(aid, cid);
        return subtitles;
    }

    private async Task<IEnumerable<SubtitleInformation>> GetSubtitleDetailAsync(SubtitleMeta subtitle)
    {
        ThrowIfCancelled();
        var gettingSubtitleDetail = new AppTipNotification(
            string.Format(ResourceToolkit.GetLocalizedString(StringNames.GettingSubtitleDetail), subtitle.LanguageName),
            InfoType.Information);
        Tips.Add(gettingSubtitleDetail);
        var subtitleDetail = await PlayerProvider.GetSubtitleDetailAsync(subtitle.Url);
        return subtitleDetail;
    }

    private void ShowError(Exception ex)
    {
        var error = new AppTipNotification(ex.Message, InfoType.Error);
        Tips.Add(error);
    }

    [RelayCommand]
    private void Cancel()
        => _cancellationTokenSource?.Cancel();

    [RelayCommand]
    private async Task SummarizeVideoAsync(VideoIdentifier video)
    {
        _cancellationTokenSource = new System.Threading.CancellationTokenSource();
        await LaunchFantasyCopilotAsync();
        var info = await GetVideoInformationAsync(video);
        var aid = info.Information.Identifier.Id;
        var partVideo = info.SubVideos.First();
        var cid = partVideo.Id;
        var videoPartSelected = new AppTipNotification(
            string.Format(ResourceToolkit.GetLocalizedString(StringNames.VideoPartSelected), partVideo.Title),
            InfoType.Success);
        Tips.Add(videoPartSelected);
        var subtitles = await GetSubtitlesAsync(aid, cid);
        if (!subtitles.Any())
        {
            var noSubtitle = new AppTipNotification(ResourceToolkit.GetLocalizedString(StringNames.NoSubtitle), InfoType.Warning);
            Tips.Add(noSubtitle);
            return;
        }

        var subtitle = subtitles.First();
        var subtitleSelected = new AppTipNotification(
                       string.Format(ResourceToolkit.GetLocalizedString(StringNames.SubtitleSelected), subtitle.LanguageName),
                       InfoType.Success);
        Tips.Add(subtitleSelected);
        var subtitleDetail = await GetSubtitleDetailAsync(subtitle);
        var sendingMessage = new AppTipNotification(ResourceToolkit.GetLocalizedString(StringNames.SendingMessage), InfoType.Information);
        Tips.Add(sendingMessage);

        var videoContent = string.Join("\n", subtitleDetail.Select(p => $"{p.Content}"));
        var language = subtitle.LanguageName;
        var prompt = GetVideoSummaryPrompt(videoContent, info.Information.Identifier.Title, info.Information.Description, language);
        var message = string.Format(ResourceToolkit.GetLocalizedString(StringNames.SummarizeMessage), info.Information.Identifier.Title);
        await SendMessageAsync(message, prompt, tokens: Math.Min(videoContent.Length, 1000));
    }
}
