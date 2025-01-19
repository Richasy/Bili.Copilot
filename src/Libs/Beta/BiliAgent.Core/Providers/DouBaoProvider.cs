// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Microsoft.Extensions.AI;
using Richasy.AgentKernel.Chat;
using Richasy.AgentKernel.Connectors.Volcano.Models;

namespace BiliAgent.Core.Providers;

/// <summary>
/// Azure Open AI 服务商.
/// </summary>
public sealed class DoubaoProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DoubaoProvider"/> class.
    /// </summary>
    public DoubaoProvider(DouBaoClientConfig config)
        : base(config.Key, config.CustomModels)
    {
    }

    /// <inheritdoc/>
    public IChatService? GetOrCreateService(string modelId)
    {
        Service ??= GetService(ProviderType.Doubao);
        Service!.Initialize(new DoubaoServiceConfig(AccessKey!, modelId));
        return Service;
    }

    /// <inheritdoc/>
    public override ChatOptions? GetChatOptions()
        => new()
        {
            MaxOutputTokens = default,
            Temperature = 0.7f,
            TopP = 1,
            FrequencyPenalty = 0,
        };
}
