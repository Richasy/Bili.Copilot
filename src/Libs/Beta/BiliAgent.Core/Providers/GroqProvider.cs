// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Richasy.AgentKernel.Chat;
using Richasy.AgentKernel.Connectors.Groq.Models;

namespace BiliAgent.Core.Providers;

/// <summary>
/// 阿里灵积（包含通义千问）.
/// </summary>
public sealed class GroqProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GroqProvider"/> class.
    /// </summary>
    public GroqProvider(GroqClientConfig config)
        : base(config.Key, config.CustomModels)
        => ServerModels = GetPredefinedModels(ProviderType.Groq);

    /// <inheritdoc/>
    public IChatService? GetOrCreateService(string modelId)
    {
        Service ??= GetService(ProviderType.Groq);
        Service!.Initialize(new GroqServiceConfig(AccessKey!, modelId));
        return Service;
    }
}
