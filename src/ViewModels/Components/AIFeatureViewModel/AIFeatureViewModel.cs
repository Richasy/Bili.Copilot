﻿// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Article;
using Bili.Copilot.Models.Data.Community;
using Bili.Copilot.Models.Data.Player;
using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.Input;
using Windows.ApplicationModel.AppService;
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
        AttachExceptionHandlerToAsyncCommand(
            ShowError,
            SummarizeVideoCommand,
            SummarizeArticleCommand,
            EvaluateVideoCommand);
    }

    /// <summary>
    /// 根据上下文解释词语.
    /// </summary>
    /// <param name="word">需要解释的内容.</param>
    /// <param name="article">文章信息.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <remarks>单纯解释某个词语可能会造成歧义，特别是在尝试解释缩写的时候。所以有必要提供上下文，以便LLM提供更准确的回答.</remarks>
    public async Task ExplainWordAsync(string word, ArticleIdentifier article)
    {
        await InitializeAsync();
        var content = await GetArticleContentAsync(article);
        var paragraphs = content.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        var para = paragraphs.FirstOrDefault(p => p.Contains(word));
        var prompt = GetExplainWordPrompt(word, para);
        var message = string.Format(ResourceToolkit.GetLocalizedString(StringNames.ExplainWordMessage), word);
        await SendMessageAsync(message, prompt);
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

    private async Task ConnectToFantasyCopilotServiceAsync()
    {
        if (_connection != null)
        {
            _connection?.Dispose();
            return;
        }

        var connectingInfo = new AppTipNotification(ResourceToolkit.GetLocalizedString(StringNames.ConnectingFantasyCopilot), InfoType.Information);
        Tips.Add(connectingInfo);
        _connection = new AppServiceConnection();
        _connection.AppServiceName = "com.fantasycopilot.aiservice";

        // 本地调试.
        // _connection.PackageFamilyName = "Richasy.FantasyCopilot_2r5cjeccfggq0";

        // 实际运行.
        _connection.PackageFamilyName = "60520B029E250.36001E4C9CEE6_5aa7k9th7aafp";
        var status = await _connection.OpenAsync();
        if (status != AppServiceConnectionStatus.Success)
        {
            _connection?.Dispose();
            _connection = null;
            throw new Exception(ResourceToolkit.GetLocalizedString(StringNames.FailToConnectFantasyCopilot) + $" {status}");
        }

        var sucInfo = new AppTipNotification(ResourceToolkit.GetLocalizedString(StringNames.FantasyCopilotConnected), InfoType.Success);
        Tips.Add(sucInfo);
    }

    private void ThrowIfCancelled()
    {
        if (_cancellationTokenSource == null)
        {
            return;
        }

        if (_cancellationTokenSource.IsCancellationRequested)
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
                IsWaiting = false;
            }

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

    private async Task<string> GetArticleContentAsync(ArticleIdentifier article)
    {
        ThrowIfCancelled();
        var gettingArticleContent = new AppTipNotification(
                       string.Format(ResourceToolkit.GetLocalizedString(StringNames.GettingArticleContent), article.Title),
                       InfoType.Information);
        Tips.Add(gettingArticleContent);
        var articleContent = await ArticleProvider.GetArticleContentAsync(article.Id);
        var filterArticle = SmartReader.Reader.ParseArticle(ApiConstants.Article.ArticleContent + $"?id={article.Id}", text: articleContent);
        return filterArticle.TextContent;
    }

    private async Task<List<CommentInformation>> GetTopCommentsAsync(string aid)
    {
        var gettingComments = new AppTipNotification(ResourceToolkit.GetLocalizedString(StringNames.GettingComments), InfoType.Information);
        Tips.Add(gettingComments);
        var commentView = await CommunityProvider.Instance.GetCommentsAsync(
            aid,
            Models.Constants.Bili.CommentType.Video,
            Models.Constants.Bili.CommentSortType.Hot,
            true);
        var currentLength = 0;
        var result = new List<CommentInformation>();
        foreach (var item in commentView.Comments)
        {
            // 限制选取的评论长度.
            if (currentLength > 800)
            {
                break;
            }

            result.Add(item);
            currentLength += item.Content.Text.Length;
        }

        var gotHotComments = new AppTipNotification(string.Format(ResourceToolkit.GetLocalizedString(StringNames.GotHotComments), result.Count), InfoType.Success);
        Tips.Add(gotHotComments);

        return result;
    }

    private void ShowError(Exception ex)
    {
        IsWaiting = false;
        var error = new AppTipNotification(ex.Message, InfoType.Error);
        Tips.Add(error);
    }

    private async Task InitializeAsync()
    {
        _cancellationTokenSource = new System.Threading.CancellationTokenSource();
        var connectType = SettingsToolkit.ReadLocalSetting(SettingNames.AIConnectType, AIConnectType.AppService);
        if (connectType == AIConnectType.AppService)
        {
            await ConnectToFantasyCopilotServiceAsync();
        }
        else
        {
            // await LaunchFantasyCopilotAsync();
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        if (_connection != null)
        {
            _connection.Dispose();
            _connection = null;
        }

        IsWaiting = false;
        _cancellationTokenSource?.Cancel();
    }

    [RelayCommand]
    private async Task SummarizeVideoAsync(VideoIdentifier video)
    {
        await InitializeAsync();
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
        var prompt = GetVideoSummaryPrompt(videoContent, info.Information.Identifier.Title, info.Information.Description);
        var message = string.Format(ResourceToolkit.GetLocalizedString(StringNames.SummarizeMessage), info.Information.Identifier.Title);
        await SendMessageAsync(message, prompt, tokens: Math.Min(videoContent.Length, 1000));
    }

    [RelayCommand]
    private async Task SummarizeArticleAsync(ArticleIdentifier article)
    {
        await InitializeAsync();
        var content = await GetArticleContentAsync(article);
        var prompt = GetArticleSummaryPrompt(content, article.Title);
        var message = string.Format(ResourceToolkit.GetLocalizedString(StringNames.SummarizeMessage), article.Title);
        await SendMessageAsync(message, prompt, tokens: Math.Min(content.Length, 1000));
    }

    [RelayCommand]
    private async Task EvaluateVideoAsync(VideoIdentifier video)
    {
        await InitializeAsync();
        var info = await GetVideoInformationAsync(video);
        var aid = info.Information.Identifier.Id;
        var comments = await GetTopCommentsAsync(aid);
        var description = info.Information.Description;
        var title = info.Information.Identifier.Title;
        var duration = TimeSpan.FromSeconds(info.Information.Identifier.Duration);
        var commentContent = string.Join("\n---\n", comments.Select(p => $"<|Comment|>{p.Content.Text}<|Comment|>"));
        var prompt = EvaluateVideoPrompt(title, description, commentContent, duration, info.Information.PublishTime, info.Information.CommunityInformation);
        var message = string.Format(ResourceToolkit.GetLocalizedString(StringNames.EvaluateVideo), title);
        await SendMessageAsync(message, prompt, temperature: 1);
    }
}
