// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// PGC����ɸѡ�����Ӧ.
/// </summary>
public class PgcIndexResultResponse
{
    /// <summary>
    /// �Ƿ�����һҳ.
    /// </summary>
    [JsonPropertyName("has_next")]
    public int HasNext { get; set; }

    /// <summary>
    /// ���.
    /// </summary>
    [JsonPropertyName("list")]
    public List<PgcIndexItem> List { get; set; }

    /// <summary>
    /// ��ǰҳ��.
    /// </summary>
    [JsonPropertyName("num")]
    public int PageNumber { get; set; }

    /// <summary>
    /// ÿҳ��Ŀ��.
    /// </summary>
    [JsonPropertyName("size")]
    public int PageSize { get; set; }

    /// <summary>
    /// ����.
    /// </summary>
    [JsonPropertyName("total")]
    public int TotalCount { get; set; }
}

/// <summary>
/// PGC������Ŀ.
/// </summary>
public class PgcIndexItem
{
    /// <summary>
    /// �����ı�.
    /// </summary>
    [JsonPropertyName("badge")]
    public string BadgeText { get; set; }

    /// <summary>
    /// ɸѡ����.
    /// </summary>
    [JsonPropertyName("cover")]
    public string Cover { get; set; }

    /// <summary>
    /// ��ʾ�����ı�.
    /// </summary>
    [JsonPropertyName("index_show")]
    public string AdditionalText { get; set; }

    /// <summary>
    /// �Ƿ����.
    /// </summary>
    [JsonPropertyName("is_finish")]
    public int IsFinish { get; set; }

    /// <summary>
    /// ��ַ.
    /// </summary>
    [JsonPropertyName("link")]
    public string Link { get; set; }

    /// <summary>
    /// ý��Id.
    /// </summary>
    [JsonPropertyName("media_id")]
    public int MediaId { get; set; }

    /// <summary>
    /// �缯Id.
    /// </summary>
    [JsonPropertyName("season_id")]
    public int SeasonId { get; set; }

    /// <summary>
    /// �缯����.
    /// </summary>
    [JsonPropertyName("season_type")]
    public int SeasonType { get; set; }

    /// <summary>
    /// ����.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// ������ʾ�ı�.
    /// </summary>
    [JsonPropertyName("order")]
    public string OrderText { get; set; }
}

