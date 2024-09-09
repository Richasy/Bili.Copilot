// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using BiliAgent.Models;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> QianFanModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "ERNIE-4.0-Turbo-8K",
            Id = "ernie-4.0-turbo-8k",
        },
        new ChatModel
        {
            DisplayName = "ERNIE-4.0-8K",
            Id = "completions_pro",
        },
        new ChatModel
        {
            DisplayName = "ERNIE-3.5-8K",
            Id = "completions",
        },
        new ChatModel
        {
            DisplayName = "ERNIE-Speed-8K",
            Id = "ernie_speed",
        },
        new ChatModel
        {
            DisplayName = "ERNIE-Lite-8K",
            Id = "eb-instant",
        },
        new ChatModel
        {
            DisplayName = "ERNIE-Tiny-8K",
            Id = "ernie-tiny-8k",
        },
    };
}
