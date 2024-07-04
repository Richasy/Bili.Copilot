// Copyright (c) Richasy. All rights reserved.

using System.Text.Json.Serialization;
using Richasy.BiliKernel.Models;

namespace Richasy.BiliKernel.Services.User.Core;

/// <summary>
/// 我的信息.
/// </summary>
internal sealed class MyInfo
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
    /// 个性签名.
    /// </summary>
    [JsonPropertyName("sign")]
    public string? Sign { get; set; }

    /// <summary>
    /// 硬币数.
    /// </summary>
    [JsonPropertyName("coins")]
    public double Coins { get; set; }

    /// <summary>
    /// 生日，格式为YYYY-MM-DD.
    /// </summary>
    [JsonPropertyName("birthday")]
    public string? Birthday { get; set; }

    /// <summary>
    /// 头像.
    /// </summary>
    [JsonPropertyName("face")]
    public string? Avatar { get; set; }

    /// <summary>
    /// 性别.
    /// </summary>
    [JsonPropertyName("sex")]
    [JsonConverter(typeof(DefaultUserGenderrConverter))]
    public UserGender? Gender { get; set; }

    /// <summary>
    /// 等级.
    /// </summary>
    [JsonPropertyName("level")]
    public int Level { get; set; }

    /// <summary>
    /// 是否被禁言.
    /// </summary>
    [JsonPropertyName("silence")]
    public int IsBlocking { get; set; }

    /// <summary>
    /// VIP 信息.
    /// </summary>
    [JsonPropertyName("vip")]
    public Vip VIP { get; set; }
}

/// <summary>
/// VIP 信息.
/// </summary>
internal sealed class Vip
{
    /// <summary>
    /// 类型.
    /// </summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>
    /// 状态，1表示是大会员.
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>
    /// 过期时间.
    /// </summary>
    [JsonPropertyName("due_date")]
    public long DueDate { get; set; }
}
