// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 评论消息.
/// </summary>
public class ReplyMessageResponse
{
    /// <summary>
    /// 偏移指针.
    /// </summary>
    [JsonPropertyName("cursor")]
    public MessageCursor Cursor { get; set; }

    /// <summary>
    /// 条目列表.
    /// </summary>
    [JsonPropertyName("items")]
    public List<ReplyMessageItem> Items { get; set; }

    /// <summary>
    /// 上次查看时间.
    /// </summary>
    [JsonPropertyName("last_view_at")]
    public long LastViewTime { get; set; }
}

/// <summary>
/// 评论消息条目.
/// </summary>
public class ReplyMessageItem : MessageItem
{
    /// <summary>
    /// 评论的用户.
    /// </summary>
    [JsonPropertyName("user")]
    public MessageUser User { get; set; }

    /// <summary>
    /// 评论消息详情.
    /// </summary>
    [JsonPropertyName("item")]
    public ReplyMessageItemDetail Item { get; set; }

    /// <summary>
    /// 评论人数.
    /// </summary>
    [JsonPropertyName("counts")]
    public int Counts { get; set; }

    /// <summary>
    /// 是否为多人评论.
    /// </summary>
    [JsonPropertyName("is_multi")]
    public int IsMultiple { get; set; }

    /// <summary>
    /// 评论时间.
    /// </summary>
    [JsonPropertyName("reply_time")]
    public long ReplyTime { get; set; }
}

/// <summary>
/// 评论消息条目详情.
/// </summary>
public class ReplyMessageItemDetail : AtMessageItemDetail
{
    /// <summary>
    /// 描述.
    /// </summary>
    [JsonPropertyName("desc")]
    public string Description { get; set; }
}

