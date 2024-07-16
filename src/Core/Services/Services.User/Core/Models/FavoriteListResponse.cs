// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.User.Core;

/// <summary>
/// 收藏夹列表响应.
/// </summary>
internal sealed class FavoriteListResponse
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
    public IList<FavoriteMeta>? List { get; set; }
}
