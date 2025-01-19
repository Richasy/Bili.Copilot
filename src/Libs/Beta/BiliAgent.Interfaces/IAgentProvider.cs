// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using BiliAgent.Models;
using Microsoft.Extensions.AI;
using Richasy.AgentKernel.Chat;

namespace BiliAgent.Interfaces;

/// <summary>
/// 助理提供程序.
/// </summary>
public interface IAgentProvider
{
    /// <summary>
    /// 创建一个内核.
    /// </summary>
    /// <param name="modelId">要使用的模型标识符.</param>
    /// <returns>内核.</returns>
    IChatService? GetOrCreateService(string modelId);

    /// <summary>
    /// 获取聊天选项.
    /// </summary>
    /// <returns><see cref="ChatOptions"/>.</returns>
    ChatOptions? GetChatOptions();

    /// <summary>
    /// 获取模型列表.
    /// </summary>
    /// <returns>模型列表.</returns>
    List<ChatModel> GetModelList();

    /// <summary>
    /// 释放资源.
    /// </summary>
    void Release();
}
