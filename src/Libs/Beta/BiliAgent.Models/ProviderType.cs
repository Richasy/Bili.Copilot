// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BiliAgent.Models;

/// <summary>
/// 服务类型.
/// </summary>
[JsonConverter(typeof(ProviderTypeConverter))]
public enum ProviderType
{
    /// <summary>
    /// Open AI.
    /// </summary>
    OpenAI,

    /// <summary>
    /// Azure Open AI.
    /// </summary>
    AzureOpenAI,

    /// <summary>
    /// Google Gemini.
    /// </summary>
    Gemini,

    /// <summary>
    /// Anthropic.
    /// </summary>
    Anthropic,

    /// <summary>
    /// 月之暗面.
    /// </summary>
    Moonshot,

    /// <summary>
    /// 智谱 AI.
    /// </summary>
    ZhiPu,

    /// <summary>
    /// 零一万物.
    /// </summary>
    LingYi,

    /// <summary>
    /// DeepSeek.
    /// </summary>
    DeepSeek,

    /// <summary>
    /// 通义千问.
    /// </summary>
    Qwen,

    /// <summary>
    /// 文心一言.
    /// </summary>
    Ernie,

    /// <summary>
    /// 腾讯混元.
    /// </summary>
    Hunyuan,

    /// <summary>
    /// 豆包.
    /// </summary>
    Doubao,

    /// <summary>
    /// 讯飞星火.
    /// </summary>
    Spark,

    /// <summary>
    /// Open Router.
    /// </summary>
    OpenRouter,

    /// <summary>
    /// Together AI.
    /// </summary>
    TogetherAI,

    /// <summary>
    /// Groq.
    /// </summary>
    Groq,

    /// <summary>
    /// Perplexity.
    /// </summary>
    Perplexity,

    /// <summary>
    /// Mistral AI.
    /// </summary>
    Mistral,

    /// <summary>
    /// Silicon Flow.
    /// </summary>
    SiliconFlow,

    /// <summary>
    /// Ollama.
    /// </summary>
    Ollama,

    /// <summary>
    /// xAI.
    /// </summary>
    XAI,
}

/// <summary>
/// 服务类型转换器.
/// </summary>
public sealed class ProviderTypeConverter : JsonConverter<ProviderType>
{
    /// <inheritdoc/>
    public override ProviderType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString()!.ToLower(System.Globalization.CultureInfo.CurrentCulture) switch
        {
            "openai" => ProviderType.OpenAI,
            "azure_openai" or "azureopenai" => ProviderType.AzureOpenAI,
            "gemini" => ProviderType.Gemini,
            "anthropic" => ProviderType.Anthropic,
            "open_router" => ProviderType.OpenRouter,
            "together_ai" or "togetherai" => ProviderType.TogetherAI,
            "groq" => ProviderType.Groq,
            "perplexity" => ProviderType.Perplexity,
            "mistral" => ProviderType.Mistral,
            "moonshot" => ProviderType.Moonshot,
            "zhipu" => ProviderType.ZhiPu,
            "lingyi" => ProviderType.LingYi,
            "qwen" => ProviderType.Qwen,
            "ernie" => ProviderType.Ernie,
            "spark" => ProviderType.Spark,
            "deep_seek" or "deepseek" => ProviderType.DeepSeek,
            "hunyuan" => ProviderType.Hunyuan,
            "ollama" => ProviderType.Ollama,
            "silicon_flow" or "siliconflow" => ProviderType.SiliconFlow,
            "doubao" => ProviderType.Doubao,
            "xai" => ProviderType.XAI,
            _ => throw new JsonException(),
        };
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, ProviderType value, JsonSerializerOptions options)
    {
        var text = value switch
        {
            ProviderType.OpenAI => "openai",
            ProviderType.AzureOpenAI => "azure_openai",
            ProviderType.Gemini => "gemini",
            ProviderType.Anthropic => "anthropic",
            ProviderType.OpenRouter => "open_router",
            ProviderType.TogetherAI => "together_ai",
            ProviderType.Groq => "groq",
            ProviderType.Perplexity => "perplexity",
            ProviderType.Mistral => "mistral",
            ProviderType.Moonshot => "moonshot",
            ProviderType.ZhiPu => "zhipu",
            ProviderType.LingYi => "lingyi",
            ProviderType.Qwen => "qwen",
            ProviderType.Ernie => "ernie",
            ProviderType.Spark => "spark",
            ProviderType.DeepSeek => "deep_seek",
            ProviderType.Hunyuan => "hunyuan",
            ProviderType.Ollama => "ollama",
            ProviderType.SiliconFlow => "silicon_flow",
            ProviderType.Doubao => "doubao",
            ProviderType.XAI => "xai",
            _ => throw new JsonException(),
        };

        writer.WriteStringValue(text);
    }
}
