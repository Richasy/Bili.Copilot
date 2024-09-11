// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

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
        : base(config.Key, config.CustomModels)
    {
        SetBaseUri(ProviderConstants.ZhipuApi);
        ServerModels = PredefinedModels.ZhiPuModels;
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

    /// <inheritdoc/>
    public override PromptExecutionSettings GetPromptExecutionSettings()
        => new OpenAIPromptExecutionSettings
        {
            Temperature = 0.95,
            TopP = 0.7d,
        };
}
