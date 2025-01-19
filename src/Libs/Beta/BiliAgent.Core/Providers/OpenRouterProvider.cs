// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Richasy.AgentKernel.Chat;
using Richasy.AgentKernel.Connectors.OpenRouter.Models;

namespace BiliAgent.Core.Providers;

/// <summary>
/// Open Router 服务商.
/// </summary>
public sealed class OpenRouterProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenRouterProvider"/> class.
    /// </summary>
    public OpenRouterProvider(OpenRouterClientConfig config)
        : base(config.Key, config.CustomModels) => ServerModels = GetPredefinedModels(ProviderType.OpenRouter);

    /// <inheritdoc/>
    public IChatService? GetOrCreateService(string modelId)
    {
        Service ??= GetService(ProviderType.OpenRouter);
        Service!.Initialize(new OpenRouterServiceConfig(AccessKey!, modelId));
        return Service;
    }
}
