// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Richasy.AgentKernel.Chat;
using Richasy.AgentKernel.Connectors.Google.Models;

namespace BiliAgent.Core.Providers;

/// <summary>
/// Gemini 服务商.
/// </summary>
public sealed class GeminiProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GeminiProvider"/> class.
    /// </summary>
    public GeminiProvider(GeminiClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        TrySetBaseUri(config.Endpoint);
        ServerModels = GetPredefinedModels(ProviderType.Gemini);
    }

    /// <inheritdoc/>
    public IChatService? GetOrCreateService(string modelId)
    {
        Service ??= GetService(ProviderType.Gemini);
        Service!.Initialize(new GeminiServiceConfig(AccessKey!, modelId, BaseUri));
        return Service;
    }
}
