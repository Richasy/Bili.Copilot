// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Richasy.AgentKernel.Chat;
using Richasy.AgentKernel.Connectors.Ali.Models;

namespace BiliAgent.Core.Providers;

/// <summary>
/// 通义千问.
/// </summary>
public sealed class QwenProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QwenProvider"/> class.
    /// </summary>
    public QwenProvider(QwenClientConfig config)
        : base(config.Key, config.CustomModels) => ServerModels = GetPredefinedModels(ProviderType.Qwen);

    /// <inheritdoc/>
    public IChatService? GetOrCreateService(string modelId)
    {
        Service ??= GetService(ProviderType.Qwen);
        Service!.Initialize(new QwenServiceConfig(AccessKey!, modelId));
        return Service;
    }
}
