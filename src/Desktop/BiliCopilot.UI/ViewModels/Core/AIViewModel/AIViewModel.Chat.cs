﻿// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Richasy.BiliKernel.Bili.Comment;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models.Media;

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
            if (string.IsNullOrEmpty(subtitleText))
            {
                return;
            }

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

            // 获取评论.
            ProgressTip = ResourceToolkit.GetLocalizedString(StringNames.GettingHotComments);
            var commentService = this.Get<ICommentService>();
            var comments = await commentService.GetCommentsAsync(_videoView.Information.Identifier.Id, Richasy.BiliKernel.Models.CommentTargetType.Video, Richasy.BiliKernel.Models.CommentSortType.Hot, cancellationToken: _generateCancellationTokenSource.Token);
            if (_generateCancellationTokenSource?.IsCancellationRequested != false)
            {
                return;
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

            // 生成总结.
            ProgressTip = ResourceToolkit.GetLocalizedString(StringNames.Summarizing);
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

    private async Task<string?> GetVideoSubtitleAsync()
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
        var meta = metas?.FirstOrDefault(p => !p.IsAI) ?? metas.First();
        if (_generateCancellationTokenSource?.IsCancellationRequested != false)
        {
            return default;
        }

        var subtitles = await service.GetSubtitleDetailAsync(meta, _generateCancellationTokenSource.Token);
        return string.Join(Environment.NewLine, subtitles.Select(p => $"{AppToolkit.FormatDuration(TimeSpan.FromSeconds(p.StartPosition))} - {AppToolkit.FormatDuration(TimeSpan.FromSeconds(p.EndPosition))}: {p.Content}"));
    }

    private async Task SendMessageAsync(string prompt)
    {
        try
        {
            if (_generateCancellationTokenSource?.IsCancellationRequested != false)
            {
                return;
            }

            var result = await _client.SendMessageAsync(SelectedService.ProviderType, SelectedModel.Id, prompt, OnStreaming, _generateCancellationTokenSource.Token);
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
            this.Get<AppViewModel>().ShowTipCommand.Execute((ex.Message, InfoType.Error));
        }
        finally
        {
            IsGenerating = false;
        }
    }

    private void OnStreaming(string text)
    {
        this.Get<DispatcherQueue>().TryEnqueue(() =>
        {
            if (_generateCancellationTokenSource is null || _generateCancellationTokenSource.IsCancellationRequested)
            {
                TempResult = string.Empty;
            }

            TempResult += text;
        });
    }
}
