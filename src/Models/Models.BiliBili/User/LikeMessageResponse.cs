// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 点赞的消息.
/// </summary>
public class LikeMessageResponse
{
    /// <summary>
    /// 最新消息.
    /// </summary>
    [JsonPropertyName("lastest")]
    public LikeMesageLatest Latest { get; set; }

    /// <summary>
    /// 总数.
    /// </summary>
    [JsonPropertyName("total")]
    public LikeMessageTotal Total { get; set; }
}

/// <summary>
/// 最新的点赞消息.
/// </summary>
public class LikeMesageLatest
{
    /// <summary>
    /// 消息列表.
    /// </summary>
    [JsonPropertyName("items")]
    public List<LikeMessageItem> Items { get; set; }

    /// <summary>
    /// 上次查看时间.
    /// </summary>
    [JsonPropertyName("last_view_at")]
    public int LastViewTime { get; set; }
}

/// <summary>
/// 点赞消息条目.
/// </summary>
public class LikeMessageItem : MessageItem
{
    /// <summary>
    /// 该消息内包含的点赞人信息.
    /// </summary>
    [JsonPropertyName("users")]
    public List<MessageUser> Users { get; set; }

    /// <summary>
    /// 点赞消息详情.
    /// </summary>
    [JsonPropertyName("item")]
    public LikeMessageItemDetail Item { get; set; }

    /// <summary>
    /// 点赞人数.
    /// </summary>
    [JsonPropertyName("counts")]
    public int Count { get; set; }

    /// <summary>
    /// 点赞时间.
    /// </summary>
    [JsonPropertyName("like_time")]
    public long LikeTime { get; set; }

    /// <summary>
    /// 是否是最新消息（应用内赋值）.
    /// </summary>
    public bool IsLatest { get; set; }
}

/// <summary>
/// 点赞消息详情.
/// </summary>
public class LikeMessageItemDetail : MessageItemDetail
{
    /// <summary>
    /// 条目Id.
    /// </summary>
    [JsonPropertyName("item_id")]
    public long ItemId { get; set; }

    /// <summary>
    /// 描述.
    /// </summary>
    [JsonPropertyName("desc")]
    public string Description { get; set; }
}

/// <summary>
/// 历史点赞消息.
/// </summary>
public class LikeMessageTotal
{
    /// <summary>
    /// 偏移指针.
    /// </summary>
    [JsonPropertyName("cursor")]
    public MessageCursor Cursor { get; set; }

    /// <summary>
    /// 消息列表.
    /// </summary>
    [JsonPropertyName("items")]
    public List<LikeMessageItem> Items { get; set; }
}

