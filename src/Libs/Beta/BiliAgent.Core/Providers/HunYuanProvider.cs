// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.HunYuan;

namespace BiliAgent.Core.Providers;

/// <summary>
/// 百度千帆提供程序.
/// </summary>
public sealed class HunYuanProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HunYuanProvider"/> class.
    /// </summary>
    public HunYuanProvider(HunYuanClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SetBaseUri(ProviderConstants.HunYuanApi);
        SecretId = config.SecretId;
        ServerModels = PredefinedModels.HunYuanModels;
    }

    /// <summary>
    /// 密钥ID.
    /// </summary>
    private string SecretId { get; }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
            Kernel = Kernel.CreateBuilder()
                .AddHunYuanChatCompletion(modelId, SecretId, AccessKey)
                .Build();
        }

        return Kernel;
    }

    /// <inheritdoc/>
    public override PromptExecutionSettings GetPromptExecutionSettings()
        => new HunYuanPromptExecutionSettings
        {
            Temperature = 1,
            TopP = 1,
        };
}
