// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.BiliKernel.Models.Appearance;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 对话消息条目视图模型.
/// </summary>
public sealed partial class ChatMessageItemViewModel
{
    /// <summary>
    /// 相对时间.
    /// </summary>
    public string RelativeTime { get; init; }

    /// <summary>
    /// 实际时间.
    /// </summary>
    public string ActualTime { get; init; }

    /// <summary>
    /// 是否是我.
    /// </summary>
    public bool IsMe { get; init; }

    /// <summary>
    /// 是否已撤回.
    /// </summary>
    public bool IsWithdrawn { get; init; }

    /// <summary>
    /// 消息内容.
    /// </summary>
    public EmoteText Content { get; init; }
}
