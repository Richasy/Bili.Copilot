// Copyright (c) Bili Copilot. All rights reserved.

using System.Reflection;
using BiliAgent.Models;
using BiliCopilot.UI.Controls.AI;
using BiliCopilot.UI.Toolkits;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace BiliCopilot.UI.Controls.Settings;

/// <summary>
/// 模型客户端端点配置设置部分.
/// </summary>
public sealed partial class ModelClientEndpointConfigSettingSection : AIServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModelClientEndpointConfigSettingSection"/> class.
    /// </summary>
    public ModelClientEndpointConfigSettingSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        Logo.Provider = ViewModel.ProviderType.ToString();

        // TODO: 也许可以调整图片.
        if (ViewModel.ProviderType == ProviderType.Anthropic)
        {
            Logo.Height = 16;
        }

        KeyCard.Description = string.Format(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.AccessKeyDescription), ViewModel.Name);
        KeyBox.PlaceholderText = string.Format(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.AccessKeyPlaceholder), ViewModel.Name);
        PredefinedCard.Description = string.Format(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.PredefinedModelsDescription), ViewModel.Name);

        ViewModel.Config ??= CreateCurrentConfig();
        if (ViewModel.Config is OllamaClientConfig)
        {
            KeyCard.Visibility = Visibility.Collapsed;
        }

        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        EndpointBox.Text = (ViewModel.Config as ClientEndpointConfigBase)?.Endpoint ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnEndpointBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((ClientEndpointConfigBase)ViewModel.Config).Endpoint = EndpointBox.Text;
        ViewModel.CheckCurrentConfig();
    }

    private ClientConfigBase CreateCurrentConfig()
    {
        var assembly = Assembly.GetAssembly(typeof(ClientEndpointConfigBase));
        var types = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ClientEndpointConfigBase)));

        foreach (var type in types)
        {
            if (type.Name.StartsWith(ViewModel.ProviderType.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                var config = (ClientEndpointConfigBase)Activator.CreateInstance(type);
                if (config is AnthropicClientConfig)
                {
                    config.Endpoint = ProviderConstants.AnthropicApi;
                }
                else if (config is GeminiClientConfig)
                {
                    config.Endpoint = ProviderConstants.GeminiApi;
                }
                else if (config is OllamaClientConfig)
                {
                    config.Endpoint = ProviderConstants.OllamaApi;
                }

                return config;
            }
        }

        return null;
    }

    private void OnPredefinedModelsButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
}
