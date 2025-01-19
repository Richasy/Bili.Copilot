// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Richasy.AgentKernel.Chat;
using Richasy.AgentKernel.Connectors.Perplexity.Models;

namespace BiliAgent.Core.Providers;

/// <summary>
/// Perplexity 服务商.
/// </summary>
public sealed class PerplexityProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PerplexityProvider"/> class.
    /// </summary>
    public PerplexityProvider(PerplexityClientConfig config)
        : base(config.Key, config.CustomModels) => ServerModels = GetPredefinedModels(ProviderType.Perplexity);

    /// <inheritdoc/>
    public IChatService? GetOrCreateService(string modelId)
    {
        Service ??= GetService(ProviderType.Perplexity);
        Service!.Initialize(new PerplexityServiceConfig(AccessKey!, modelId));
        return Service;
    }
}
