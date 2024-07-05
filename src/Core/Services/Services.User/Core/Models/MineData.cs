﻿// Copyright (c) Richasy. All rights reserved.

using System.Text.Json.Serialization;
using Richasy.BiliKernel.Models;

namespace Richasy.BiliKernel.Services.User.Core;

/// <summary>
/// 我的基本数据.
/// </summary>
internal sealed class Mine
{
    /// <summary>
    /// 用户ID.
    /// </summary>
    [JsonPropertyName("mid")]
    public long Mid { get; set; }

    /// <summary>
    /// 用户名.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 用户头像.
    /// </summary>
    [JsonPropertyName("face")]
    public string? Avatar { get; set; }

    /// <summary>
    /// 硬币数.
    /// </summary>
    [JsonPropertyName("coin")]
    public double? CoinNumber { get; set; }

    /// <summary>
    /// B币数.
    /// </summary>
    [JsonPropertyName("bcoin")]
    public double? BcoinCount { get; set; }

    /// <summary>
    /// 性别，0-保密，1-男性，2-女性.
    /// </summary>
    [JsonPropertyName("sex")]
    [JsonConverter(typeof(DefaultUserGenderConverter))]
    public UserGender? Gender { get; set; }

    /// <summary>
    /// 等级.
    /// </summary>
    [JsonPropertyName("level")]
    public int? Level { get; set; }

    /// <summary>
    /// 动态数.
    /// </summary>
    [JsonPropertyName("dynamic")]
    public int? DynamicCount { get; set; }

    /// <summary>
    /// 关注数.
    /// </summary>
    [JsonPropertyName("following")]
    public int? FollowCount { get; set; }

    /// <summary>
    /// 粉丝数.
    /// </summary>
    [JsonPropertyName("follower")]
    public int? FansCount { get; set; }
}
