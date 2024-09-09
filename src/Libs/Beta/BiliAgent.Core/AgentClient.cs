// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BiliAgent.Interfaces;
using BiliAgent.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace BiliAgent.Core;

/// <summary>
/// 助理客户端.
/// </summary>
public sealed partial class AgentClient : IAgentClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AgentClient"/> class.
    /// </summary>
    public AgentClient(
        IAgentProviderFactory providerFactory,
        ILogger<AgentClient> logger)
    {
        _providerFactory = providerFactory;
        _logger = logger;
    }

    /// <inheritdoc/>
    public IReadOnlyList<ChatModel> GetModels(ProviderType type)
        => GetProvider(type).GetModelList();

    /// <inheritdoc/>
    public async Task<ChatMessageContent> SendMessageAsync(
        ProviderType type,
        string modelId,
        ChatHistory session,
        string? message,
        Action<string> streamingAction = null,
        CancellationToken cancellationToken = default)
    {
        var provider = GetProvider(type);
        var executionSettings = GetExecutionSettings(type);
        executionSettings.ModelId = modelId;
        var kernel = FindKernelProvider(type, modelId) ?? throw new ArgumentException($"{type} | {modelId} 没有找到实现");
        session.AddUserMessage(message);
        var responseContent = string.Empty;
        try
        {
            await foreach (var partialResponse in kernel.GetRequiredService<IChatCompletionService>().GetStreamingChatMessageContentsAsync(session, executionSettings, kernel, cancellationToken).ConfigureAwait(false))
            {
                if (!string.IsNullOrEmpty(partialResponse.Content))
                {
                    streamingAction?.Invoke(partialResponse.Content);
                }

                responseContent += partialResponse.Content;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{type} | {modelId} 发生错误");
        }

        if (!string.IsNullOrEmpty(responseContent))
        {
            session.AddAssistantMessage(responseContent);
            return new ChatMessageContent(AuthorRole.Assistant, responseContent);
        }

        throw new Exception($"{type} | {modelId} 返回空响应，具体错误请查看日志");
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private PromptExecutionSettings GetExecutionSettings(ProviderType provider)
        => GetProvider(provider).GetPromptExecutionSettings();

    private Kernel? FindKernelProvider(ProviderType type, string modelId)
        => GetProvider(type).GetOrCreateKernel(modelId);

    private IAgentProvider GetProvider(ProviderType type)
        => _providerFactory.GetOrCreateProvider(type);

    private ChatModel? FindModelInProvider(ProviderType type, string modelId)
        => GetProvider(type).GetModelOrDefault(modelId);

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: 释放托管状态(托管对象)
            }

            // TODO: 释放未托管的资源(未托管的对象)并重写终结器
            // TODO: 将大型字段设置为 null
            _disposedValue = true;
        }
    }
}
