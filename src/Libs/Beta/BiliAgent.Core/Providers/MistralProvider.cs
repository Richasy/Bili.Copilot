// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Richasy.AgentKernel.Chat;
using Richasy.AgentKernel.Connectors.Mistral.Models;

namespace BiliAgent.Core.Providers;

/// <summary>
/// Mistral AI 服务商.
/// </summary>
public sealed class MistralProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MistralProvider"/> class.
    /// </summary>
    public MistralProvider(MistralClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        CodestralKey = config.CodestralKey;
        UseCodestral = config.UseCodestral;
        ServerModels = GetPredefinedModels(ProviderType.Mistral);
    }

    /// <summary>
    /// Codestral 密钥.
    /// </summary>
    public string? CodestralKey { get; set; }

    /// <summary>
    /// 是否使用 Codestral.
    /// </summary>
    public bool UseCodestral { get; set; }

    /// <inheritdoc/>
    public IChatService? GetOrCreateService(string modelId)
    {
        Service ??= GetService(ProviderType.Mistral);
        var key = UseCodestral ? CodestralKey : AccessKey;
        Service!.Initialize(new MistralServiceConfig(key!, modelId, UseCodestral));
        return Service;
    }
}
