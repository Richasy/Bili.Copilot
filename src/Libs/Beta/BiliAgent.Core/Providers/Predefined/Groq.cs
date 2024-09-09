// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using BiliAgent.Models;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> GroqModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "Llama3 8B",
            Id = "llama3-8b-8192",
        },
        new ChatModel
        {
            DisplayName = "Llama3 70B",
            Id = "llama3-70b-8192",
        },
        new ChatModel
        {
            DisplayName = "Mixtral 8x7b",
            Id = "mixtral-8x7b-32768",
        },
        new ChatModel
        {
            DisplayName = "Gemma 7b",
            Id = "gemma-7b-it",
        },
    };
}
