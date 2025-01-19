// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using BiliAgent.Models;
using Microsoft.Extensions.AI;
using Richasy.AgentKernel.Chat;

namespace BiliAgent.Core.Providers;

/// <summary>
/// 服务商基类.
/// </summary>
public abstract class ProviderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProviderBase"/> class.
    /// </summary>
    protected ProviderBase(string? key, List<ChatModel>? customModels = null)
    {
        AccessKey = key;

        if (customModels != null)
        {
            CustomModels = customModels;
        }
    }

    /// <summary>
    /// 自定义的模型列表.
    /// </summary>
    public List<ChatModel>? CustomModels { get; set; }

    /// <summary>
    /// 服务端模型列表.
    /// </summary>
    public List<ChatModel>? ServerModels { get; set; }

    /// <summary>
    /// 访问密钥.
    /// </summary>
    protected string? AccessKey { get; set; }

    /// <summary>
    /// 内核.
    /// </summary>
    protected IChatService? Service { get; set; }

    /// <summary>
    /// 基础 URL.
    /// </summary>
    protected Uri? BaseUri { get; private set; }

    /// <summary>
    /// 服务商类型.
    /// </summary>
    protected ProviderType Type { get; set; }

    /// <summary>
    /// 获取当前模型 ID.
    /// </summary>
    /// <returns>模型 ID.</returns>
    public string? GetCurrentModelId()
        => Service!.Config!.Model;

    /// <summary>
    /// 获取模型信息.
    /// </summary>
    /// <param name="modelId">模型标识符.</param>
    /// <returns>模型信息或者 <c>null</c>.</returns>
    public ChatModel? GetModelOrDefault(string modelId)
        => CustomModels?.FirstOrDefault(m => m.Id == modelId)
            ?? ServerModels?.FirstOrDefault(m => m.Id == modelId)
            ?? default;

    /// <summary>
    /// 获取模型列表.
    /// </summary>
    /// <returns>模型列表.</returns>
    public List<ChatModel> GetModelList()
    {
        var models = new List<ChatModel>();
        if (ServerModels != null)
        {
            models.AddRange(ServerModels);
        }

        if (CustomModels != null)
        {
            models.AddRange(CustomModels);
        }

        return [.. models.Distinct().OrderByDescending(p => p.IsCustomModel)];
    }

    /// <summary>
    /// 释放资源.
    /// </summary>
    public void Release()
        => Service = default;

    /// <summary>
    /// 获取执行设置.
    /// </summary>
    /// <returns>执行设置.</returns>
    public virtual ChatOptions? GetChatOptions()
        => new()
        {
            Temperature = 0.6f,
            TopP = 1,
        };

    /// <summary>
    /// 获取服务.
    /// </summary>
    /// <param name="key">名称.</param>
    /// <returns><see cref="IChatService"/>.</returns>
    protected static IChatService? GetService(ProviderType key)
        => AgentStatics.GlobalKernel.GetRequiredService<IChatService>(key.ToString());

    /// <summary>
    /// 获取预定义模型.
    /// </summary>
    /// <returns>模型列表.</returns>
    protected static List<ChatModel> GetPredefinedModels(ProviderType type)
        => AgentStatics.GlobalKernel.GetRequiredService<IChatModelProvider>(type.ToString()).GetModels().ToList().ConvertAll(p => p.ToChatModel());

    /// <summary>
    /// 设置基础 URL.
    /// </summary>
    protected void TrySetBaseUri(string? proxyUrl = null)
    {
        if (string.IsNullOrEmpty(proxyUrl))
        {
            BaseUri = default;
            return;
        }

        if (!proxyUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            var isLocalHost = proxyUrl.Contains("localhost", StringComparison.OrdinalIgnoreCase) || proxyUrl.Contains("127.0.0.1", StringComparison.OrdinalIgnoreCase) || proxyUrl.Contains("0.0.0.0", StringComparison.OrdinalIgnoreCase);
            var schema = isLocalHost ? "http" : "https";
            proxyUrl = $"{schema}://{proxyUrl}";
        }

        if (Uri.TryCreate(proxyUrl, UriKind.Absolute, out var uri))
        {
            BaseUri = uri;
        }
    }
}
