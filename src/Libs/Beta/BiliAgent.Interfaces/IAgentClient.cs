// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BiliAgent.Models;

namespace BiliAgent.Interfaces;

/// <summary>
/// 助理客户端.
/// </summary>
public interface IAgentClient : IDisposable
{
    /// <summary>
    /// 获取模型列表.
    /// </summary>
    /// <param name="type">供应商类型.</param>
    /// <returns>模型列表.</returns>
    IReadOnlyList<ChatModel> GetModels(ProviderType type);

    /// <summary>
    /// 获取预定义模型.
    /// </summary>
    /// <param name="type">供应商类型.</param>
    /// <returns>预定义模型列表.</returns>
    IReadOnlyList<ChatModel> GetPredefinedModels(ProviderType type);

    /// <summary>
    /// 发送消息.
    /// </summary>
    /// <returns>返回的消息内容.</returns>
    Task<string> SendMessageAsync(
        ProviderType type,
        string modelId,
        string? message,
        Action<string> streamingAction = default,
        CancellationToken cancellationToken = default);
}
