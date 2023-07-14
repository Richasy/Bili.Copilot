// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 粉丝列表响应结果.
/// </summary>
public class RelatedUserResponse
{
    /// <summary>
    /// 粉丝列表.
    /// </summary>
    [JsonPropertyName("list")]
    public List<RelatedUser> UserList { get; set; }

    /// <summary>
    /// 总数.
    /// </summary>
    [JsonPropertyName("total")]
    public int TotalCount { get; set; }
}

/// <summary>
/// 相关的用户，用于粉丝或关注.
/// </summary>
public class RelatedUser
{
    /// <summary>
    /// 用户ID.
    /// </summary>
    [JsonPropertyName("mid")]
    public long Mid { get; set; }

    /// <summary>
    /// 关注方式，0-未关注，2-已关注，3-已互粉.
    /// </summary>
    [JsonPropertyName("attribute")]
    public int Attribute { get; set; }

    /// <summary>
    /// 成为粉丝/关注用户的时间.
    /// </summary>
    [JsonPropertyName("mtime")]
    public int StartTime { get; set; }

    /// <summary>
    /// 是否为特别关注，0-是，1-否.
    /// </summary>
    [JsonPropertyName("special")]
    public int Special { get; set; }

    /// <summary>
    /// 用户名.
    /// </summary>
    [JsonPropertyName("uname")]
    public string Name { get; set; }

    /// <summary>
    /// 头像.
    /// </summary>
    [JsonPropertyName("face")]
    public string Avatar { get; set; }

    /// <summary>
    /// 个性签名.
    /// </summary>
    [JsonPropertyName("sign")]
    public string Sign { get; set; }

    /// <summary>
    /// 会员信息.
    /// </summary>
    [JsonPropertyName("vip")]
    public Vip Vip { get; set; }
}

/// <summary>
/// 关注分组.
/// </summary>
public class RelatedTag
{
    /// <summary>
    /// 分组Id.
    /// </summary>
    [JsonPropertyName("tagid")]
    public int TagId { get; set; }

    /// <summary>
    /// 名称.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// 总数.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>
    /// 说明.
    /// </summary>
    [JsonPropertyName("tip")]
    public string Tip { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is RelatedTag tag && TagId == tag.TagId;

    /// <inheritdoc/>
    public override int GetHashCode() => 1948533646 + TagId.GetHashCode();
}

