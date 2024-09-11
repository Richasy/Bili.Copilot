// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using BiliAgent.Models;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> OpenAIModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "GPT-3.5 Turbo",
            Id = "gpt-3.5-turbo",
        },
        new ChatModel
        {
            DisplayName = "GPT-4 Turbo Preview",
            Id = "gpt-4-turbo-preview",
        },
        new ChatModel
        {
            DisplayName = "GPT-4 Turbo Vision Preview",
            Id = "gpt-4-vision-preview",
        },
        new ChatModel
        {
            DisplayName = "GPT-4",
            Id = "gpt-4",
        },
        new ChatModel
        {
            DisplayName = "GPT-4 32K",
            Id = "gpt-4-32k",
        },
        new ChatModel
        {
            DisplayName = "GPT-4 Omni",
            Id = "gpt-4o",
        },
        new ChatModel
        {
            DisplayName = "GPT-4o mini",
            Id = "gpt-4o-mini",
        },
    };
}
