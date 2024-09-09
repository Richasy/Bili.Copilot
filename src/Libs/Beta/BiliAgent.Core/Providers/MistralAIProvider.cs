// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Microsoft.SemanticKernel;

namespace BiliAgent.Core.Providers;

/// <summary>
/// Mistral AI 服务商.
/// </summary>
public sealed class MistralAIProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MistralAIProvider"/> class.
    /// </summary>
    public MistralAIProvider(MistralAIClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SetBaseUri(ProviderConstants.MistralAIApi);
        ServerModels = PredefinedModels.MistralAIModels;
    }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
#pragma warning disable SKEXP0070
#pragma warning disable CS0612
            Kernel = Kernel.CreateBuilder()
                .AddMistralChatCompletion(modelId, AccessKey)
                .Build();
#pragma warning restore SKEXP0070
#pragma warning restore CS0612
        }

        return Kernel;
    }
}
