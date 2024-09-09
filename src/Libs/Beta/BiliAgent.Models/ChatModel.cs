// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Text.Json.Serialization;

namespace BiliAgent.Models;

/// <summary>
/// 模型信息.
/// </summary>
public sealed class ChatModel
{
    /// <summary>
    /// 获取或设置模型 ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// 获取或设置模型的显示名称.
    /// </summary>
    [JsonPropertyName("name")]
    public string? DisplayName { get; set; }

    /// <summary>
    /// 获取或设置该模型是否为自定义模型.
    /// </summary>
    [JsonPropertyName("custom")]
    public bool IsCustomModel { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ChatModel model && Id == model.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);

    /// <inheritdoc/>
    public override string ToString() => DisplayName ?? Id;
}
