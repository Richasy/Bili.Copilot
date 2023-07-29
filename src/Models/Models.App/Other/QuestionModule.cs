// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.App.Other;

/// <summary>
/// ����ģ��.
/// </summary>
public class QuestionModule
{
    /// <summary>
    /// ģ���ʶ��.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// ģ����.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// �����б�.
    /// </summary>
    [JsonPropertyName("items")]
    public List<QuestionItem> Questions { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is QuestionModule module && Id == module.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => 2108858624 + Id.GetHashCode();
}

/// <summary>
/// ������Ŀ.
/// </summary>
public class QuestionItem
{
    /// <summary>
    /// �����ʶ��.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// �������.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// ����˵��.
    /// </summary>
    [JsonPropertyName("answer")]
    public string Answer { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is QuestionItem item && Id == item.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => 2108858624 + Id.GetHashCode();
}

