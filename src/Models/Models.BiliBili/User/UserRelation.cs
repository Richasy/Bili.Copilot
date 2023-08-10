// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 用户关系.
/// </summary>
public class UserRelation
{
    /// <summary>
    /// 用户关系状态，1:未关注 2:已关注 3:被关注 4:互相关注.
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }
}

/// <summary>
/// 用户关系响应结果.
/// </summary>
public class UserRelationResponse
{
    /// <summary>
    /// 用户Id.
    /// </summary>
    [JsonPropertyName("mid")]
    public long UserId { get; set; }

    /// <summary>
    /// 关注时间，未关注则为0.
    /// </summary>
    [JsonPropertyName("mtime")]
    public long FollowTime { get; set; }

    /// <summary>
    /// 关注类型,0-未关注，2-已关注，6-已互关，128-拉黑.
    /// </summary>
    [JsonPropertyName("attribute")]
    public int Type { get; set; }

    /// <summary>
    /// 是否为特别关注，0-否，1-是.
    /// </summary>
    [JsonPropertyName("special")]
    public int IsSpecialFollow { get; set; }

    /// <summary>
    /// 是否关注.
    /// </summary>
    /// <returns>关注结果.</returns>
    public bool IsFollow() => Type is 2 or 6;
}

