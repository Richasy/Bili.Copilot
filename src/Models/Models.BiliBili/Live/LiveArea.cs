// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// ֱ�������.
/// </summary>
public class LiveArea
{
    /// <summary>
    /// ����Id.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// ����.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// ��ǩ��ַ.
    /// </summary>
    [JsonPropertyName("link")]
    public string Link { get; set; }

    /// <summary>
    /// ��־.
    /// </summary>
    [JsonPropertyName("pic")]
    public string Cover { get; set; }

    /// <summary>
    /// ������ Id.
    /// </summary>
    [JsonPropertyName("parent_id")]
    public int ParentId { get; set; }

    /// <summary>
    /// ��������.
    /// </summary>
    [JsonPropertyName("parent_name")]
    public string ParentName { get; set; }

    /// <summary>
    /// ��������.
    /// </summary>
    [JsonPropertyName("area_type")]
    public int AreaType { get; set; }

    /// <summary>
    /// �Ƿ�Ϊ�·���.
    /// </summary>
    [JsonPropertyName("is_new")]
    public bool IsNew { get; set; }
}

/// <summary>
/// ֱ���������.
/// </summary>
public class LiveAreaGroup
{
    /// <summary>
    /// ��ʶ��.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// ����.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// ����������.
    /// </summary>
    [JsonPropertyName("parent_area_type")]
    public int ParentAreaType { get; set; }

    /// <summary>
    /// �����б�.
    /// </summary>
    [JsonPropertyName("area_list")]
    public List<LiveArea> AreaList { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is LiveAreaGroup group && Id == group.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => 2108858624 + Id.GetHashCode();
}

