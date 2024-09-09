// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using BiliAgent.Models;
using Microsoft.SemanticKernel;

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
    Kernel? GetOrCreateKernel(string modelId);

    /// <summary>
    /// 获取当前内核.
    /// </summary>
    /// <returns>如果有，则返回当前正在使用的内核.</returns>
    Kernel? GetCurrentKernel();

    /// <summary>
    /// 获取模型信息.
    /// </summary>
    /// <param name="modelId">模型标识符.</param>
    /// <returns>模型信息或者 <c>null</c>.</returns>
    ChatModel? GetModelOrDefault(string modelId);

    /// <summary>
    /// 获取执行设置.
    /// </summary>
    /// <remarks>
    /// 哔哩助理在特定的场景中使用 AI，提示词和参数是固定的，暂不提供自定义能力.
    /// </remarks>
    /// <returns><see cref="PromptExecutionSettings"/>.</returns>
    PromptExecutionSettings GetPromptExecutionSettings();

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
