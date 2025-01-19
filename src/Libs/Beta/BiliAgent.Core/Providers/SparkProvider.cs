// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Microsoft.Extensions.AI;
using Richasy.AgentKernel.Chat;
using Richasy.AgentKernel.Connectors.IFlyTek.Models;

namespace BiliAgent.Core.Providers;

/// <summary>
/// 百度千帆提供程序.
/// </summary>
public sealed class SparkProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkProvider"/> class.
    /// </summary>
    public SparkProvider(SparkClientConfig config)
        : base(config.Key, config.CustomModels) => ServerModels = GetPredefinedModels(ProviderType.Spark);

    /// <inheritdoc/>
    public IChatService? GetOrCreateService(string modelId)
    {
        Service ??= GetService(ProviderType.Spark);
        Service!.Initialize(new SparkChatServiceConfig(AccessKey!, modelId));
        return Service;
    }

    /// <inheritdoc/>
    public override ChatOptions? GetChatOptions()
        => new()
        {
            Temperature = 0.7f,
            TopK = 1,
        };
}
