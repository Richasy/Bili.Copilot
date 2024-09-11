// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using BiliAgent.Models;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> PerplexityModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "Sonar Small Chat",
            Id = "sonar-small-chat",
        },
        new ChatModel
        {
            DisplayName = "Sonar Small Online",
            Id = "sonar-small-online",
        },
        new ChatModel
        {
            DisplayName = "Sonar Medium Chat",
            Id = "sonar-medium-chat",
        },
        new ChatModel
        {
            DisplayName = "Sonar Medium Online",
            Id = "sonar-medium-online",
        },
    };
}
