// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Richasy.AgentKernel.Chat;
using Richasy.AgentKernel.Connectors.Ollama.Models;

namespace BiliAgent.Core.Providers;

/// <summary>
/// Ollama 服务商.
/// </summary>
public sealed class OllamaProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaProvider"/> class.
    /// </summary>
    public OllamaProvider(OllamaClientConfig config)
        : base("ollama", config.CustomModels) => TrySetBaseUri(config.Endpoint);

    /// <inheritdoc/>
    public IChatService? GetOrCreateService(string modelId)
    {
        Service ??= GetService(ProviderType.Ollama);
        Service!.Initialize(new OllamaServiceConfig(modelId, BaseUri));
        return Service;
    }
}
