// Copyright (c) Bili Copilot. All rights reserved.

using System;
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
                .AddAzureOpenAIChatCompletion(modelId, BaseUri.AbsoluteUri, AccessKey, apiVersion: ConvertAzureOpenAIVersion(Version), modelId: modelId)
                .Build();
        }

        return Kernel;
    }

    private static OpenAIClientOptions.ServiceVersion ConvertAzureOpenAIVersion(AzureOpenAIVersion version)
    {
        return version switch
        {
            AzureOpenAIVersion.V2022_12_01 => OpenAIClientOptions.ServiceVersion.V2022_12_01,
            AzureOpenAIVersion.V2023_05_15 or AzureOpenAIVersion.V2023_10_01_Preview => OpenAIClientOptions.ServiceVersion.V2023_05_15,
            AzureOpenAIVersion.V2023_06_01_Preview => OpenAIClientOptions.ServiceVersion.V2023_06_01_Preview,
            AzureOpenAIVersion.V2024_02_15_Preview => OpenAIClientOptions.ServiceVersion.V2024_02_15_Preview,
            AzureOpenAIVersion.V2024_03_01_Preview => OpenAIClientOptions.ServiceVersion.V2024_03_01_Preview,
            AzureOpenAIVersion.V2024_02_01 => OpenAIClientOptions.ServiceVersion.V2024_02_15_Preview,
            _ => throw new NotSupportedException("Version not supported."),
        };
    }
}
