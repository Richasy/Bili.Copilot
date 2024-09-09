// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using BiliAgent.Models;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> SiliconFlowModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "DeepSeek Chat V2",
            Id = "deepseek-ai/DeepSeek-V2-Chat",
        },
        new ChatModel
        {
            DisplayName = "Qwen2 72B",
            Id = "Qwen/Qwen2-72B-Instruct",
        },
        new ChatModel
        {
            DisplayName = "ChatGLM4 9B",
            Id = "THUDM/glm-4-9b-chat",
        },
    };
}
