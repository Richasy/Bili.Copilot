// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 通知消息项视图模型.
/// </summary>
public sealed partial class NotifyMessageItemViewModel
{
    /// <summary>
    /// 发布相对时间.
    /// </summary>
    public string PublishRelativeTime { get; init; }

    /// <summary>
    /// 第一个用户的头像.
    /// </summary>
    public Uri? FirstUserAvatar { get; init; }

    /// <summary>
    /// 第一个用户的名字.
    /// </summary>
    public string? FirstUserName { get; init; }

    /// <summary>
    /// 是否是多用户.
    /// </summary>
    public bool IsMultipleUsers { get; init; }

    /// <summary>
    /// 副标题.
    /// </summary>
    public string Subtitle { get; init; }
}
