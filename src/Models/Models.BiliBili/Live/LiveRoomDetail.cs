// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 直播间详情.
/// </summary>
public class LiveRoomDetail
{
    /// <summary>
    /// 房间信息.
    /// </summary>
    [JsonPropertyName("room_info")]
    public LiveRoomInformation RoomInformation { get; set; }

    /// <summary>
    /// 锚点信息.
    /// </summary>
    [JsonPropertyName("anchor_info")]
    public LiveAnchorInformation AnchorInformation { get; set; }
}

/// <summary>
/// 直播间信息.
/// </summary>
public class LiveRoomInformation
{
    /// <summary>
    /// 用户Id.
    /// </summary>
    [JsonPropertyName("uid")]
    public long UserId { get; set; }

    /// <summary>
    /// 直播间Id.
    /// </summary>
    [JsonPropertyName("room_id")]
    public int RoomId { get; set; }

    /// <summary>
    /// 标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 封面.
    /// </summary>
    [JsonPropertyName("cover")]
    public string Cover { get; set; }

    /// <summary>
    /// 标签.
    /// </summary>
    [JsonPropertyName("tags")]
    public string Tags { get; set; }

    /// <summary>
    /// 描述文本.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; }

    /// <summary>
    /// 在线观看人数.
    /// </summary>
    [JsonPropertyName("online")]
    public int ViewerCount { get; set; }

    /// <summary>
    /// 直播状态：0-未开播，1-正在直播，2-轮播.
    /// </summary>
    [JsonPropertyName("live_status")]
    public int LiveStatus { get; set; }

    /// <summary>
    /// 直播开始时间.
    /// </summary>
    [JsonPropertyName("live_start_time")]
    public int LiveStartTime { get; set; }

    /// <summary>
    /// 直播间封禁状态，0-未封禁，1-已封禁.
    /// </summary>
    [JsonPropertyName("lock_status")]
    public int LockStatus { get; set; }

    /// <summary>
    /// 封禁时间.
    /// </summary>
    [JsonPropertyName("lock_time")]
    public int LockTime { get; set; }

    /// <summary>
    /// 隐藏状态，0-未隐藏，1-已隐藏.
    /// </summary>
    [JsonPropertyName("hidden_status")]
    public int HiddenStatus { get; set; }

    /// <summary>
    /// 隐藏时间.
    /// </summary>
    [JsonPropertyName("hidden_time")]
    public int HiddenTime { get; set; }

    /// <summary>
    /// 所属区域Id.
    /// </summary>
    [JsonPropertyName("area_id")]
    public int AreaId { get; set; }

    /// <summary>
    /// 所属区域名称.
    /// </summary>
    [JsonPropertyName("area_name")]
    public string AreaName { get; set; }

    /// <summary>
    /// 父分区Id.
    /// </summary>
    [JsonPropertyName("parent_area_id")]
    public int ParentAreaId { get; set; }

    /// <summary>
    /// 父分区名称.
    /// </summary>
    [JsonPropertyName("parent_area_name")]
    public string ParentAreaName { get; set; }

    /// <summary>
    /// 关键帧（截图）.
    /// </summary>
    [JsonPropertyName("keyframe")]
    public string Keyframe { get; set; }

    /// <summary>
    /// 特别关注类型，0-非特别关注，1-特别关注.
    /// </summary>
    [JsonPropertyName("special_type")]
    public int SpecialFollowType { get; set; }
}

/// <summary>
/// 直播间锚点信息.
/// </summary>
public class LiveAnchorInformation
{
    /// <summary>
    /// 房主基本信息.
    /// </summary>
    [JsonPropertyName("base_info")]
    public LiveUserBasicInformation UserBasicInformation { get; set; }

    /// <summary>
    /// 直播等级信息.
    /// </summary>
    [JsonPropertyName("live_info")]
    public LiveLevelInformation LevelInformation { get; set; }

    /// <summary>
    /// 直播关注信息.
    /// </summary>
    [JsonPropertyName("relation_info")]
    public LiveRelationInformation RelationInformation { get; set; }

    /// <summary>
    /// 勋章信息.
    /// </summary>
    [JsonPropertyName("metal_info")]
    public LiveMedalInformation MedalInformation { get; set; }
}

/// <summary>
/// 直播用户基本信息.
/// </summary>
public class LiveUserBasicInformation
{
    /// <summary>
    /// 用户名.
    /// </summary>
    [JsonPropertyName("uname")]
    public string UserName { get; set; }

    /// <summary>
    /// 用户头像.
    /// </summary>
    [JsonPropertyName("face")]
    public string Avatar { get; set; }

    /// <summary>
    /// 性别.
    /// </summary>
    [JsonPropertyName("gender")]
    public string Gender { get; set; }
}

/// <summary>
/// 直播等级信息.
/// </summary>
public class LiveLevelInformation
{
    /// <summary>
    /// 等级.
    /// </summary>
    [JsonPropertyName("level")]
    public int Level { get; set; }
}

/// <summary>
/// 直播关注信息.
/// </summary>
public class LiveRelationInformation
{
    /// <summary>
    /// 关注人数.
    /// </summary>
    [JsonPropertyName("attention")]
    public int AttentionCount { get; set; }
}

/// <summary>
/// 直播勋章信息.
/// </summary>
public class LiveMedalInformation
{
    /// <summary>
    /// 勋章名.
    /// </summary>
    [JsonPropertyName("medal_name")]
    public string Name { get; set; }

    /// <summary>
    /// 勋章Id.
    /// </summary>
    [JsonPropertyName("medal_id")]
    public int Id { get; set; }

    /// <summary>
    /// 粉丝俱乐部（已有多少领取了勋章的粉丝）.
    /// </summary>
    [JsonPropertyName("fansclub")]
    public int FansClub { get; set; }
}

