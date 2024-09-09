// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Microsoft.SemanticKernel;

namespace BiliAgent.Core.Providers;

/// <summary>
/// Open AI 服务商.
/// </summary>
public sealed class OpenAIProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIProvider"/> class.
    /// </summary>
    public OpenAIProvider(OpenAIClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SetBaseUri(ProviderConstants.OpenAIApi, config.Endpoint);
        ServerModels = PredefinedModels.OpenAIModels;
        OrganizationId = config.OrganizationId;
    }

    /// <summary>
    /// 组织标识符.
    /// </summary>
    private string OrganizationId { get; }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
            Kernel = Kernel.CreateBuilder()
                .AddOpenAIChatCompletion(modelId, BaseUri, AccessKey, OrganizationId)
                .Build();
        }

        return Kernel;
    }
}
