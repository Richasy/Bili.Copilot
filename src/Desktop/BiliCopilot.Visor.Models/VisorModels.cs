// Copyright (c) Bili Copilot. All rights reserved.

using System.Text.Json.Serialization;

namespace BiliCopilot.Visor.Models;

/// <summary>
/// Visor 命令类型
/// </summary>
public enum VisorCommandType
{
    /// <summary>
    /// 初始化
    /// </summary>
    Initialize,

    /// <summary>
    /// 更新滚动速度
    /// </summary>
    UpdateScrollSpeed,

    /// <summary>
    /// 启用滚动加速监听
    /// </summary>
    EnableScrollAccelerate,

    /// <summary>
    /// 禁用滚动加速监听
    /// </summary>
    DisableScrollAccelerate,

    /// <summary>
    /// 关闭服务
    /// </summary>
    Shutdown,
}

/// <summary>
/// Visor 命令
/// </summary>
public sealed class VisorCommand
{
    /// <summary>
    /// 命令类型
    /// </summary>
    [JsonPropertyName("type")]
    public VisorCommandType Type { get; set; }

    /// <summary>
    /// 原始速度
    /// </summary>
    [JsonPropertyName("originalSpeed")]
    public int? OriginalSpeed { get; set; }

    /// <summary>
    /// 期望速度
    /// </summary>
    [JsonPropertyName("expectedSpeed")]
    public int? ExpectedSpeed { get; set; }

    /// <summary>
    /// 是否启用滚动加速
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool? Enabled { get; set; }
}

/// <summary>
/// Visor 响应
/// </summary>
public sealed class VisorResponse
{
    /// <summary>
    /// 是否成功
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
