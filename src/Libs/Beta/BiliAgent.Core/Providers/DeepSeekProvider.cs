// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Connectors.DeepSeek.Models;
using Richasy.AgentKernel.Chat;

namespace BiliAgent.Core.Providers;

/// <summary>
/// DeepSeek 服务商.
/// </summary>
public sealed class DeepSeekProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeepSeekProvider"/> class.
    /// </summary>
    public DeepSeekProvider(DeepSeekClientConfig config)
        : base(config.Key, config.CustomModels) => ServerModels = GetPredefinedModels(ProviderType.DeepSeek);

    /// <inheritdoc/>
    public IChatService? GetOrCreateService(string modelId)
    {
        Service ??= GetService(ProviderType.DeepSeek);
        Service!.Initialize(new DeepSeekServiceConfig(AccessKey!, modelId));
        return Service;
    }
}
