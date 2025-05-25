// Copyright (c) Bili Copilot. All rights reserved.

using Connectors.DeepSeek.Models;
using Richasy.AgentKernel;
using Richasy.AgentKernel.Connectors.Ali.Models;
using Richasy.AgentKernel.Connectors.Anthropic.Models;
using Richasy.AgentKernel.Connectors.Azure.Models;
using Richasy.AgentKernel.Connectors.Baidu.Models;
using Richasy.AgentKernel.Connectors.Google.Models;
using Richasy.AgentKernel.Connectors.Groq.Models;
using Richasy.AgentKernel.Connectors.IFlyTek.Models;
using Richasy.AgentKernel.Connectors.LingYi.Models;
using Richasy.AgentKernel.Connectors.Mistral.Models;
using Richasy.AgentKernel.Connectors.Moonshot.Models;
using Richasy.AgentKernel.Connectors.Ollama.Models;
using Richasy.AgentKernel.Connectors.OpenAI.Models;
using Richasy.AgentKernel.Connectors.OpenRouter.Models;
using Richasy.AgentKernel.Connectors.SiliconFlow.Models;
using Richasy.AgentKernel.Connectors.Tencent.Models;
using Richasy.AgentKernel.Connectors.TogetherAI.Models;
using Richasy.AgentKernel.Connectors.Volcano.Models;
using Richasy.AgentKernel.Connectors.XAI.Models;
using Richasy.AgentKernel.Connectors.ZhiPu.Models;
using Richasy.AgentKernel.Models;
using Richasy.WinUIKernel.Share.Toolkits;
using System.Diagnostics.CodeAnalysis;

namespace BiliCopilot.UI.Extensions;

/// <summary>
/// Chat service configuration manager.
/// </summary>
public sealed class ChatConfigManager : ChatConfigManagerBase
{
    /// <inheritdoc/>
    protected override AIServiceConfig? ConvertToConfig(ChatClientConfigBase? config)
    {
        return config switch
        {
            OpenAIChatConfig openAIConfig => openAIConfig.ToAIServiceConfig(),
            AzureOpenAIChatConfig azureOaiConfig => azureOaiConfig.ToAIServiceConfig<AzureOpenAIServiceConfig>(),
            AzureAIChatConfig azureConfig => azureConfig.ToAIServiceConfig<AzureOpenAIServiceConfig>(),
            XAIChatConfig xaiConfig => xaiConfig.ToAIServiceConfig<XAIServiceConfig>(),
            ZhiPuChatConfig zhiPuConfig => zhiPuConfig.ToAIServiceConfig<ZhiPuServiceConfig>(),
            LingYiChatConfig lingYiConfig => lingYiConfig.ToAIServiceConfig<LingYiServiceConfig>(),
            AnthropicChatConfig anthropicConfig => anthropicConfig.ToAIServiceConfig<AnthropicServiceConfig>(),
            MoonshotChatConfig moonshotConfig => moonshotConfig.ToAIServiceConfig<MoonshotServiceConfig>(),
            GeminiChatConfig geminiConfig => geminiConfig.ToAIServiceConfig<GeminiServiceConfig>(),
            DeepSeekChatConfig deepSeekConfig => deepSeekConfig.ToAIServiceConfig<DeepSeekServiceConfig>(),
            QwenChatConfig qwenConfig => qwenConfig.ToAIServiceConfig<QwenServiceConfig>(),
            ErnieChatConfig ernieConfig => ernieConfig.ToAIServiceConfig(),
            HunyuanChatConfig hunyuanConfig => hunyuanConfig.ToAIServiceConfig<HunyuanChatServiceConfig>(),
            SparkChatConfig sparkConfig => sparkConfig.ToAIServiceConfig<SparkChatServiceConfig>(),
            DoubaoChatConfig douBaoConfig => douBaoConfig.ToAIServiceConfig<DoubaoServiceConfig>(),
            SiliconFlowChatConfig siliconFlowConfig => siliconFlowConfig.ToAIServiceConfig<SiliconFlowServiceConfig>(),
            OpenRouterChatConfig openRouterConfig => openRouterConfig.ToAIServiceConfig<OpenRouterServiceConfig>(),
            TogetherAIChatConfig togetherAIConfig => togetherAIConfig.ToAIServiceConfig<TogetherAIServiceConfig>(),
            GroqChatConfig groqConfig => groqConfig.ToAIServiceConfig<GroqServiceConfig>(),
            MistralChatConfig mistralConfig => mistralConfig.ToAIServiceConfig(),
            OllamaChatConfig ollamaConfig => ollamaConfig.ToAIServiceConfig(),
            _ => default,
        };
    }

    /// <inheritdoc/>
    protected override async Task<ChatClientConfiguration> OnInitializeAsync()
    {
        var config = await this.Get<IFileToolkit>().ReadLocalDataAsync($"{nameof(ChatClientConfiguration)}.json", GlobalSerializeContext.Default.ChatClientConfiguration);
        if (config is null)
        {
            config = new ChatClientConfiguration();
            await this.Get<IFileToolkit>().WriteLocalDataAsync($"{nameof(ChatClientConfiguration)}.json", config!, GlobalSerializeContext.Default.ChatClientConfiguration);
        }

        return config;
    }

    /// <inheritdoc/>
    protected override Task OnSaveAsync(ChatClientConfiguration configuration)
        => this.Get<IFileToolkit>().WriteLocalDataAsync($"{nameof(ChatClientConfiguration)}.json", configuration, GlobalSerializeContext.Default.ChatClientConfiguration);
}

internal static partial class ConfigExtensions
{
    public static AIServiceConfig? ToAIServiceConfig(this OpenAIChatConfig? config)
    {
        var endpoint = string.IsNullOrEmpty(config?.Endpoint) ? null : new Uri(config.Endpoint);
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? default
            : new OpenAIServiceConfig(config.Key, string.Empty, endpoint, config.OrganizationId);
    }

    public static AIServiceConfig? ToAIServiceConfig<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TAIServiceConfig>(this ChatEndpointConfigBase? config)
        where TAIServiceConfig : AIServiceConfig
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? default
            : Activator.CreateInstance(typeof(TAIServiceConfig), config.Key, string.Empty, string.IsNullOrEmpty(config.Endpoint) ? default : new Uri(config.Endpoint)) as TAIServiceConfig;
    }

    public static AIServiceConfig? ToAIServiceConfig<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TAIServiceConfig>(this ChatClientConfigBase? config)
        where TAIServiceConfig : AIServiceConfig
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? default
            : Activator.CreateInstance(typeof(TAIServiceConfig), config.Key, string.Empty) as TAIServiceConfig;
    }

    public static AIServiceConfig? ToAIServiceConfig(this OllamaChatConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Endpoint)
             ? default
             : new OllamaServiceConfig(string.Empty, new Uri(config.Endpoint));
    }

    public static AIServiceConfig? ToAIServiceConfig(this ErnieChatConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? default
            : new ErnieServiceConfig(config.Key, config.Secret!, string.Empty);
    }

    public static AIServiceConfig? ToAIServiceConfig(this MistralChatConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? default
            : new MistralServiceConfig(config.UseCodestral ? config.CodestralKey! : config.Key!, string.Empty, config.UseCodestral);
    }
}
