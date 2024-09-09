// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Microsoft.SemanticKernel;

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
        : base("ollama", config.CustomModels)
    {
        SetBaseUri(ProviderConstants.OllamaApi, config.Endpoint);
    }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
            Kernel = Kernel.CreateBuilder()
                .AddOpenAIChatCompletion(modelId, BaseUri, AccessKey)
                .Build();
        }

        return Kernel;
    }
}
