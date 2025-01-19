// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Richasy.AgentKernel.Chat;
using Richasy.AgentKernel.Connectors.OpenAI.Models;

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
        TrySetBaseUri(config.Endpoint);
        ServerModels = GetPredefinedModels(ProviderType.OpenAI);
        OrganizationId = config.OrganizationId;
    }

    /// <summary>
    /// 组织标识符.
    /// </summary>
    private string? OrganizationId { get; }

    /// <inheritdoc/>
    public IChatService? GetOrCreateService(string modelId)
    {
        Service ??= GetService(ProviderType.OpenAI);
        Service!.Initialize(new OpenAIServiceConfig(AccessKey!, modelId, BaseUri, OrganizationId));
        return Service;
    }
}
