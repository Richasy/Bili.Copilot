// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 视频收藏概览响应.
/// </summary>
public class VideoFavoriteGalleryResponse
{
    /// <summary>
    /// 收藏夹列表.
    /// </summary>
    [JsonPropertyName("space_infos")]
    public List<FavoriteFolder> FavoriteFolderList { get; set; }

    /// <summary>
    /// 默认收藏夹.
    /// </summary>
    [JsonPropertyName("default_folder")]
    public VideoFavoriteListResponse DefaultFavoriteList { get; set; }
}

/// <summary>
/// 视频默认收藏夹.
/// </summary>
public class VideoFavoriteListResponse
{
    /// <summary>
    /// 收藏夹信息.
    /// </summary>
    [JsonPropertyName("folder_detail")]
    public FavoriteListDetail Detail { get; set; }

    /// <summary>
    /// 收藏夹信息.
    /// </summary>
    [JsonPropertyName("info")]
    public FavoriteListDetail Information { get; set; }

    /// <summary>
    /// 收藏夹的媒体列表.
    /// </summary>
    [JsonPropertyName("medias")]
    public List<FavoriteMedia> Medias { get; set; }

    /// <summary>
    /// 是否有更多.
    /// </summary>
    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }
}

/// <summary>
/// 收藏夹详情.
/// </summary>
public class FavoriteListDetail
{
    /// <summary>
    /// 收藏夹完整ID.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    /// 收藏夹原始ID.
    /// </summary>
    [JsonPropertyName("fid")]
    public long OriginId { get; set; }

    /// <summary>
    /// 用户ID.
    /// </summary>
    [JsonPropertyName("mid")]
    public long Mid { get; set; }

    /// <summary>
    /// 收藏夹标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 封面.
    /// </summary>
    [JsonPropertyName("cover")]
    public string Cover { get; set; }

    /// <summary>
    /// 创建收藏夹的用户信息.
    /// </summary>
    [JsonPropertyName("upper")]
    public PublisherInfo Publisher { get; set; }

    /// <summary>
    /// 说明/备注.
    /// </summary>
    [JsonPropertyName("intro")]
    public string Description { get; set; }

    /// <summary>
    /// 创建时间.
    /// </summary>
    [JsonPropertyName("ctime")]
    public int CreateTime { get; set; }

    /// <summary>
    /// 收藏时间.
    /// </summary>
    [JsonPropertyName("mtime")]
    public int CollectTime { get; set; }

    /// <summary>
    /// 收藏夹收藏状态，1-已收藏，0-未收藏.
    /// </summary>
    [JsonPropertyName("fav_state")]
    public int FavoriteState { get; set; }

    /// <summary>
    /// 内容数目.
    /// </summary>
    [JsonPropertyName("media_count")]
    public int MediaCount { get; set; }

    /// <summary>
    /// 查看次数.
    /// </summary>
    [JsonPropertyName("view_count")]
    public int ViewCount { get; set; }

    /// <summary>
    /// 类型.
    /// </summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>
    /// 链接.
    /// </summary>
    [JsonPropertyName("link")]
    public string Link { get; set; }
}

/// <summary>
/// 收藏夹媒体.
/// </summary>
public class FavoriteMedia
{
    /// <summary>
    /// 媒体Id.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    /// 媒体类型，2-视频，12-音频，21-视频合集.
    /// </summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

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
    /// 媒体说明文本.
    /// </summary>
    [JsonPropertyName("intro")]
    public string Description { get; set; }

    /// <summary>
    /// 页码.
    /// </summary>
    [JsonPropertyName("page")]
    public int Page { get; set; }

    /// <summary>
    /// 时长.
    /// </summary>
    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    /// <summary>
    /// 发布者.
    /// </summary>
    [JsonPropertyName("upper")]
    public PublisherInfo Publisher { get; set; }

    /// <summary>
    /// 是否有效，0-有效，1-无效.
    /// </summary>
    [JsonPropertyName("attr")]
    public int IsValid { get; set; }

    /// <summary>
    /// 用户交互数据.
    /// </summary>
    [JsonPropertyName("cnt_info")]
    public FavoriteMediaStat Stat { get; set; }

    /// <summary>
    /// 网址.
    /// </summary>
    [JsonPropertyName("link")]
    public string Link { get; set; }

    /// <summary>
    /// 创建时间.
    /// </summary>
    [JsonPropertyName("ctime")]
    public long CreateTime { get; set; }

    /// <summary>
    /// 发布时间.
    /// </summary>
    [JsonPropertyName("pubtime")]
    public long PublishTime { get; set; }

    /// <summary>
    /// 收藏时间.
    /// </summary>
    [JsonPropertyName("fav_time")]
    public long FavoriteTime { get; set; }

    /// <summary>
    /// Bv Id.
    /// </summary>
    [JsonPropertyName("bvid")]
    public string BvId { get; set; }
}

/// <summary>
/// 收藏夹媒体用户交互数据.
/// </summary>
public class FavoriteMediaStat
{
    /// <summary>
    /// 收藏数.
    /// </summary>
    [JsonPropertyName("collect")]
    public int FavoriteCount { get; set; }

    /// <summary>
    /// 播放数.
    /// </summary>
    [JsonPropertyName("play")]
    public int PlayCount { get; set; }

    /// <summary>
    /// 弹幕数.
    /// </summary>
    [JsonPropertyName("danmaku")]
    public int DanmakuCount { get; set; }
}

/// <summary>
/// 收藏夹分类.
/// </summary>
public class FavoriteFolder
{
    /// <summary>
    /// 收藏夹所属分类Id.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    /// 分类名.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// 媒体列表.
    /// </summary>
    [JsonPropertyName("mediaListResponse")]
    public FavoriteMediaList MediaList { get; set; }
}

/// <summary>
/// 收藏夹媒体列表.
/// </summary>
public class FavoriteMediaList
{
    /// <summary>
    /// 个数.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>
    /// 媒体列表.
    /// </summary>
    [JsonPropertyName("list")]
    public List<FavoriteListDetail> List { get; set; }

    /// <summary>
    /// 是否有更多.
    /// </summary>
    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }
}

