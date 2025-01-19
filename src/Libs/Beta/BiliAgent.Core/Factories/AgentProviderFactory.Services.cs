// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Core.Providers;
using BiliAgent.Models;

namespace BiliAgent.Core;

/// <summary>
/// 助理提供程序工厂.
/// </summary>
public sealed partial class AgentProviderFactory
{
    private void InjectOpenAI(OpenAIClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key))
        {
            AddCreateMethod(ProviderType.OpenAI, () => new OpenAIProvider(config));
        }
    }

    private void InjectAzureOpenAI(AzureOpenAIClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key)
            && !string.IsNullOrEmpty(config?.Endpoint)
            && config.IsCustomModelNotEmpty())
        {
            AddCreateMethod(ProviderType.AzureOpenAI, () => new AzureOpenAIProvider(config));
        }
    }

    private void InjectZhiPu(ZhiPuClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key))
        {
            AddCreateMethod(ProviderType.ZhiPu, () => new ZhiPuProvider(config));
        }
    }

    private void InjectLingYi(LingYiClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key))
        {
            AddCreateMethod(ProviderType.LingYi, () => new LingYiProvider(config));
        }
    }

    private void InjectMoonshot(MoonshotClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key))
        {
            AddCreateMethod(ProviderType.Moonshot, () => new MoonshotProvider(config));
        }
    }

    private void InjectGemini(GeminiClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key))
        {
            AddCreateMethod(ProviderType.Gemini, () => new GeminiProvider(config));
        }
    }

    private void InjectQwen(QwenClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key))
        {
            AddCreateMethod(ProviderType.Qwen, () => new QwenProvider(config));
        }
    }

    private void InjectErnie(ErnieClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key)
            && !string.IsNullOrEmpty(config?.Secret))
        {
            AddCreateMethod(ProviderType.Ernie, () => new ErnieProvider(config));
        }
    }

    private void InjectSparkDesk(SparkClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key))
        {
            AddCreateMethod(ProviderType.Spark, () => new SparkProvider(config));
        }
    }

    private void InjectGroq(GroqClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key))
        {
            AddCreateMethod(ProviderType.Groq, () => new GroqProvider(config));
        }
    }

    private void InjectMistralAI(MistralClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key))
        {
            AddCreateMethod(ProviderType.Mistral, () => new MistralProvider(config));
        }
    }

    private void InjectPerplexity(PerplexityClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key))
        {
            AddCreateMethod(ProviderType.Perplexity, () => new PerplexityProvider(config));
        }
    }

    private void InjectTogetherAI(TogetherAIClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key))
        {
            AddCreateMethod(ProviderType.TogetherAI, () => new TogetherAIProvider(config));
        }
    }

    private void InjectOpenRouter(OpenRouterClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key))
        {
            AddCreateMethod(ProviderType.OpenRouter, () => new OpenRouterProvider(config));
        }
    }

    private void InjectAnthropic(AnthropicClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key))
        {
            AddCreateMethod(ProviderType.Anthropic, () => new AnthropicProvider(config));
        }
    }

    private void InjectDeepSeek(DeepSeekClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key))
        {
            AddCreateMethod(ProviderType.DeepSeek, () => new DeepSeekProvider(config));
        }
    }

    private void InjectHunYuan(HunyuanClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key))
        {
            AddCreateMethod(ProviderType.Hunyuan, () => new HunyuanProvider(config));
        }
    }

    private void InjectOllama(OllamaClientConfig? config)
    {
        if (config != null && config.IsCustomModelNotEmpty())
        {
            AddCreateMethod(ProviderType.Ollama, () => new OllamaProvider(config));
        }
    }

    private void InjectSiliconFlow(SiliconFlowClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key))
        {
            AddCreateMethod(ProviderType.SiliconFlow, () => new SiliconFlowProvider(config));
        }
    }

    private void InjectDouBao(DouBaoClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key))
        {
            AddCreateMethod(ProviderType.Doubao, () => new DoubaoProvider(config));
        }
    }

    private void InjectXAI(XAIClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key))
        {
            AddCreateMethod(ProviderType.XAI, () => new XAIProvider(config));
        }
    }
}
