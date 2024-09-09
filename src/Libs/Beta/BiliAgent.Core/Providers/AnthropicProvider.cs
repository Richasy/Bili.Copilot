// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Anthropic;

namespace BiliAgent.Core.Providers;

/// <summary>
/// Anthropic 服务商.
/// </summary>
public sealed class AnthropicProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnthropicProvider"/> class.
    /// </summary>
    public AnthropicProvider(AnthropicClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        ServerModels = PredefinedModels.AnthropicModels;
        SetBaseUri(ProviderConstants.AnthropicApi, config.Endpoint);
    }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
            Kernel = Kernel.CreateBuilder()
                .AddAnthropicChatCompletion(modelId, AccessKey, BaseUri)
                .Build();
        }

        return Kernel;
    }

    /// <inheritdoc/>
    public override PromptExecutionSettings GetPromptExecutionSettings()
        => new AnthropicPromptExecutionSettings
        {
            MaxTokens = default,
            Temperature = 1,
            TopP = 1,
            Stream = true,
        };
}
