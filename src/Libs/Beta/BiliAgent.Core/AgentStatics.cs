// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Models;
using RichasyKernel;

namespace BiliAgent.Core;

/// <summary>
/// Agent静态类.
/// </summary>
public static class AgentStatics
{
    /// <summary>
    /// 全局内核.
    /// </summary>
    public static Kernel GlobalKernel { get; set; }

    internal static ChatModel ToChatModel(this Richasy.AgentKernel.Models.ChatModel model)
        => new()
        {
            Id = model.Id,
            DisplayName = model.Name,
            IsCustomModel = false,
        };
}
