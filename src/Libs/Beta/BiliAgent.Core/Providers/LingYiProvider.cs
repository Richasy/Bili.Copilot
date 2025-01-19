// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Richasy.AgentKernel.Chat;
using Richasy.AgentKernel.Connectors.LingYi.Models;

namespace BiliAgent.Core.Providers;

/// <summary>
/// 零一万物服务商.
/// </summary>
public sealed class LingYiProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LingYiProvider"/> class.
    /// </summary>
    public LingYiProvider(LingYiClientConfig config)
        : base(config.Key, config.CustomModels) => ServerModels = GetPredefinedModels(ProviderType.LingYi);

    /// <inheritdoc/>
    public IChatService? GetOrCreateService(string modelId)
    {
        Service ??= GetService(ProviderType.LingYi);
        Service!.Initialize(new LingYiServiceConfig(AccessKey!, modelId));
        return Service;
    }
}
