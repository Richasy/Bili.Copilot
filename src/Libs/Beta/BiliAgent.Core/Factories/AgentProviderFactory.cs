// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using BiliAgent.Interfaces;
using BiliAgent.Models;

namespace BiliAgent.Core;

/// <summary>
/// 助理提供程序工厂.
/// </summary>
public sealed partial class AgentProviderFactory : IAgentProviderFactory
{
    private readonly Dictionary<ProviderType, IAgentProvider> _providers;
    private readonly Dictionary<ProviderType, Func<IAgentProvider>> _functions;

    /// <summary>
    /// Initializes a new instance of the <see cref="AgentProviderFactory"/> class.
    /// </summary>
    public AgentProviderFactory(ChatClientConfiguration configuration)
    {
        _providers = new Dictionary<ProviderType, IAgentProvider>();
        _functions = new Dictionary<ProviderType, Func<IAgentProvider>>();
        Initialize(configuration);
    }

    /// <inheritdoc/>
    public IAgentProvider GetOrCreateProvider(ProviderType type)
    {
        var providerExist = _providers.TryGetValue(type, out var provider);
        if (!providerExist && _functions.TryGetValue(type, out var createFunc))
        {
            provider = createFunc();
            _providers.Add(type, provider);
        }

        return provider ?? throw new KeyNotFoundException("Provider not found and also not provide create method.");
    }

    /// <inheritdoc/>
    public void Clear()
    {
        var existTypes = _providers.Keys.ToList();
        foreach (var type in existTypes)
        {
            RemoveProvider(type);
        }
    }

    /// <inheritdoc/>
    public void ResetConfiguration(ChatClientConfiguration configuration)
        => Initialize(configuration);

    private void Initialize(ChatClientConfiguration config)
    {
        InjectOpenAI(config.OpenAI);
        InjectAzureOpenAI(config.AzureOpenAI);
        InjectZhiPu(config.ZhiPu);
        InjectLingYi(config.LingYi);
        InjectMoonshot(config.Moonshot);
        InjectDashScope(config.DashScope);
        InjectQianFan(config.QianFan);
        InjectSparkDesk(config.SparkDesk);
        InjectGemini(config.Gemini);
        InjectGroq(config.Groq);
        InjectMistralAI(config.MistralAI);
        InjectPerplexity(config.Perplexity);
        InjectTogetherAI(config.TogetherAI);
        InjectOpenRouter(config.OpenRouter);
        InjectAnthropic(config.Anthropic);
        InjectDeepSeek(config.DeepSeek);
        InjectHunYuan(config.HunYuan);
        InjectOllama(config.Ollama);
        InjectSiliconFlow(config.SiliconFlow);
        InjectDouBao(config.DouBao);
    }

    private void AddCreateMethod(ProviderType type, Func<IAgentProvider> createFunc)
    {
        RemoveProvider(type);
        _functions[type] = createFunc;
    }

    private void RemoveProvider(ProviderType type)
    {
        if (_providers.TryGetValue(type, out var value))
        {
            value.Release();
            _providers.Remove(type);
        }
    }
}
