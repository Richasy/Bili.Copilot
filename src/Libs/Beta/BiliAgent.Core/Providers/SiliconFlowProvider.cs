// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Richasy.AgentKernel.Chat;
using Richasy.AgentKernel.Connectors.SiliconFlow.Models;

namespace BiliAgent.Core.Providers;

/// <summary>
/// Silicon Flow 服务商.
/// </summary>
public sealed class SiliconFlowProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SiliconFlowProvider"/> class.
    /// </summary>
    public SiliconFlowProvider(SiliconFlowClientConfig config)
        : base(config.Key, config.CustomModels) => ServerModels = GetPredefinedModels(ProviderType.SiliconFlow);

    /// <inheritdoc/>
    public IChatService? GetOrCreateService(string modelId)
    {
        Service ??= GetService(ProviderType.SiliconFlow);
        Service!.Initialize(new SiliconFlowServiceConfig(AccessKey!, modelId));
        return Service;
    }
}
