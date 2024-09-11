// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
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
    public OpenAIClientConfig? OpenAI { get; set; }

    /// <summary>
    /// Azure Open AI 客户端配置.
    /// </summary>
    [JsonPropertyName("azure_openai")]
    public AzureOpenAIClientConfig? AzureOpenAI { get; set; }

    /// <summary>
    /// 智谱客户端配置.
    /// </summary>
    [JsonPropertyName("zhipu")]
    public ZhiPuClientConfig? ZhiPu { get; set; }

    /// <summary>
    /// 零一万物客户端配置.
    /// </summary>
    [JsonPropertyName("lingyi")]
    public LingYiClientConfig? LingYi { get; set; }

    /// <summary>
    /// 月之暗面客户端配置.
    /// </summary>
    [JsonPropertyName("moonshot")]
    public MoonshotClientConfig? Moonshot { get; set; }

    /// <summary>
    /// 阿里灵积客户端配置.
    /// </summary>
    [JsonPropertyName("dash_scope")]
    public DashScopeClientConfig? DashScope { get; set; }

    /// <summary>
    /// DeepSeek 客户端配置.
    /// </summary>
    [JsonPropertyName("deep_seek")]
    public DeepSeekClientConfig? DeepSeek { get; set; }

    /// <summary>
    /// 千帆客户端配置.
    /// </summary>
    [JsonPropertyName("qianfan")]
    public QianFanClientConfig? QianFan { get; set; }

    /// <summary>
    /// 讯飞星火客户端配置.
    /// </summary>
    [JsonPropertyName("spark_desk")]
    public SparkDeskClientConfig? SparkDesk { get; set; }

    /// <summary>
    /// Gemini 客户端配置.
    /// </summary>
    [JsonPropertyName("gemini")]
    public GeminiClientConfig? Gemini { get; set; }

    /// <summary>
    /// Groq 客户端配置.
    /// </summary>
    [JsonPropertyName("groq")]
    public GroqClientConfig? Groq { get; set; }

    /// <summary>
    /// Mistral AI 客户端配置.
    /// </summary>
    [JsonPropertyName("mistral_ai")]
    public MistralAIClientConfig? MistralAI { get; set; }

    /// <summary>
    /// Perplexity 客户端配置.
    /// </summary>
    [JsonPropertyName("perplexity")]
    public PerplexityClientConfig? Perplexity { get; set; }

    /// <summary>
    /// Together AI 客户端配置.
    /// </summary>
    [JsonPropertyName("together_ai")]
    public TogetherAIClientConfig? TogetherAI { get; set; }

    /// <summary>
    /// Open Router 客户端配置.
    /// </summary>
    [JsonPropertyName("open_router")]
    public OpenRouterClientConfig? OpenRouter { get; set; }

    /// <summary>
    /// Anthropic 客户端配置.
    /// </summary>
    [JsonPropertyName("anthropic")]
    public AnthropicClientConfig? Anthropic { get; set; }

    /// <summary>
    /// Ollama 客户端配置.
    /// </summary>
    [JsonPropertyName("ollama")]
    public OllamaClientConfig? Ollama { get; set; }

    /// <summary>
    /// 混元客户端配置.
    /// </summary>
    [JsonPropertyName("hunyuan")]
    public HunYuanClientConfig? HunYuan { get; set; }

    /// <summary>
    /// 硅动客户端配置.
    /// </summary>
    [JsonPropertyName("silicon_flow")]
    public SiliconFlowClientConfig? SiliconFlow { get; set; }

    /// <summary>
    /// 豆包客户端配置.
    /// </summary>
    [JsonPropertyName("doubao")]
    public DouBaoClientConfig? DouBao { get; set; }
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
    /// <summary>
    /// 版本.
    /// </summary>
    [JsonPropertyName("version")]
    public AzureOpenAIVersion Version { get; set; } = AzureOpenAIVersion.V2024_02_01;

    /// <inheritdoc/>
    public override bool IsValid()
    {
        return base.IsValid()
            && !string.IsNullOrEmpty(Endpoint)
            && CustomModels != null
            && CustomModels.Count > 0;
    }
}

/// <summary>
/// 千帆客户端配置.
/// </summary>
public class QianFanClientConfig : ClientConfigBase
{
    /// <summary>
    /// 密匙.
    /// </summary>
    [JsonPropertyName("secret")]
    public string Secret { get; set; }

    /// <inheritdoc/>
    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(Secret);
}

/// <summary>
/// 讯飞星火服务配置.
/// </summary>
public sealed class SparkDeskClientConfig : QianFanClientConfig
{
    /// <summary>
    /// 应用程序 ID.
    /// </summary>
    [JsonPropertyName("app_id")]
    public string AppId { get; set; }

    /// <inheritdoc/>
    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(AppId);
}

/// <summary>
/// 混元客户端配置.
/// </summary>
public sealed class HunYuanClientConfig : ClientConfigBase
{
    /// <summary>
    /// 密匙 ID.
    /// </summary>
    [JsonPropertyName("secret_id")]
    public string SecretId { get; set; }

    /// <inheritdoc/>
    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(SecretId);
}

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
/// 智谱客户端配置.
/// </summary>
public sealed class ZhiPuClientConfig : ClientConfigBase
{
}

/// <summary>
/// 零一万物客户端配置.
/// </summary>
public sealed class LingYiClientConfig : ClientConfigBase
{
}

/// <summary>
/// 月之暗面客户端配置.
/// </summary>
public sealed class MoonshotClientConfig : ClientConfigBase
{
}

/// <summary>
/// 阿里灵积客户端配置.
/// </summary>
public sealed class DashScopeClientConfig : ClientConfigBase
{
}

/// <summary>
/// Gemini 客户端配置.
/// </summary>
public sealed class GeminiClientConfig : ClientEndpointConfigBase
{
}

/// <summary>
/// Groq 客户端配置.
/// </summary>
public sealed class GroqClientConfig : ClientConfigBase
{
}

/// <summary>
/// Mistral AI 客户端配置.
/// </summary>
public sealed class MistralAIClientConfig : ClientConfigBase
{
}

/// <summary>
/// Perplexity 客户端配置.
/// </summary>
public sealed class PerplexityClientConfig : ClientConfigBase
{
}

/// <summary>
/// Together AI 客户端配置.
/// </summary>
public sealed class TogetherAIClientConfig : ClientConfigBase
{
}

/// <summary>
/// Open Router 客户端配置.
/// </summary>
public sealed class OpenRouterClientConfig : ClientConfigBase
{
}

/// <summary>
/// DeepSeek 客户端配置.
/// </summary>
public sealed class DeepSeekClientConfig : ClientConfigBase
{
}

/// <summary>
/// Anthropic 客户端配置.
/// </summary>
public sealed class AnthropicClientConfig : ClientEndpointConfigBase
{
}

/// <summary>
/// 硅动客户端配置.
/// </summary>
public sealed class SiliconFlowClientConfig : ClientConfigBase
{
}

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
        => CustomModels != null && CustomModels.Count > 0;
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
