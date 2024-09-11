// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.SparkDesk;

namespace BiliAgent.Core.Providers;

/// <summary>
/// 百度千帆提供程序.
/// </summary>
public sealed class SparkDeskProvider : ProviderBase, IAgentProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkDeskProvider"/> class.
    /// </summary>
    public SparkDeskProvider(SparkDeskClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        Secret = config.Secret;
        AppId = config.AppId;
        ServerModels = PredefinedModels.SparkDeskModels;
    }

    /// <summary>
    /// 获取应用程序 ID.
    /// </summary>
    private string AppId { get; }

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
                .AddSparkDeskChatCompletion(AccessKey, Secret, AppId, modelId)
                .Build();
        }

        return Kernel;
    }

    /// <inheritdoc/>
    public override PromptExecutionSettings GetPromptExecutionSettings()
        => new SparkDeskPromptExecutionSettings
        {
            Temperature = 0.7,
            TopK = 1,
        };
}
