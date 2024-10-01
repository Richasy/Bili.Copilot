// Copyright (c) Bili Copilot. All rights reserved.

using Azure.AI.OpenAI;
using BiliAgent.Interfaces;
using BiliAgent.Models;
using Microsoft.SemanticKernel;

namespace BiliAgent.Core.Providers;

/// <summary>
/// Azure Open AI 服务商.
/// </summary>
public sealed class AzureOpenAIProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureOpenAIProvider"/> class.
    /// </summary>
    public AzureOpenAIProvider(AzureOpenAIClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SetBaseUri(config.Endpoint);
        Version = config.Version;
    }

    /// <summary>
    /// 获取 API 版本.
    /// </summary>
    private AzureOpenAIVersion Version { get; }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
            Kernel = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(modelId, BaseUri.AbsoluteUri, AccessKey, serviceVersion: AzureOpenAIClientOptions.ServiceVersion.V2024_06_01, modelId: modelId)
                .Build();
        }

        return Kernel;
    }
}
