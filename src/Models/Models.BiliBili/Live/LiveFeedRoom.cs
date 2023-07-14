// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 直播源推荐中我关注的直播间.
/// </summary>
public class LiveFeedRoom : LiveRoomBase
{
    /// <summary>
    /// 直播间Id.
    /// </summary>
    [JsonPropertyName("roomid")]
    public long RoomId { get; set; }

    /// <summary>
    /// 用户名.
    /// </summary>
    [JsonPropertyName("uname")]
    public string UserName { get; set; }

    /// <summary>
    /// 用户头像.
    /// </summary>
    [JsonPropertyName("face")]
    public string UserAvatar { get; set; }

    /// <summary>
    /// 直播开始时间.
    /// </summary>
    [JsonPropertyName("live_time")]
    public long LiveStartTime { get; set; }

    /// <summary>
    /// 显示分区Id.
    /// </summary>
    [JsonPropertyName("area")]
    public string DisplayAreaId { get; set; }

    /// <summary>
    /// 显示分区名.
    /// </summary>
    [JsonPropertyName("area_name")]
    public string DisplayAreaName { get; set; }

    /// <summary>
    /// 分区Id.
    /// </summary>
    [JsonPropertyName("area_v2_id")]
    public long AreaId { get; set; }

    /// <summary>
    /// 分区名.
    /// </summary>
    [JsonPropertyName("area_v2_name")]
    public string AreaName { get; set; }

    /// <summary>
    /// 父分区名.
    /// </summary>
    [JsonPropertyName("area_v2_parent_name")]
    public string ParentAreaName { get; set; }

    /// <summary>
    /// 父分区Id.
    /// </summary>
    [JsonPropertyName("area_v2_parent_id")]
    public long ParentAreaId { get; set; }

    /// <summary>
    /// 直播标签名.
    /// </summary>
    [JsonPropertyName("live_tag_name")]
    public string LiveTagName { get; set; }

    /// <summary>
    /// 是否为特别关注，0-否，1-是.
    /// </summary>
    [JsonPropertyName("special_attention")]
    public int SpecialAttention { get; set; }

    /// <summary>
    /// 是否官方认证，0-否，1-是.
    /// </summary>
    [JsonPropertyName("official_verify")]
    public int OfficialVerify { get; set; }
}

/// <summary>
/// 直播源关注用户列表.
/// </summary>
public class LiveFeedFollowUserList
{
    /// <summary>
    /// 列表数据.
    /// </summary>
    [JsonPropertyName("list")]
    public List<LiveFeedRoom> List { get; set; }
}

