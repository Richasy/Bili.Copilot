// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;

namespace BiliCopilot.UI.Toolkits;

/// <summary>
/// 配置工具类.
/// </summary>
public static class ConfigToolkit
{
    private static ChatClientConfiguration? _config;

    /// <summary>
    /// 获取聊天配置.
    /// </summary>
    /// <returns>聊天配置.</returns>
    /// <exception cref="NotImplementedException">该供应商未实现.</exception>
    public static async Task<ClientConfigBase> GetChatConfigAsync(ProviderType type)
    {
        await InitializeChatConfigurationAsync();
        return type switch
        {
            ProviderType.Ollama => _config.Ollama,
            ProviderType.OpenAI => _config.OpenAI,
            ProviderType.AzureOpenAI => _config.AzureOpenAI,
            ProviderType.Gemini => _config.Gemini,
            ProviderType.Anthropic => _config.Anthropic,
            ProviderType.Moonshot => _config.Moonshot,
            ProviderType.ZhiPu => _config.ZhiPu,
            ProviderType.LingYi => _config.LingYi,
            ProviderType.DeepSeek => _config.DeepSeek,
            ProviderType.Qwen => _config.Qwen,
            ProviderType.Ernie => _config.Ernie,
            ProviderType.Hunyuan => _config.HunYuan,
            ProviderType.Spark => _config.Spark,
            ProviderType.OpenRouter => _config.OpenRouter,
            ProviderType.TogetherAI => _config.TogetherAI,
            ProviderType.Groq => _config.Groq,
            ProviderType.Perplexity => _config.Perplexity,
            ProviderType.Mistral => _config.Mistral,
            ProviderType.SiliconFlow => _config.SiliconFlow,
            ProviderType.Doubao => _config.DouBao,
            ProviderType.XAI => _config.XAI,
            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    /// 设置聊天配置.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    /// <exception cref="NotImplementedException">该供应商未实现.</exception>
    public static async Task SetChatConfigAsync(ProviderType type, ClientConfigBase config)
    {
        await InitializeChatConfigurationAsync();
        UpdateConfig(type, config);
        await FileToolkit.WriteLocalDataAsync($"{nameof(ChatClientConfiguration)}.json", _config!, GlobalSerializeContext.Default.ChatClientConfiguration);
    }

    /// <summary>
    /// 保存聊天配置.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task SaveChatConfigAsync(Dictionary<ProviderType, ClientConfigBase> config)
    {
        await InitializeChatConfigurationAsync();
        foreach (var item in config)
        {
            UpdateConfig(item.Key, item.Value);
        }

        await FileToolkit.WriteLocalDataAsync($"{nameof(ChatClientConfiguration)}.json", _config!, GlobalSerializeContext.Default.ChatClientConfiguration);
    }

    /// <summary>
    /// 重置配置工厂.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task ResetConfigFactoryAsync()
    {
        await InitializeChatConfigurationAsync();
        var factory = GlobalDependencies.Kernel.GetRequiredService<IAgentProviderFactory>();
        factory.ResetConfiguration(_config);
    }

    private static void UpdateConfig(ProviderType type, ClientConfigBase config)
    {
        switch (type)
        {
            case ProviderType.Ollama:
                _config.Ollama = (OllamaClientConfig)config;
                break;
            case ProviderType.OpenAI:
                _config.OpenAI = (OpenAIClientConfig)config;
                break;
            case ProviderType.AzureOpenAI:
                _config.AzureOpenAI = (AzureOpenAIClientConfig)config;
                break;
            case ProviderType.Gemini:
                _config.Gemini = (GeminiClientConfig)config;
                break;
            case ProviderType.Anthropic:
                _config.Anthropic = (AnthropicClientConfig)config;
                break;
            case ProviderType.Moonshot:
                _config.Moonshot = (MoonshotClientConfig)config;
                break;
            case ProviderType.ZhiPu:
                _config.ZhiPu = (ZhiPuClientConfig)config;
                break;
            case ProviderType.LingYi:
                _config.LingYi = (LingYiClientConfig)config;
                break;
            case ProviderType.DeepSeek:
                _config.DeepSeek = (DeepSeekClientConfig)config;
                break;
            case ProviderType.Qwen:
                _config.Qwen = (QwenClientConfig)config;
                break;
            case ProviderType.Ernie:
                _config.Ernie = (ErnieClientConfig)config;
                break;
            case ProviderType.Hunyuan:
                _config.HunYuan = (HunyuanClientConfig)config;
                break;
            case ProviderType.Spark:
                _config.Spark = (SparkClientConfig)config;
                break;
            case ProviderType.OpenRouter:
                _config.OpenRouter = (OpenRouterClientConfig)config;
                break;
            case ProviderType.TogetherAI:
                _config.TogetherAI = (TogetherAIClientConfig)config;
                break;
            case ProviderType.Groq:
                _config.Groq = (GroqClientConfig)config;
                break;
            case ProviderType.Perplexity:
                _config.Perplexity = (PerplexityClientConfig)config;
                break;
            case ProviderType.Mistral:
                _config.Mistral = (MistralClientConfig)config;
                break;
            case ProviderType.SiliconFlow:
                _config.SiliconFlow = (SiliconFlowClientConfig)config;
                break;
            case ProviderType.Doubao:
                _config.DouBao = (DouBaoClientConfig)config;
                break;
            case ProviderType.XAI:
                _config.XAI = (XAIClientConfig)config;
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private static async Task InitializeChatConfigurationAsync()
    {
        if (_config is not null)
        {
            return;
        }

        var config = await FileToolkit.ReadLocalDataAsync($"{nameof(ChatClientConfiguration)}.json", GlobalSerializeContext.Default.ChatClientConfiguration);
        if (config is null)
        {
            config = new ChatClientConfiguration();
            await FileToolkit.WriteLocalDataAsync($"{nameof(ChatClientConfiguration)}.json", config!, GlobalSerializeContext.Default.ChatClientConfiguration);
        }

        _config = config;
    }
}
