// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using BiliAgent.Models;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> MoonshotModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "Moonshot V1 8K",
            Id = "moonshot-v1-8k",
        },
        new ChatModel
        {
            DisplayName = "Moonshot V1 32K",
            Id = "moonshot-v1-32k",
        },
        new ChatModel
        {
            DisplayName = "Moonshot V1 128K",
            Id = "moonshot-v1-128k",
        },
    };
}
