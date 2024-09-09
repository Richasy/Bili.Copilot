// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Microsoft.SemanticKernel;

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
        : base(config.Key, config.CustomModels)
    {
        ServerModels = PredefinedModels.SiliconFlowModels;
        SetBaseUri(ProviderConstants.SiliconFlowApi);
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
