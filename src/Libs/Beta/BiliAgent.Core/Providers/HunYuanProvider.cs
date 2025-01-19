// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Microsoft.Extensions.AI;
using Richasy.AgentKernel.Chat;
using Richasy.AgentKernel.Connectors.Tencent.Models;

namespace BiliAgent.Core.Providers;

/// <summary>
/// 百度千帆提供程序.
/// </summary>
public sealed class HunyuanProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HunyuanProvider"/> class.
    /// </summary>
    public HunyuanProvider(HunyuanClientConfig config)
        : base(config.Key, config.CustomModels) => ServerModels = GetPredefinedModels(ProviderType.Hunyuan);

    /// <inheritdoc/>
    public IChatService? GetOrCreateService(string modelId)
    {
        Service ??= GetService(ProviderType.Hunyuan);
        Service!.Initialize(new HunyuanChatServiceConfig(AccessKey!, modelId));
        return Service;
    }

    /// <inheritdoc/>
    public override ChatOptions? GetChatOptions()
        => new()
        {
            Temperature = 1,
            TopP = 1,
        };
}
