// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Microsoft.SemanticKernel;

namespace BiliAgent.Core.Providers;

/// <summary>
/// Perplexity 服务商.
/// </summary>
public sealed class PerplexityProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PerplexityProvider"/> class.
    /// </summary>
    public PerplexityProvider(PerplexityClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SetBaseUri(ProviderConstants.PerplexityApi);
        ServerModels = PredefinedModels.PerplexityModels;
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
