// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 直播间分区.
/// </summary>
public class LiveArea
{
    /// <summary>
    /// 分区Id.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 名称.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// 标签地址.
    /// </summary>
    [JsonPropertyName("link")]
    public string Link { get; set; }

    /// <summary>
    /// 标志.
    /// </summary>
    [JsonPropertyName("pic")]
    public string Cover { get; set; }

    /// <summary>
    /// 父分区 Id.
    /// </summary>
    [JsonPropertyName("parent_id")]
    public int ParentId { get; set; }

    /// <summary>
    /// 父分区名.
    /// </summary>
    [JsonPropertyName("parent_name")]
    public string ParentName { get; set; }

    /// <summary>
    /// 分区类型.
    /// </summary>
    [JsonPropertyName("area_type")]
    public int AreaType { get; set; }

    /// <summary>
    /// 是否为新分区.
    /// </summary>
    [JsonPropertyName("is_new")]
    public bool IsNew { get; set; }
}

/// <summary>
/// 直播间分区组.
/// </summary>
public class LiveAreaGroup
{
    /// <summary>
    /// 标识符.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 名称.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// 父分区类型.
    /// </summary>
    [JsonPropertyName("parent_area_type")]
    public int ParentAreaType { get; set; }

    /// <summary>
    /// 分区列表.
    /// </summary>
    [JsonPropertyName("area_list")]
    public List<LiveArea> AreaList { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is LiveAreaGroup group && Id == group.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => 2108858624 + Id.GetHashCode();
}

