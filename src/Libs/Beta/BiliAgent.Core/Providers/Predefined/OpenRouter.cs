// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using BiliAgent.Models;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> OpenRouterModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "Auto",
            Id = "openrouter/auto",
        },
        new ChatModel
        {
            DisplayName = "Mistral 7B Instruct (free)",
            Id = "mistralai/mistral-7b-instruct:free",
        },
        new ChatModel
        {
            DisplayName = "Yi 34B Chat",
            Id = "01-ai/yi-34b-chat",
        },
    };
}
