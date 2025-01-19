// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Microsoft.Extensions.AI;
using Richasy.AgentKernel.Chat;
using Richasy.AgentKernel.Connectors.Anthropic.Models;

namespace BiliAgent.Core.Providers;

/// <summary>
/// Anthropic 服务商.
/// </summary>
public sealed class AnthropicProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnthropicProvider"/> class.
    /// </summary>
    public AnthropicProvider(AnthropicClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        TrySetBaseUri(config.Endpoint);
        ServerModels = GetPredefinedModels(ProviderType.Anthropic);
    }

    /// <inheritdoc/>
    public IChatService? GetOrCreateService(string modelId)
    {
        Service ??= GetService(ProviderType.Anthropic);
        Service!.Initialize(new AnthropicServiceConfig(AccessKey!, modelId, default));
        return Service;
    }

    /// <inheritdoc/>
    public override ChatOptions? GetChatOptions()
        => new ChatOptions
        {
            MaxOutputTokens = default,
            Temperature = 1,
            TopP = 1,
        };
}
