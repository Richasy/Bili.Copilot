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
    /// 阿里灵积.
    /// </summary>
    DashScope,

    /// <summary>
    /// 百度千帆.
    /// </summary>
    QianFan,

    /// <summary>
    /// 腾讯混元.
    /// </summary>
    HunYuan,

    /// <summary>
    /// 讯飞星火.
    /// </summary>
    SparkDesk,

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
    MistralAI,

    /// <summary>
    /// Ollama.
    /// </summary>
    Ollama,

    /// <summary>
    /// Silicon Flow.
    /// </summary>
    SiliconFlow,

    /// <summary>
    /// 豆包.
    /// </summary>
    DouBao,
}

/// <summary>
/// 服务类型转换器.
/// </summary>
public sealed class ProviderTypeConverter : JsonConverter<ProviderType>
{
    /// <inheritdoc/>
    public override ProviderType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString().ToLower() switch
        {
            "openai" => ProviderType.OpenAI,
            "azure_openai" or "azureopenai" => ProviderType.AzureOpenAI,
            "gemini" => ProviderType.Gemini,
            "anthropic" => ProviderType.Anthropic,
            "open_router" => ProviderType.OpenRouter,
            "together_ai" or "togetherai" => ProviderType.TogetherAI,
            "groq" => ProviderType.Groq,
            "perplexity" => ProviderType.Perplexity,
            "mistral_ai" or "mistralai" => ProviderType.MistralAI,
            "moonshot" => ProviderType.Moonshot,
            "zhipu" => ProviderType.ZhiPu,
            "lingyi" => ProviderType.LingYi,
            "dash_scope" or "dashscope" => ProviderType.DashScope,
            "qianfan" => ProviderType.QianFan,
            "spark_desk" or "sparkdesk" => ProviderType.SparkDesk,
            "deep_seek" or "deepseek" => ProviderType.DeepSeek,
            "hunyuan" => ProviderType.HunYuan,
            "ollama" => ProviderType.Ollama,
            "silicon_flow" or "siliconflow" => ProviderType.SiliconFlow,
            "doubao" => ProviderType.DouBao,
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
            ProviderType.MistralAI => "mistral_ai",
            ProviderType.Moonshot => "moonshot",
            ProviderType.ZhiPu => "zhipu",
            ProviderType.LingYi => "lingyi",
            ProviderType.DashScope => "dash_scope",
            ProviderType.QianFan => "qianfan",
            ProviderType.SparkDesk => "spark_desk",
            ProviderType.DeepSeek => "deep_seek",
            ProviderType.HunYuan => "hunyuan",
            ProviderType.Ollama => "ollama",
            ProviderType.SiliconFlow => "silicon_flow",
            ProviderType.DouBao => "doubao",
            _ => throw new JsonException(),
        };

        writer.WriteStringValue(text);
    }
}
