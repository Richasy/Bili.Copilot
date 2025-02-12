// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Richasy.AgentKernel.Chat;
using Richasy.AgentKernel;
using Richasy.BiliKernel.Bili.Comment;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUIKernel.Share.Base;
using RichasyKernel;
using System.Text.RegularExpressions;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// AI 视图模型.
/// </summary>
public sealed partial class AIViewModel
{
    private async Task SummaryVideoAsync(AIQuickItemViewModel source)
    {
        try
        {
            Erase();
            InitOtherPrompts(source);
            RequestText = string.Format(source.RequestTemplate, _videoView.Information.Identifier.Title);
            var promptTemplate = source.Prompt;
            IsGenerating = true;
            _generateCancellationTokenSource = new CancellationTokenSource();
            var subtitleText = await GetVideoSubtitleAsync();
            subtitleText ??= "无字幕";

            // 生成总结.
            ProgressTip = ResourceToolkit.GetLocalizedString(StringNames.Summarizing);
            var prompt = promptTemplate
                .Replace("{title}", _videoView.Information.Identifier.Title)
                .Replace("{description}", _videoView.Information.GetExtensionIfNotNull<string>(VideoExtensionDataId.Description))
                .Replace("{subtitle}", subtitleText);
            await SendMessageAsync(prompt);
            ProgressTip = default;
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "总结视频内容失败");
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsGenerating = false;
            ProgressTip = default;
        }
    }

    private async Task EvaluateVideoAsync(AIQuickItemViewModel source)
    {
        try
        {
            Erase();
            InitOtherPrompts(source);
            RequestText = string.Format(source.RequestTemplate, _videoView.Information.Identifier.Title);
            var promptTemplate = source.Prompt;
            IsGenerating = true;
            _generateCancellationTokenSource = new CancellationTokenSource();
            var subtitleText = await GetVideoSubtitleAsync();
            subtitleText ??= "无字幕";
            var commentText = await GetHotCommentTextAsync(_videoView.Information.Identifier.Id, CommentTargetType.Video);
            commentText ??= "无评论";

            // 生成总结.
            ProgressTip = ResourceToolkit.GetLocalizedString(StringNames.Generating);
            var prompt = promptTemplate
                .Replace("{title}", _videoView.Information.Identifier.Title)
                .Replace("{description}", _videoView.Information.GetExtensionIfNotNull<string>(VideoExtensionDataId.Description))
                .Replace("{subtitle}", subtitleText)
                .Replace("{comments}", commentText);
            await SendMessageAsync(prompt);
            ProgressTip = default;
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "评价视频内容失败");
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsGenerating = false;
            ProgressTip = default;
        }
    }

    private async Task AskVideoQuestionAsync(string question)
    {
        if (string.IsNullOrEmpty(question))
        {
            return;
        }

        try
        {
            Erase();
            InitOtherPrompts(default);
            RequestText = question;
            var promptTemplate = PromptConstants.VideoQuestionPrompt;
            IsGenerating = true;
            _generateCancellationTokenSource = new CancellationTokenSource();
            var subtitleText = await GetVideoSubtitleAsync();
            subtitleText ??= "无字幕";

            // 生成总结.
            ProgressTip = ResourceToolkit.GetLocalizedString(StringNames.Generating);
            var prompt = promptTemplate
                .Replace("{title}", _videoView.Information.Identifier.Title)
                .Replace("{description}", _videoView.Information.GetExtensionIfNotNull<string>(VideoExtensionDataId.Description))
                .Replace("{subtitle}", subtitleText)
                .Replace("{question}", question);
            await SendMessageAsync(prompt);
            ProgressTip = default;
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "回答问题失败");
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsGenerating = false;
            ProgressTip = default;
        }
    }

    private async Task SummaryArticleAsync(AIQuickItemViewModel source)
    {
        try
        {
            Erase();
            InitOtherPrompts(source);
            RequestText = string.Format(source.RequestTemplate, _articleDetail.Identifier.Title);
            var promptTemplate = source.Prompt;
            IsGenerating = true;
            _generateCancellationTokenSource = new CancellationTokenSource();
            var articleContent = GetArticlePlainText();

            // 生成总结.
            ProgressTip = ResourceToolkit.GetLocalizedString(StringNames.Summarizing);
            var prompt = promptTemplate
                .Replace("{title}", _articleDetail.Identifier.Title)
                .Replace("{content}", articleContent);
            await SendMessageAsync(prompt);
            ProgressTip = default;
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "总结文章内容失败");
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsGenerating = false;
            ProgressTip = default;
        }
    }

    private async Task EvaluateArticleAsync(AIQuickItemViewModel source)
    {
        try
        {
            Erase();
            InitOtherPrompts(source);
            RequestText = string.Format(source.RequestTemplate, _articleDetail.Identifier.Title);
            var promptTemplate = source.Prompt;
            IsGenerating = true;
            _generateCancellationTokenSource = new CancellationTokenSource();
            var articleContent = GetArticlePlainText();

            var commentText = await GetHotCommentTextAsync(_articleDetail.Identifier.Id, CommentTargetType.Article);
            commentText ??= "无评论";

            // 生成总结.
            ProgressTip = ResourceToolkit.GetLocalizedString(StringNames.Generating);
            var prompt = promptTemplate
                .Replace("{title}", _articleDetail.Identifier.Title)
                .Replace("{content}", articleContent)
                .Replace("{comments}", commentText);
            await SendMessageAsync(prompt);
            ProgressTip = default;
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "评价文章内容失败");
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsGenerating = false;
            ProgressTip = default;
        }
    }

    private async Task AskArticleQuestionAsync(string question)
    {
        if (string.IsNullOrEmpty(question))
        {
            return;
        }

        try
        {
            Erase();
            InitOtherPrompts(default);
            RequestText = question;
            var promptTemplate = PromptConstants.ArticleQuestionPrompt;
            IsGenerating = true;
            _generateCancellationTokenSource = new CancellationTokenSource();
            var articleContent = GetArticlePlainText();

            // 生成总结.
            ProgressTip = ResourceToolkit.GetLocalizedString(StringNames.Generating);
            var prompt = promptTemplate
                .Replace("{title}", _articleDetail.Identifier.Title)
                .Replace("{content}", articleContent)
                .Replace("{question}", question);
            await SendMessageAsync(prompt);
            ProgressTip = default;
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "回答问题失败");
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsGenerating = false;
            ProgressTip = default;
        }
    }

    private async Task<string?> GetVideoSubtitleAsync()
    {
        if (_subtitles is null)
        {
            var service = this.Get<ISubtitleService>();

            // 1. 先获取字幕元数据.
            ProgressTip = ResourceToolkit.GetLocalizedString(StringNames.GettingSubtitleMeta);
            var metas = await service.GetSubtitleMetasAsync(_videoView.Information.Identifier.Id, _videoPart.Identifier.Id, _generateCancellationTokenSource.Token);
            if (metas is null || metas.Count == 0)
            {
                ErrorMessage = ResourceToolkit.GetLocalizedString(StringNames.NoSubtitleAvailable);
                return default;
            }

            // 2. 选择字幕元数据.
            ProgressTip = ResourceToolkit.GetLocalizedString(StringNames.GettingSubtitleInformation);
            var meta = metas.FirstOrDefault(p => !p.IsAI) ?? metas.First();
            if (_generateCancellationTokenSource?.IsCancellationRequested != false)
            {
                return default;
            }

            _subtitles = [.. await service.GetSubtitleDetailAsync(meta, _generateCancellationTokenSource.Token)];
        }

        return string.Join(Environment.NewLine, _subtitles.Select(p => $"{AppToolkit.FormatDuration(TimeSpan.FromSeconds(p.StartPosition))} - {AppToolkit.FormatDuration(TimeSpan.FromSeconds(p.EndPosition))}: {p.Content}"));
    }

    private async Task<string?> GetHotCommentTextAsync(string id, CommentTargetType target)
    {
        // 获取评论.
        ProgressTip = ResourceToolkit.GetLocalizedString(StringNames.GettingHotComments);
        var commentService = this.Get<ICommentService>();
        var comments = await commentService.GetCommentsAsync(id, target, CommentSortType.Hot, cancellationToken: _generateCancellationTokenSource.Token);
        if (_generateCancellationTokenSource?.IsCancellationRequested != false)
        {
            return default;
        }

        var commentText = string.Empty;
        if (comments?.Comments.Count > 0)
        {
            foreach (var item in comments.Comments.Select(p => p.Content.Text + "\n------"))
            {
                if (commentText.Length + item.Length > 800)
                {
                    break;
                }

                commentText += item;
            }
        }

        return commentText;
    }

    private async Task SendMessageAsync(string prompt)
    {
        try
        {
            if (_generateCancellationTokenSource?.IsCancellationRequested != false)
            {
                return;
            }

            var useStreaming = SettingsToolkit.ReadLocalSetting(SettingNames.IsAIStreamingResponse, true);
            var result = useStreaming
                ? await SendMessageInternalAsync(SelectedService.ProviderType, prompt, OnStreaming, _generateCancellationTokenSource.Token)
                : await SendMessageInternalAsync(SelectedService.ProviderType, prompt, cancellationToken: _generateCancellationTokenSource.Token);
            TempResult = string.Empty;

            if (_generateCancellationTokenSource?.IsCancellationRequested != false)
            {
                return;
            }

            FinalResult = result.Trim();
        }
        catch (TaskCanceledException)
        {
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(UI.Models.Constants.StringNames.UserCancelled), InfoType.Warning));
        }
        catch (Exception ex)
        {
            Cancel();
            ErrorMessage = ex.Message;
            this.Get<AppViewModel>().ShowTipCommand.Execute((ex.Message, InfoType.Error));
        }
        finally
        {
            IsGenerating = false;
        }
    }

    private async Task<string> SendMessageInternalAsync(
        ChatProviderType type,
        string? message,
        Action<string>? streamingAction = null,
        CancellationToken cancellationToken = default)
    {
        var messages = new List<ChatMessage>();
        var options = new ChatOptions
        {
            ModelId = SelectedModel.Id,
        };
        var config = await this.Get<IChatConfigManager>().GetServiceConfigAsync(type, SelectedModel.Data);
        var service = GlobalDependencies.Kernel.GetRequiredService<IChatService>(type.ToString());
        service.Initialize(config);
        messages.Add(new(ChatRole.User, message));
        var responseContent = string.Empty;
        try
        {
            if (streamingAction is null)
            {
                var response = await service.Client!.CompleteAsync(messages, options, cancellationToken).ConfigureAwait(false);
                responseContent = response.Choices.FirstOrDefault()?.Text ?? string.Empty;
            }
            else
            {
                await foreach (var partialResponse in service.Client!.CompleteStreamingAsync(messages, options, cancellationToken).ConfigureAwait(false))
                {
                    if (!string.IsNullOrEmpty(partialResponse.Text))
                    {
                        streamingAction?.Invoke(partialResponse.Text);
                    }

                    responseContent += partialResponse.Text;
                }
            }
        }
        catch (TaskCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{type} | {SelectedModel.Id} 发生错误");
            throw;
        }

        return !string.IsNullOrEmpty(responseContent) ? responseContent : throw new KernelException($"{type} | {SelectedModel.Id} 返回空响应，具体错误请查看日志");
    }

    private string GetArticlePlainText()
    {
        var html = _articleDetail.HtmlContent;
        html = Regex.Replace(html, "<[^>]+>", string.Empty);
        html = Regex.Replace(html, @"\s+", " ");
        if (string.IsNullOrEmpty(html.Trim()))
        {
            throw new Exception(ResourceToolkit.GetLocalizedString(StringNames.ArticleContentEmpty));
        }

        return html;
    }

    private void OnStreaming(string text)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (_generateCancellationTokenSource is null || _generateCancellationTokenSource.IsCancellationRequested)
            {
                TempResult = string.Empty;
            }

            TempResult += text;
        });
    }
}
