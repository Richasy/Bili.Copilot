// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using BiliAgent.Models;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> ZhiPuModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "GLM-4-0520",
            Id = "glm-4-0520",
        },
        new ChatModel
        {
            DisplayName = "GLM-4",
            Id = "glm-4",
        },
        new ChatModel
        {
            DisplayName = "GLM-4 Vision",
            Id = "glm-4v",
        },
        new ChatModel
        {
            DisplayName = "GLM-3 Turbo",
            Id = "glm-3-turbo",
        },
    };
}
