// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Richasy.AgentKernel.Chat;
using RichasyKernel;

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
    public IReadOnlyList<ChatModel> GetPredefinedModels(ProviderType type)
    {
        var modelProvider = AgentStatics.GlobalKernel.GetRequiredService<IChatModelProvider>(type.ToString());
        return [.. modelProvider.GetModels().Select(p => p.ToChatModel())];
    }

    /// <inheritdoc/>
    public async Task<string> SendMessageAsync(
        ProviderType type,
        string modelId,
        string? message,
        Action<string>? streamingAction = null,
        CancellationToken cancellationToken = default)
    {
        var messages = new List<ChatMessage>();
        var provider = GetProvider(type);
        var options = GetExecutionSettings(type);
        options!.ModelId = modelId;
        var service = FindChatServiceByProvider(type, modelId) ?? throw new ArgumentException($"{type} | {modelId} 没有找到实现");
        messages.Add(new(ChatRole.User, message));
        var responseContent = string.Empty;
        try
        {
            if (streamingAction is null)
            {
                var response = await service.Client!.CompleteAsync(messages, options, cancellationToken).ConfigureAwait(false);
                responseContent = response.Choices.FirstOrDefault()?.Text ?? string.Empty;
            }
            else
            {
                await foreach (var partialResponse in service.Client!.CompleteStreamingAsync(messages, options, cancellationToken).ConfigureAwait(false))
                {
                    if (!string.IsNullOrEmpty(partialResponse.Text))
                    {
                        streamingAction?.Invoke(partialResponse.Text);
                    }

                    responseContent += partialResponse.Text;
                }
            }
        }
        catch (TaskCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{type} | {modelId} 发生错误");
            throw;
        }

        return !string.IsNullOrEmpty(responseContent) ? responseContent : throw new KernelException($"{type} | {modelId} 返回空响应，具体错误请查看日志");
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private ChatOptions? GetExecutionSettings(ProviderType provider)
        => GetProvider(provider).GetChatOptions();

    private IChatService? FindChatServiceByProvider(ProviderType type, string modelId)
        => GetProvider(type).GetOrCreateService(modelId);

    private IAgentProvider GetProvider(ProviderType type)
        => _providerFactory.GetOrCreateProvider(type);

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
