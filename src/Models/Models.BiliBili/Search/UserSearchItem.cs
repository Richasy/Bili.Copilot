// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 用户搜索条目.
/// </summary>
public class UserSearchItem : SearchItemBase
{
    /// <summary>
    /// 签名/个人介绍.
    /// </summary>
    [JsonPropertyName("sign")]
    public string Sign { get; set; }

    /// <summary>
    /// 粉丝数.
    /// </summary>
    [JsonPropertyName("fans")]
    public int FollowerCount { get; set; }

    /// <summary>
    /// 等级.
    /// </summary>
    [JsonPropertyName("level")]
    public int Level { get; set; }

    /// <summary>
    /// 大会员信息.
    /// </summary>
    [JsonPropertyName("vip")]
    public Vip Vip { get; set; }

    /// <summary>
    /// 是否是UP主.
    /// </summary>
    [JsonPropertyName("is_up")]
    public bool IsUp { get; set; }

    /// <summary>
    /// 投稿数.
    /// </summary>
    [JsonPropertyName("archives")]
    public int ArchiveCount { get; set; }

    /// <summary>
    /// 直播间Id.
    /// </summary>
    [JsonPropertyName("roomid")]
    public int RoomId { get; set; }

    /// <summary>
    /// 用户Id.
    /// </summary>
    [JsonPropertyName("mid")]
    public int UserId { get; set; }

    /// <summary>
    /// 直播间网址.
    /// </summary>
    [JsonPropertyName("live_link")]
    public string LiveLink { get; set; }

    /// <summary>
    /// 用户关系.
    /// </summary>
    [JsonPropertyName("relation")]
    public UserRelation Relation { get; set; }
}

