// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Models;
using BiliCopilot.UI.Controls.AI;
using BiliCopilot.UI.Toolkits;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace BiliCopilot.UI.Controls.Settings;

/// <summary>
/// 模型客户端配置设置部分.
/// </summary>
public sealed partial class ModelClientConfigSettingSection : AIServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModelClientConfigSettingSection"/> class.
    /// </summary>
    public ModelClientConfigSettingSection()
    {
        InitializeComponent();
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        Logo.Provider = ViewModel.ProviderType.ToString();
        KeyCard.Description = string.Format(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.AccessKeyDescription), ViewModel.Name);
        KeyBox.PlaceholderText = string.Format(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.AccessKeyPlaceholder), ViewModel.Name);
        PredefinedCard.Description = string.Format(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.PredefinedModelsDescription), ViewModel.Name);

        ViewModel.Config ??= CreateCurrentConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private ClientConfigBase CreateCurrentConfig()
    {
        return ViewModel.ProviderType switch
        {
            ProviderType.OpenAI => (ClientConfigBase)new OpenAIClientConfig(),
            ProviderType.AzureOpenAI => new AzureOpenAIClientConfig(),
            ProviderType.Gemini => new GeminiClientConfig(),
            ProviderType.Anthropic => new AnthropicClientConfig(),
            ProviderType.Moonshot => new MoonshotClientConfig(),
            ProviderType.ZhiPu => new ZhiPuClientConfig(),
            ProviderType.LingYi => new LingYiClientConfig(),
            ProviderType.DeepSeek => new DeepSeekClientConfig(),
            ProviderType.DashScope => new DashScopeClientConfig(),
            ProviderType.QianFan => new QianFanClientConfig(),
            ProviderType.HunYuan => new HunYuanClientConfig(),
            ProviderType.DouBao => new DouBaoClientConfig(),
            ProviderType.SparkDesk => new SparkDeskClientConfig(),
            ProviderType.OpenRouter => new OpenRouterClientConfig(),
            ProviderType.TogetherAI => new TogetherAIClientConfig(),
            ProviderType.Groq => new GroqClientConfig(),
            ProviderType.Perplexity => new PerplexityClientConfig(),
            ProviderType.MistralAI => new MistralAIClientConfig(),
            ProviderType.SiliconFlow => new SiliconFlowClientConfig(),
            ProviderType.Ollama => new OllamaClientConfig(),
            _ => throw new NotImplementedException(),
        };
    }

    private void OnPredefinedModelsButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
}
