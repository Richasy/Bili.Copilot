// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Richasy.AgentKernel.Chat;
using Richasy.AgentKernel.Connectors.TogetherAI.Models;

namespace BiliAgent.Core.Providers;

/// <summary>
/// TogetherAI 服务商.
/// </summary>
public sealed class TogetherAIProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TogetherAIProvider"/> class.
    /// </summary>
    public TogetherAIProvider(TogetherAIClientConfig config)
        : base(config.Key, config.CustomModels) => ServerModels = GetPredefinedModels(ProviderType.TogetherAI);

    /// <inheritdoc/>
    public IChatService? GetOrCreateService(string modelId)
    {
        Service ??= GetService(ProviderType.TogetherAI);
        Service!.Initialize(new TogetherAIServiceConfig(AccessKey!, modelId));
        return Service;
    }
}
