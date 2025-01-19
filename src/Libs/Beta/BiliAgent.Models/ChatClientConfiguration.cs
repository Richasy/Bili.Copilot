// Copyright (c) Bili Copilot. All rights reserved.

using System.Text.Json.Serialization;

namespace BiliAgent.Models;

/// <summary>
/// 聊天客户端配置.
/// </summary>
public sealed class ChatClientConfiguration
{
    /// <summary>
    /// Open AI 客户端配置.
    /// </summary>
    [JsonPropertyName("openai")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public OpenAIClientConfig? OpenAI { get; set; }

    /// <summary>
    /// Azure Open AI 客户端配置.
    /// </summary>
    [JsonPropertyName("azure_openai")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AzureOpenAIClientConfig? AzureOpenAI { get; set; }

    /// <summary>
    /// 智谱客户端配置.
    /// </summary>
    [JsonPropertyName("zhipu")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ZhiPuClientConfig? ZhiPu { get; set; }

    /// <summary>
    /// 零一万物客户端配置.
    /// </summary>
    [JsonPropertyName("lingyi")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public LingYiClientConfig? LingYi { get; set; }

    /// <summary>
    /// 月之暗面客户端配置.
    /// </summary>
    [JsonPropertyName("moonshot")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MoonshotClientConfig? Moonshot { get; set; }

    /// <summary>
    /// 通义千问客户端配置.
    /// </summary>
    [JsonPropertyName("qwen")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public QwenClientConfig? Qwen { get; set; }

    /// <summary>
    /// DeepSeek 客户端配置.
    /// </summary>
    [JsonPropertyName("deep_seek")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DeepSeekClientConfig? DeepSeek { get; set; }

    /// <summary>
    /// 千帆客户端配置.
    /// </summary>
    [JsonPropertyName("ernie")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ErnieClientConfig? Ernie { get; set; }

    /// <summary>
    /// 讯飞星火客户端配置.
    /// </summary>
    [JsonPropertyName("spark")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SparkClientConfig? Spark { get; set; }

    /// <summary>
    /// Gemini 客户端配置.
    /// </summary>
    [JsonPropertyName("gemini")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GeminiClientConfig? Gemini { get; set; }

    /// <summary>
    /// Groq 客户端配置.
    /// </summary>
    [JsonPropertyName("groq")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GroqClientConfig? Groq { get; set; }

    /// <summary>
    /// Mistral AI 客户端配置.
    /// </summary>
    [JsonPropertyName("mistral")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MistralClientConfig? Mistral { get; set; }

    /// <summary>
    /// Perplexity 客户端配置.
    /// </summary>
    [JsonPropertyName("perplexity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PerplexityClientConfig? Perplexity { get; set; }

    /// <summary>
    /// Together AI 客户端配置.
    /// </summary>
    [JsonPropertyName("together_ai")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TogetherAIClientConfig? TogetherAI { get; set; }

    /// <summary>
    /// Open Router 客户端配置.
    /// </summary>
    [JsonPropertyName("open_router")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public OpenRouterClientConfig? OpenRouter { get; set; }

    /// <summary>
    /// Anthropic 客户端配置.
    /// </summary>
    [JsonPropertyName("anthropic")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AnthropicClientConfig? Anthropic { get; set; }

    /// <summary>
    /// Ollama 客户端配置.
    /// </summary>
    [JsonPropertyName("ollama")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public OllamaClientConfig? Ollama { get; set; }

    /// <summary>
    /// 混元客户端配置.
    /// </summary>
    [JsonPropertyName("hunyuan")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public HunyuanClientConfig? HunYuan { get; set; }

    /// <summary>
    /// 硅动客户端配置.
    /// </summary>
    [JsonPropertyName("silicon_flow")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SiliconFlowClientConfig? SiliconFlow { get; set; }

    /// <summary>
    /// 豆包客户端配置.
    /// </summary>
    [JsonPropertyName("doubao")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DouBaoClientConfig? DouBao { get; set; }

    /// <summary>
    /// XAI 客户端配置.
    /// </summary>
    [JsonPropertyName("xai")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public XAIClientConfig? XAI { get; set; }
}

/// <summary>
/// Open AI 客户端配置.
/// </summary>
public class OpenAIClientConfig : ClientEndpointConfigBase
{
    /// <summary>
    /// 组织 ID.
    /// </summary>
    [JsonPropertyName("organization")]
    public string? OrganizationId { get; set; }
}

/// <summary>
/// Azure Open AI 客户端配置.
/// </summary>
public class AzureOpenAIClientConfig : ClientEndpointConfigBase
{
    /// <inheritdoc/>
    public override bool IsValid()
    {
        return base.IsValid()
            && !string.IsNullOrEmpty(Endpoint)
            && CustomModels?.Count > 0;
    }
}

/// <summary>
/// 千帆客户端配置.
/// </summary>
public class ErnieClientConfig : ClientConfigBase
{
    /// <summary>
    /// 密匙.
    /// </summary>
    [JsonPropertyName("secret")]
    public string? Secret { get; set; }

    /// <inheritdoc/>
    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(Secret);
}

/// <summary>
/// 讯飞星火服务配置.
/// </summary>
public sealed class SparkClientConfig : ClientConfigBase;

/// <summary>
/// 混元客户端配置.
/// </summary>
public sealed class HunyuanClientConfig : ClientConfigBase;

/// <summary>
/// 豆包大模型配置.
/// </summary>
public sealed class DouBaoClientConfig : ClientConfigBase
{
    /// <inheritdoc/>
    public override bool IsValid()
        => base.IsValid() && IsCustomModelNotEmpty();
}

/// <summary>
/// xAI大模型配置.
/// </summary>
public sealed class XAIClientConfig : ClientConfigBase;

/// <summary>
/// 智谱客户端配置.
/// </summary>
public sealed class ZhiPuClientConfig : ClientConfigBase;

/// <summary>
/// 零一万物客户端配置.
/// </summary>
public sealed class LingYiClientConfig : ClientConfigBase;

/// <summary>
/// 月之暗面客户端配置.
/// </summary>
public sealed class MoonshotClientConfig : ClientConfigBase;

/// <summary>
/// 通义千问客户端配置.
/// </summary>
public sealed class QwenClientConfig : ClientConfigBase;

/// <summary>
/// Gemini 客户端配置.
/// </summary>
public sealed class GeminiClientConfig : ClientEndpointConfigBase;

/// <summary>
/// Groq 客户端配置.
/// </summary>
public sealed class GroqClientConfig : ClientConfigBase;

/// <summary>
/// Mistral AI 客户端配置.
/// </summary>
public sealed class MistralClientConfig : ClientConfigBase
{
    /// <summary>
    /// Codestral 密钥.
    /// </summary>
    public string? CodestralKey { get; set; }

    /// <summary>
    /// 是否使用 Codestral.
    /// </summary>
    public bool UseCodestral { get; set; }
}

/// <summary>
/// Perplexity 客户端配置.
/// </summary>
public sealed class PerplexityClientConfig : ClientConfigBase;

/// <summary>
/// Together AI 客户端配置.
/// </summary>
public sealed class TogetherAIClientConfig : ClientConfigBase;

/// <summary>
/// Open Router 客户端配置.
/// </summary>
public sealed class OpenRouterClientConfig : ClientConfigBase;

/// <summary>
/// DeepSeek 客户端配置.
/// </summary>
public sealed class DeepSeekClientConfig : ClientConfigBase;

/// <summary>
/// Anthropic 客户端配置.
/// </summary>
public sealed class AnthropicClientConfig : ClientEndpointConfigBase;

/// <summary>
/// 硅动客户端配置.
/// </summary>
public sealed class SiliconFlowClientConfig : ClientConfigBase;

/// <summary>
/// Ollama 客户端配置.
/// </summary>
public sealed class OllamaClientConfig : ClientEndpointConfigBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaClientConfig"/> class.
    /// </summary>
    public OllamaClientConfig() => Key = "ollama";

    /// <inheritdoc/>
    public override bool IsValid()
        => IsCustomModelNotEmpty() && !string.IsNullOrEmpty(Endpoint);
}

/// <summary>
/// 配置基类.
/// </summary>
public abstract class ConfigBase
{
    /// <summary>
    /// 自定义模型列表.
    /// </summary>
    [JsonPropertyName("models")]
    public List<ChatModel>? CustomModels { get; set; }

    /// <summary>
    /// 自定义模型是否不为空.
    /// </summary>
    /// <returns>是否不为空.</returns>
    public bool IsCustomModelNotEmpty()
        => CustomModels?.Count > 0;
}

/// <summary>
/// 客户端配置基类.
/// </summary>
public abstract class ClientConfigBase : ConfigBase
{
    /// <summary>
    /// 访问密钥.
    /// </summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>
    /// 是否有效.
    /// </summary>
    /// <returns>配置是否有效.</returns>
    public virtual bool IsValid()
        => !string.IsNullOrEmpty(Key);
}

/// <summary>
/// 客户端终结点配置基类.
/// </summary>
public abstract class ClientEndpointConfigBase : ClientConfigBase
{
    /// <summary>
    /// 终结点.
    /// </summary>
    [JsonPropertyName("endpoint")]
    public string? Endpoint { get; set; }
}
