// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 直播套接字消息基类.
/// </summary>
public class LiveMessage
{
}

/// <summary>
/// 直播弹幕消息.
/// </summary>
public class LiveDanmakuMessage : LiveMessage
{
    /// <summary>
    /// 文本.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// 用户名.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 用户等级.
    /// </summary>
    public string Level { get; set; }

    /// <summary>
    /// 等级颜色.
    /// </summary>
    public string LevelColor { get; set; }

    /// <summary>
    /// 用户头衔.
    /// </summary>
    public string UserTitle { get; set; }

    /// <summary>
    /// 会员文本.
    /// </summary>
    public string VipText { get; set; }

    /// <summary>
    /// 勋章名.
    /// </summary>
    public string MedalName { get; set; }

    /// <summary>
    /// 勋章等级.
    /// </summary>
    public string MedalLevel { get; set; }

    /// <summary>
    /// 勋章颜色.
    /// </summary>
    public string MedalColor { get; set; }

    /// <summary>
    /// 是否为管理员.
    /// </summary>
    public bool IsAdmin { get; set; }

    /// <summary>
    /// 是否为老爷.
    /// </summary>
    public bool IsVip { get; set; }

    /// <summary>
    /// 是否为年费老爷.
    /// </summary>
    public bool IsBigVip { get; set; }

    /// <summary>
    /// 是否有徽章.
    /// </summary>
    public bool HasMedal { get; set; }

    /// <summary>
    /// 是否有头衔.
    /// </summary>
    public bool HasTitle { get; set; }

    /// <summary>
    /// 用户名颜色.
    /// </summary>
    public string UserNameColor { get; set; }

    /// <summary>
    /// 内容颜色.
    /// </summary>
    public string ContentColor { get; set; }
}

/// <summary>
/// 直播礼物消息.
/// </summary>
public class LiveGiftMessage : LiveMessage
{
    /// <summary>
    /// 用户名.
    /// </summary>
    [JsonPropertyName("uname")]
    public string UserName { get; set; }

    /// <summary>
    /// 礼物名.
    /// </summary>
    [JsonPropertyName("gift_name")]
    public string GiftName { get; set; }

    /// <summary>
    /// 动作.
    /// </summary>
    [JsonPropertyName("action")]
    public string Action { get; set; }

    /// <summary>
    /// 个数.
    /// </summary>
    [JsonPropertyName("total_num")]
    public string TotalNumber { get; set; }

    /// <summary>
    /// 礼物Id.
    /// </summary>
    [JsonPropertyName("gift_id")]
    public int GiftId { get; set; }

    /// <summary>
    /// 用户Id.
    /// </summary>
    [JsonPropertyName("uid")]
    public string UserId { get; set; }

    /// <summary>
    /// 动图.
    /// </summary>
    [JsonPropertyName("gif")]
    public string Gif { get; set; }
}

/// <summary>
/// 直播欢迎消息.
/// </summary>
public class LiveWelcomeMessage : LiveMessage
{
    /// <summary>
    /// 动图.
    /// </summary>
    [JsonPropertyName("uname")]
    public string UserName { get; set; }

    /// <summary>
    /// 用户Id.
    /// </summary>
    [JsonPropertyName("uid")]
    public string UserId { get; set; }

    /// <summary>
    /// 是否为老爷.
    /// </summary>
    [JsonPropertyName("vip")]
    public int Vip { get; set; }
}

