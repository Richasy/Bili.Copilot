// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 用户空间信息响应结果.
/// </summary>
public class UserSpaceResponse
{
    /// <summary>
    /// 用户信息.
    /// </summary>
    [JsonPropertyName("card")]
    public UserSpaceInformation User { get; set; }

    /// <summary>
    /// 直播信息.
    /// </summary>
    [JsonPropertyName("live")]
    public UserSpaceLive Live { get; set; }

    /// <summary>
    /// 视频集.
    /// </summary>
    [JsonPropertyName("archive")]
    public UserSpaceVideoSet VideoSet { get; set; }

    /// <summary>
    /// 文章集.
    /// </summary>
    [JsonPropertyName("article")]
    public UserSpaceArticleSet ArticleSet { get; set; }
}

/// <summary>
/// 用户空间信息.
/// </summary>
public class UserSpaceInformation
{
    /// <summary>
    /// 用户Id.
    /// </summary>
    [JsonPropertyName("mid")]
    public string UserId { get; set; }

    /// <summary>
    /// 用户名.
    /// </summary>
    [JsonPropertyName("name")]
    public string UserName { get; set; }

    /// <summary>
    /// 性别.
    /// </summary>
    [JsonPropertyName("sex")]
    public string Sex { get; set; }

    /// <summary>
    /// 头像.
    /// </summary>
    [JsonPropertyName("face")]
    public string Avatar { get; set; }

    /// <summary>
    /// 粉丝数.
    /// </summary>
    [JsonPropertyName("fans")]
    public int FollowerCount { get; set; }

    /// <summary>
    /// 关注数.
    /// </summary>
    [JsonPropertyName("attention")]
    public int FollowCount { get; set; }

    /// <summary>
    /// 个性签名.
    /// </summary>
    [JsonPropertyName("sign")]
    public string Sign { get; set; }

    /// <summary>
    /// 等级信息.
    /// </summary>
    [JsonPropertyName("level_info")]
    public UserSpaceLevelInformation LevelInformation { get; set; }

    /// <summary>
    /// 大会员信息.
    /// </summary>
    [JsonPropertyName("vip")]
    public Vip Vip { get; set; }

    /// <summary>
    /// 关系.
    /// </summary>
    [JsonPropertyName("relation")]
    public UserRelation Relation { get; set; }

    /// <summary>
    /// 点赞信息.
    /// </summary>
    [JsonPropertyName("likes")]
    public UserSpaceLikeInformation LikeInformation { get; set; }
}

/// <summary>
/// 用户空间获赞信息.
/// </summary>
public class UserSpaceLikeInformation
{
    /// <summary>
    /// 点赞数.
    /// </summary>
    [JsonPropertyName("like_num")]
    public int LikeCount { get; set; }
}

/// <summary>
/// 用户空间的等级信息.
/// </summary>
public class UserSpaceLevelInformation
{
    /// <summary>
    /// 用户当前等级.
    /// </summary>
    [JsonPropertyName("current_level")]
    public int CurrentLevel { get; set; }

    /// <summary>
    /// 当前经验值.
    /// </summary>
    [JsonPropertyName("current_exp")]
    public int CurrentExperience { get; set; }
}

/// <summary>
/// 用户空间的直播信息.
/// </summary>
public class UserSpaceLive
{
    /// <summary>
    /// 直播间状态，0-未开播，1-正在直播.
    /// </summary>
    [JsonPropertyName("roomStatus")]
    public int RoomStatus { get; set; }

    /// <summary>
    /// 直播状态，0-未开播，1-正在直播，2-轮播.
    /// </summary>
    [JsonPropertyName("liveStatus")]
    public int LiveStatus { get; set; }

    /// <summary>
    /// 直播地址.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }

    /// <summary>
    /// 直播标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 封面.
    /// </summary>
    [JsonPropertyName("cover")]
    public string Cover { get; set; }

    /// <summary>
    /// 在线观看人数.
    /// </summary>
    [JsonPropertyName("online")]
    public int ViewerCount { get; set; }

    /// <summary>
    /// 直播间Id.
    /// </summary>
    [JsonPropertyName("roomid")]
    public int RoomId { get; set; }
}

/// <summary>
/// 用户空间视频集.
/// </summary>
public class UserSpaceVideoSet
{
    /// <summary>
    /// 视频总数.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>
    /// 视频列表.
    /// </summary>
    [JsonPropertyName("item")]
    public List<UserSpaceVideoItem> List { get; set; }
}

/// <summary>
/// 用户空间视频条目.
/// </summary>
public class UserSpaceVideoItem
{
    /// <summary>
    /// 标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 分区名.
    /// </summary>
    [JsonPropertyName("tname")]
    public string PartitionName { get; set; }

    /// <summary>
    /// 视频封面.
    /// </summary>
    [JsonPropertyName("cover")]
    public string Cover { get; set; }

    /// <summary>
    /// 视频Id.
    /// </summary>
    [JsonPropertyName("param")]
    public string Id { get; set; }

    /// <summary>
    /// 目标类型.
    /// </summary>
    [JsonPropertyName("goto")]
    public string Goto { get; set; }

    /// <summary>
    /// 时长.
    /// </summary>
    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    /// <summary>
    /// 是否为合作视频.
    /// </summary>
    [JsonPropertyName("is_cooperation")]
    public bool IsCooperation { get; set; }

    /// <summary>
    /// 是否为PGC内容.
    /// </summary>
    [JsonPropertyName("is_pgc")]
    public bool IsPgc { get; set; }

    /// <summary>
    /// 是否为直播回放.
    /// </summary>
    [JsonPropertyName("is_live_playback")]
    public bool IsLivePlayback { get; set; }

    /// <summary>
    /// 播放次数.
    /// </summary>
    [JsonPropertyName("play")]
    public int PlayCount { get; set; }

    /// <summary>
    /// 弹幕数.
    /// </summary>
    [JsonPropertyName("danmaku")]
    public int DanmakuCount { get; set; }

    /// <summary>
    /// 创建时间.
    /// </summary>
    [JsonPropertyName("ctime")]
    public int CreateTime { get; set; }

    /// <summary>
    /// 发布者名称.
    /// </summary>
    [JsonPropertyName("author")]
    public string PublisherName { get; set; }

    /// <summary>
    /// Bv Id.
    /// </summary>
    [JsonPropertyName("bvid")]
    public string Bvid { get; set; }

    /// <summary>
    /// 首个分P的Id.
    /// </summary>
    [JsonPropertyName("first_cid")]
    public long FirstCid { get; set; }
}

/// <summary>
/// 用户空间文章集.
/// </summary>
public class UserSpaceArticleSet
{
    /// <summary>
    /// 文章个数.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>
    /// 文章列表.
    /// </summary>
    [JsonPropertyName("item")]
    public List<Article> List { get; set; }
}

