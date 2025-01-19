// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Microsoft.Extensions.AI;
using Richasy.AgentKernel.Chat;
using Richasy.AgentKernel.Connectors.Baidu.Models;

namespace BiliAgent.Core.Providers;

/// <summary>
/// 文心一言提供程序.
/// </summary>
public sealed class ErnieProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ErnieProvider"/> class.
    /// </summary>
    public ErnieProvider(ErnieClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        Secret = config.Secret!;
        ServerModels = GetPredefinedModels(ProviderType.Ernie);
    }

    /// <summary>
    /// 获取密钥.
    /// </summary>
    private string Secret { get; }

    /// <inheritdoc/>
    public IChatService? GetOrCreateService(string modelId)
    {
        Service ??= GetService(ProviderType.Ernie);
        Service!.Initialize(new ErnieServiceConfig(AccessKey!, Secret, modelId));
        return Service;
    }

    /// <inheritdoc/>
    public override ChatOptions? GetChatOptions()
        => new()
        {
            Temperature = 0.8f,
            TopP = 0.8f,
            PresencePenalty = 1,
        };
}
