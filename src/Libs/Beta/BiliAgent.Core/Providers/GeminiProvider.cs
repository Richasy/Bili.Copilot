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
                new(GeminiSafetyCategory.Sexual, GeminiSafetyThreshold.BlockOnlyHigh),
                new(GeminiSafetyCategory.SexuallyExplicit, GeminiSafetyThreshold.BlockOnlyHigh),
                new(GeminiSafetyCategory.Violence, GeminiSafetyThreshold.BlockOnlyHigh),
                new(GeminiSafetyCategory.DangerousContent, GeminiSafetyThreshold.BlockOnlyHigh),
                new(GeminiSafetyCategory.Medical, GeminiSafetyThreshold.BlockOnlyHigh),
                new(GeminiSafetyCategory.Toxicity, GeminiSafetyThreshold.BlockOnlyHigh),
                new(GeminiSafetyCategory.Harassment, GeminiSafetyThreshold.BlockOnlyHigh),
                new(GeminiSafetyCategory.Derogatory, GeminiSafetyThreshold.BlockOnlyHigh),
            ],
        };
}
