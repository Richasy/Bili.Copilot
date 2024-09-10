// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// AI 视图模型.
/// </summary>
public sealed partial class AIViewModel
{
    private void InitializeVideoPrompts()
    {
        var videoSummaryItem = new AIQuickItemViewModel(FluentIcons.Common.Symbol.TextBulletListSquareSparkle, "视频总结", "根据视频字幕及简介总结视频内容", string.Empty);
        QuickItems = [videoSummaryItem];
    }
}
