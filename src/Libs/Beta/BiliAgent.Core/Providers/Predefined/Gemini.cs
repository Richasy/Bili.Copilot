// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using BiliAgent.Models;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> GeminiModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "Gemini 1.5 Pro",
            Id = "gemini-1.5-pro",
        },
        new ChatModel
        {
            DisplayName = "Gemini 1.5 Flash",
            Id = "gemini-1.5-flash",
        },
        new ChatModel
        {
            DisplayName = "Gemini Pro",
            Id = "gemini-1.0-pro",
        },
        new ChatModel
        {
            DisplayName = "Gemini 1.0 Pro Vision",
            Id = "gemini-pro-vision",
        },
    };
}
