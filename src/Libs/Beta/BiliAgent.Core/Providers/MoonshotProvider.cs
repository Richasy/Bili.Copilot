// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Microsoft.SemanticKernel;

namespace BiliAgent.Core.Providers;

/// <summary>
/// 月之暗面服务商.
/// </summary>
public sealed class MoonshotProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MoonshotProvider"/> class.
    /// </summary>
    public MoonshotProvider(MoonshotClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        ServerModels = PredefinedModels.MoonshotModels;
        SetBaseUri(ProviderConstants.MoonshotApi);
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
