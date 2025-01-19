// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Richasy.AgentKernel.Chat;
using Richasy.AgentKernel.Connectors.Moonshot.Models;

namespace BiliAgent.Core.Providers;

/// <summary>
/// 月之暗面服务商.
/// </summary>
public sealed class MoonshotProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MoonshotProvider"/> class.
    /// </summary>
    public MoonshotProvider(MoonshotClientConfig config)
        : base(config.Key, config.CustomModels)
        => ServerModels = GetPredefinedModels(ProviderType.Moonshot);

    /// <inheritdoc/>
    public IChatService? GetOrCreateService(string modelId)
    {
        Service ??= GetService(ProviderType.Moonshot);
        Service!.Initialize(new MoonshotServiceConfig(AccessKey!, modelId));
        return Service;
    }
}
