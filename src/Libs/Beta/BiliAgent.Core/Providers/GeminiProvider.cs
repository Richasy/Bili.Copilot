// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;

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
        SetBaseUri(ProviderConstants.GeminiApi, config.Endpoint);
        ServerModels = PredefinedModels.GeminiModels;
    }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
            Kernel = Kernel.CreateBuilder()
                .AddGoogleAIGeminiChatCompletion(modelId, BaseUri, AccessKey)
                .Build();
        }

        return Kernel;
    }

    /// <inheritdoc/>
    public override PromptExecutionSettings GetPromptExecutionSettings()
        => new GeminiPromptExecutionSettings
        {
            TopP = 1,
            TopK = 20,
            Temperature = 1,
            SafetySettings =
            [
                new(GeminiSafetyCategory.DangerousContent, GeminiSafetyThreshold.BlockNone),
                new(GeminiSafetyCategory.SexuallyExplicit, GeminiSafetyThreshold.BlockNone),
                new(GeminiSafetyCategory.Harassment, GeminiSafetyThreshold.BlockNone),
            ],
        };
}
