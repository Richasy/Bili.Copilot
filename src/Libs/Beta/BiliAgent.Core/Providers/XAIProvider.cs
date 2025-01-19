// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Richasy.AgentKernel.Chat;
using Richasy.AgentKernel.Connectors.XAI.Models;

namespace BiliAgent.Core.Providers;

/// <summary>
/// xAI 服务商.
/// </summary>
public sealed class XAIProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XAIProvider"/> class.
    /// </summary>
    public XAIProvider(XAIClientConfig config)
        : base(config.Key, config.CustomModels) => ServerModels = GetPredefinedModels(ProviderType.XAI);

    /// <inheritdoc/>
    public IChatService? GetOrCreateService(string modelId)
    {
        Service ??= GetService(ProviderType.XAI);
        Service!.Initialize(new XAIServiceConfig(AccessKey!, modelId));
        return Service;
    }
}
