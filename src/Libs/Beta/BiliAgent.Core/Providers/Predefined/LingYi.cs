// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using BiliAgent.Models;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> LingYiModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "Yi 34B Chat",
            Id = "yi-34b-chat-0205",
        },
        new ChatModel
        {
            DisplayName = "Yi 34B Chat 200k",
            Id = "yi-34b-chat-200k",
        },
        new ChatModel
        {
            DisplayName = "Yi Vision Plus",
            Id = "yi-vl-plus",
        },
    };
}
