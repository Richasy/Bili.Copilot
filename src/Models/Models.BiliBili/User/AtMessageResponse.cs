// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// @我的消息.
/// </summary>
public class AtMessageResponse
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
    public List<AtMessageItem> Items { get; set; }
}

/// <summary>
/// @我的消息条目.
/// </summary>
public class AtMessageItem : MessageItem
{
    /// <summary>
    /// 用户.
    /// </summary>
    [JsonPropertyName("user")]
    public MessageUser User { get; set; }

    /// <summary>
    /// 消息详情.
    /// </summary>
    [JsonPropertyName("item")]
    public AtMessageItemDetail Item { get; set; }

    /// <summary>
    /// @我的时间.
    /// </summary>
    [JsonPropertyName("at_time")]
    public long AtTime { get; set; }
}

/// <summary>
/// @我的消息条目详情.
/// </summary>
public class AtMessageItemDetail : MessageItemDetail
{
    /// <summary>
    /// 主题Id.
    /// </summary>
    [JsonPropertyName("subject_id")]
    public long SubjectId { get; set; }

    /// <summary>
    /// 源对象Id.
    /// </summary>
    [JsonPropertyName("source_id")]
    public long SourceId { get; set; }

    /// <summary>
    /// 源内容.
    /// </summary>
    [JsonPropertyName("source_content")]
    public string SourceContent { get; set; }

    /// <summary>
    /// At的人的信息.
    /// </summary>
    [JsonPropertyName("at_details")]
    public List<MessageUser> AtDetails { get; set; }
}

