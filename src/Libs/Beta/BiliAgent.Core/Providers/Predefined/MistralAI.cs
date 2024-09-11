// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using BiliAgent.Models;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> MistralAIModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "Mistral 7B",
            Id = "open-mistral-7b",
        },
        new ChatModel
        {
            DisplayName = "Mixtral 8x7b",
            Id = "open-mixtral-8x7b",
        },
        new ChatModel
        {
            DisplayName = "Mixtral 8x22B",
            Id = "open-mixtral-8x22b",
        },
        new ChatModel
        {
            DisplayName = "Mistral Small",
            Id = "mistral-small-latest",
        },
        new ChatModel
        {
            DisplayName = "Mistral Large",
            Id = "mistral-large-latest",
        },
    };
}
