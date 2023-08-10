// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// ÎÒµÄÐÅÏ¢.
/// </summary>
public class MyInfo
{
    /// <summary>
    /// ÓÃ»§ID.
    /// </summary>
    [JsonPropertyName("mid")]
    public long Mid { get; set; }

    /// <summary>
    /// ÓÃ»§Ãû.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// ÓÃ»§Ç©Ãû.
    /// </summary>
    [JsonPropertyName("sign")]
    public string Sign { get; set; }

    /// <summary>
    /// Ó²±ÒÊý.
    /// </summary>
    [JsonPropertyName("coins")]
    public double Coins { get; set; }

    /// <summary>
    /// ÉúÈÕ£¬¸ñÊ½ÎªYYYY-MM-DD.
    /// </summary>
    [JsonPropertyName("birthday")]
    public string Birthday { get; set; }

    /// <summary>
    /// Í·Ïñ.
    /// </summary>
    [JsonPropertyName("face")]
    public string Avatar { get; set; }

    /// <summary>
    /// ÐÔ±ð£¬0-±£ÃÜ£¬1-ÄÐÐÔ£¬2-Å®ÐÔ.
    /// </summary>
    [JsonPropertyName("sex")]
    public int Sex { get; set; }

    /// <summary>
    /// ÕË»§µÈ¼¶.
    /// </summary>
    [JsonPropertyName("level")]
    public int Level { get; set; }

    /// <summary>
    /// ·â½û×´Ì¬£¬0-Õý³££¬1-±»·â.
    /// </summary>
    [JsonPropertyName("silence")]
    public int IsBlocking { get; set; }

    /// <summary>
    /// ´ó»áÔ±ÐÅÏ¢.
    /// </summary>
    [JsonPropertyName("vip")]
    public Vip VIP { get; set; }
}

/// <summary>
/// ´ó»áÔ±ÐÅÏ¢.
/// </summary>
public class Vip
{
    /// <summary>
    /// ´ó»áÔ±ÀàÐÍ£¬0-·Ç»áÔ±£¬1-ÔÂ¶È´ó»áÔ±£¬2-Äê¶È¼°ÒÔÉÏ´ó»áÔ±.
    /// </summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>
    /// »áÔ±×´Ì¬£¬0-ÎÞ£¬1-ÓÐ.
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>
    /// »áÔ±¹ýÆÚÊ±¼ä£¨ºÁÃëUnixÊ±¼ä´Á£©.
    /// </summary>
    [JsonPropertyName("due_date")]
    public long DueDate { get; set; }
}

