// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.App.Other;

/// <summary>
/// 问题模块.
/// </summary>
public class QuestionModule
{
    /// <summary>
    /// 模块标识符.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 模块名.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// 问题列表.
    /// </summary>
    [JsonPropertyName("items")]
    public List<QuestionItem> Questions { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is QuestionModule module && Id == module.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => 2108858624 + Id.GetHashCode();
}

/// <summary>
/// 问题条目.
/// </summary>
public class QuestionItem
{
    /// <summary>
    /// 问题标识符.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 问题标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 问题说明.
    /// </summary>
    [JsonPropertyName("answer")]
    public string Answer { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is QuestionItem item && Id == item.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => 2108858624 + Id.GetHashCode();
}

