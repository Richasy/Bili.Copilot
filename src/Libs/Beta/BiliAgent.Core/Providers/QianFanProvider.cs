// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.QianFan;

namespace BiliAgent.Core.Providers;

/// <summary>
/// 百度千帆提供程序.
/// </summary>
public sealed class QianFanProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QianFanProvider"/> class.
    /// </summary>
    public QianFanProvider(QianFanClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SetBaseUri(ProviderConstants.QianFanApi);
        Secret = config.Secret;
        ServerModels = PredefinedModels.QianFanModels;
    }

    /// <summary>
    /// 获取密钥.
    /// </summary>
    private string Secret { get; }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
            Kernel = Kernel.CreateBuilder()
                .AddQianFanChatCompletion(modelId, AccessKey, Secret)
                .Build();
        }

        return Kernel;
    }

    /// <inheritdoc/>
    public override PromptExecutionSettings GetPromptExecutionSettings()
        => new QianFanPromptExecutionSettings
        {
            Temperature = 0.8,
            TopP = 0.8,
            PenaltyScore = 1,
        };
}
