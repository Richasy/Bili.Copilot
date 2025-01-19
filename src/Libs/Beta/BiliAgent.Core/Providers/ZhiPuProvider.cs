// Copyright (c) Bili Copilot. All rights reserved.

using System;
using BiliAgent.Interfaces;
using BiliAgent.Models;
using Microsoft.Extensions.AI;
using Richasy.AgentKernel.Chat;
using Richasy.AgentKernel.Connectors.ZhiPu.Models;

namespace BiliAgent.Core.Providers;

/// <summary>
/// 智谱服务商.
/// </summary>
public sealed class ZhiPuProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ZhiPuProvider"/> class.
    /// </summary>
    public ZhiPuProvider(ZhiPuClientConfig config)
        : base(config.Key, config.CustomModels) => ServerModels = GetPredefinedModels(ProviderType.ZhiPu);

    /// <inheritdoc/>
    public IChatService? GetOrCreateService(string modelId)
    {
        Service ??= GetService(ProviderType.ZhiPu);
        Service!.Initialize(new ZhiPuServiceConfig(AccessKey!, modelId));
        return Service;
    }

    /// <inheritdoc/>
    public override ChatOptions? GetChatOptions()
        => new ZhiPuChatOptions
        {
            Temperature = 0.95f,
            TopP = 0.7f,
            VisionSupport = Service?.Config?.Model?.Contains("v-", StringComparison.OrdinalIgnoreCase) ?? false,
        };
}
