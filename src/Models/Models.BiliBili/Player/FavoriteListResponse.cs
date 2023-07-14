// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 收藏夹列表响应.
/// </summary>
public class FavoriteListResponse
{
    /// <summary>
    /// 收藏夹总数.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>
    /// 收藏夹列表.
    /// </summary>
    [JsonPropertyName("list")]
    public List<FavoriteMeta> List { get; set; }
}

/// <summary>
/// 收藏夹元数据.
/// </summary>
public class FavoriteMeta
{
    /// <summary>
    /// 收藏夹完整Id.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    /// 收藏夹原始Id.
    /// </summary>
    [JsonPropertyName("fid")]
    public long FolderId { get; set; }

    /// <summary>
    /// 用户Id.
    /// </summary>
    [JsonPropertyName("mid")]
    public long UserId { get; set; }

    /// <summary>
    /// 收藏夹标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 查询的视频是否在该收藏夹内，0-不存在，1-存在.
    /// </summary>
    [JsonPropertyName("fav_state")]
    public int FavoriteState { get; set; }

    /// <summary>
    /// 媒体数目.
    /// </summary>
    [JsonPropertyName("media_count")]
    public int MediaCount { get; set; }
}

