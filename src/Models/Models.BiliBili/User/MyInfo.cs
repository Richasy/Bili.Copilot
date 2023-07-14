// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 我的信息.
/// </summary>
public class MyInfo
{
    /// <summary>
    /// 用户ID.
    /// </summary>
    [JsonPropertyName("mid")]
    public int Mid { get; set; }

    /// <summary>
    /// 用户名.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// 用户签名.
    /// </summary>
    [JsonPropertyName("sign")]
    public string Sign { get; set; }

    /// <summary>
    /// 硬币数.
    /// </summary>
    [JsonPropertyName("coins")]
    public double Coins { get; set; }

    /// <summary>
    /// 生日，格式为YYYY-MM-DD.
    /// </summary>
    [JsonPropertyName("birthday")]
    public string Birthday { get; set; }

    /// <summary>
    /// 头像.
    /// </summary>
    [JsonPropertyName("face")]
    public string Avatar { get; set; }

    /// <summary>
    /// 性别，0-保密，1-男性，2-女性.
    /// </summary>
    [JsonPropertyName("sex")]
    public int Sex { get; set; }

    /// <summary>
    /// 账户等级.
    /// </summary>
    [JsonPropertyName("level")]
    public int Level { get; set; }

    /// <summary>
    /// 封禁状态，0-正常，1-被封.
    /// </summary>
    [JsonPropertyName("silence")]
    public int IsBlocking { get; set; }

    /// <summary>
    /// 大会员信息.
    /// </summary>
    [JsonPropertyName("vip")]
    public Vip VIP { get; set; }
}

/// <summary>
/// 大会员信息.
/// </summary>
public class Vip
{
    /// <summary>
    /// 大会员类型，0-非会员，1-月度大会员，2-年度及以上大会员.
    /// </summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>
    /// 会员状态，0-无，1-有.
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>
    /// 会员过期时间（毫秒Unix时间戳）.
    /// </summary>
    [JsonPropertyName("due_date")]
    public long DueDate { get; set; }
}

