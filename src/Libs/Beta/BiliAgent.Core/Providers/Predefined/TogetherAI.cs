// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using BiliAgent.Models;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> TogetherAIModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "LLaMA-2-7B-32K-Instruct (7B)",
            Id = "togethercomputer/Llama-2-7B-32K-Instruct",
        },
        new ChatModel
        {
            DisplayName = "Mistral (7B) Instruct",
            Id = "mistralai/Mistral-7B-Instruct-v0.1",
        },
        new ChatModel
        {
            DisplayName = "Mixtral-8x7B Instruct",
            Id = "mistralai/Mixtral-8x7B-Instruct-v0.1",
        },
        new ChatModel
        {
            DisplayName = "Qwen 1.5 Chat (72B)",
            Id = "Qwen/Qwen1.5-72B-Chat",
        },
    };
}
