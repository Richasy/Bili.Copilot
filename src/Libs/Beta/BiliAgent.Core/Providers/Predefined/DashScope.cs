// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using BiliAgent.Models;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    // 通义千问对 Function call 的支持并不完善，无法兼容标准的 OpenAI 接口，因此暂时不支持工具调用。
    internal static List<ChatModel> DashScopeModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "Qwen Turbo",
            Id = "qwen-turbo",
        },
        new ChatModel
        {
            DisplayName = "Qwen Plus",
            Id = "qwen-plus",
        },
        new ChatModel
        {
            DisplayName = "Qwen Max",
            Id = "qwen-max",
        },
        new ChatModel
        {
            DisplayName = "Qwen Max Long Context",
            Id = "qwen-max-longcontext",
        },
    };
}
