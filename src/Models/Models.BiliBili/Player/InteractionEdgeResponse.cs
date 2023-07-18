// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 互动视频选项响应.
/// </summary>
public class InteractionEdgeResponse
{
    /// <summary>
    /// 标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 选区列表.
    /// </summary>
    [JsonPropertyName("edges")]
    public InteractionEdge Edges { get; set; }

    /// <summary>
    /// 隐藏变量.
    /// </summary>
    [JsonPropertyName("hidden_vars")]
    public List<InteractionHiddenVariable> HiddenVariables { get; set; }
}

/// <summary>
/// 互动视频选取.
/// </summary>
public class InteractionEdge
{
    /// <summary>
    /// 互动视频问题.
    /// </summary>
    [JsonPropertyName("questions")]
    public List<InteractionQuestion> Questions { get; set; }
}

/// <summary>
/// 互动视频问题.
/// </summary>
public class InteractionQuestion
{
    /// <summary>
    /// 选项列表.
    /// </summary>
    [JsonPropertyName("choices")]
    public List<InteractionChoice> Choices { get; set; }
}

/// <summary>
/// 互动视频选项.
/// </summary>
public class InteractionChoice
{
    /// <summary>
    /// 选项Id.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    /// 条件语句.
    /// </summary>
    [JsonPropertyName("condition")]
    public string Condition { get; set; }

    /// <summary>
    /// 对应分P Id.
    /// </summary>
    [JsonPropertyName("cid")]
    public int PartId { get; set; }

    /// <summary>
    /// 选项.
    /// </summary>
    [JsonPropertyName("option")]
    public string Option { get; set; }
}

/// <summary>
/// 隐藏变量.
/// </summary>
public class InteractionHiddenVariable
{
    /// <summary>
    /// 值.
    /// </summary>
    [JsonPropertyName("value")]
    public int Value { get; set; }

    /// <summary>
    /// 标识.
    /// </summary>
    [JsonPropertyName("id_v2")]
    public string Id { get; set; }
}

