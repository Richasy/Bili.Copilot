// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using BiliAgent.Models;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> DeepSeekModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "DeepSeek Chat",
            Id = "deepseek-chat",
        },
        new ChatModel
        {
            DisplayName = "DeepSeek Coder",
            Id = "deepseek-coder",
        },
    };
}
