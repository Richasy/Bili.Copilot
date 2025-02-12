// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUIKernel.Share.Toolkits;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// AI 视图模型.
/// </summary>
public sealed partial class AIViewModel
{
    private async Task InitializeVideoPromptsAsync()
    {
        if (!FileToolkit.IsLocalDataExist("video_summarize.txt", "Prompt"))
        {
            await this.Get<IFileToolkit>().WriteLocalDataAsync("video_summarize.txt", PromptConstants.VideoSummaryPrompt, default, "Prompt");
        }

        if (!FileToolkit.IsLocalDataExist("video_evaluation.txt", "Prompt"))
        {
            await this.Get<IFileToolkit>().WriteLocalDataAsync("video_evaluation.txt", PromptConstants.VideoEvaluationPrompt, default, "Prompt");
        }

        var videoSummarizePrompt = await this.Get<IFileToolkit>().ReadLocalDataAsync<string>("video_summarize.txt", default, string.Empty, "Prompt");
        var videoEvaluationPrompt = await this.Get<IFileToolkit>().ReadLocalDataAsync<string>("video_evaluation.txt", default, string.Empty, "Prompt");

        var videoSummaryItem = new AIQuickItemViewModel(
            FluentIcons.Common.Symbol.TextBulletListSquareSparkle,
            ResourceToolkit.GetLocalizedString(UI.Models.Constants.StringNames.VideoSummarize),
            "根据视频字幕及简介总结视频内容",
            "总结《{0}》的内容",
            videoSummarizePrompt,
            SummaryVideoAsync);
        var videoEvaluationItem = new AIQuickItemViewModel(
            FluentIcons.Common.Symbol.ChatSparkle,
            ResourceToolkit.GetLocalizedString(UI.Models.Constants.StringNames.VideoEvaluation),
            "根据视频内容及热门评论评价视频",
            "评价《{0}》的内容",
            videoEvaluationPrompt,
            EvaluateVideoAsync);
        QuickItems = [videoSummaryItem, videoEvaluationItem];
    }

    private async Task InitializeArticlePromptsAsync()
    {
        if (!FileToolkit.IsLocalDataExist("article_summarize.txt", "Prompt"))
        {
            await this.Get<IFileToolkit>().WriteLocalDataAsync("article_summarize.txt", PromptConstants.ArticleSummaryPrompt, default, "Prompt");
        }

        if (!FileToolkit.IsLocalDataExist("article_evaluation.txt", "Prompt"))
        {
            await this.Get<IFileToolkit>().WriteLocalDataAsync("article_evaluation.txt", PromptConstants.ArticleEvaluationPrompt, default, "Prompt");
        }

        var articleSummarizePrompt = await this.Get<IFileToolkit>().ReadLocalDataAsync<string>("article_summarize.txt", default, string.Empty, "Prompt");
        var articleEvaluationPrompt = await this.Get<IFileToolkit>().ReadLocalDataAsync<string>("article_evaluation.txt", default, string.Empty, "Prompt");

        var articleSummaryItem = new AIQuickItemViewModel(
            FluentIcons.Common.Symbol.DocumentOnePageSparkle,
            "文章总结",
            "根据文章内容总结文章",
            "总结《{0}》的内容",
            articleSummarizePrompt,
            SummaryArticleAsync);
        var articleEvaluationItem = new AIQuickItemViewModel(
            FluentIcons.Common.Symbol.ChatSparkle,
            "文章评价",
            "根据文章内容及热门评论评价文章",
            "评价《{0}》的内容",
            articleEvaluationPrompt,
            EvaluateArticleAsync);
        QuickItems = [articleSummaryItem, articleEvaluationItem];
    }

    private void InitOtherPrompts(AIQuickItemViewModel? currentPrompt)
    {
        _currentPrompt = currentPrompt;
        var morePrompts = QuickItems.Where(p => p != currentPrompt).ToList();
        morePrompts.Insert(0, GetCustomQuestionQuickItem());
        MorePrompts = morePrompts;
    }

    private AIQuickItemViewModel GetCustomQuestionQuickItem()
    {
        return new AIQuickItemViewModel(
            FluentIcons.Common.Symbol.Question,
            "举手提问",
            default,
            default,
            default,
            _ =>
            {
                Discard();
                return Task.CompletedTask;
            });
    }
}
