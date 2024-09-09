// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using BiliAgent.Models;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> HunYuanModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "HunYuan Pro",
            Id = "hunyuan-pro",
        },
        new ChatModel
        {
            DisplayName = "HunYuan Standard",
            Id = "hunyuan-standard",
        },
        new ChatModel
        {
            DisplayName = "HunYuan Lite",
            Id = "hunyuan-lite",
        },
    };
}
