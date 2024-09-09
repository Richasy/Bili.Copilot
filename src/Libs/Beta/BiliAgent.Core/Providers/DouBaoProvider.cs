// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.DouBao;

namespace BiliAgent.Core.Providers;

/// <summary>
/// Azure Open AI 服务商.
/// </summary>
public sealed class DouBaoProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DouBaoProvider"/> class.
    /// </summary>
    public DouBaoProvider(DouBaoClientConfig config)
        : base(config.Key, config.CustomModels)
    {
    }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
            Kernel = Kernel.CreateBuilder()
                .AddDouBaoChatCompletion(modelId, AccessKey)
                .Build();
        }

        return Kernel;
    }

    /// <inheritdoc/>
    public override PromptExecutionSettings GetPromptExecutionSettings()
        => new DouBaoPromptExecutionSettings
        {
            MaxTokens = default,
            Temperature = 0.7,
            TopP = 1,
            FrequencyPenalty = 0,
        };
}
