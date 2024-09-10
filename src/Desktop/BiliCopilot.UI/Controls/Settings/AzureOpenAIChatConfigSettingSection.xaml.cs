// Copyright (c) Bili Copilot. All rights reserved.

using System.Text.Json;
using BiliAgent.Models;
using BiliCopilot.UI.Controls.AI;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace BiliCopilot.UI.Controls.Settings;

/// <summary>
/// Azure OpenAI 聊天配置设置部分.
/// </summary>
public sealed partial class AzureOpenAIChatConfigSettingSection : AIServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureOpenAIChatConfigSettingSection"/> class.
    /// </summary>
    public AzureOpenAIChatConfigSettingSection()
    {
        InitializeComponent();
        InitializeApiVersion();
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        Logo.Provider = ViewModel.ProviderType.ToString();
        ViewModel.Config ??= new AzureOpenAIClientConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        EndpointBox.Text = (ViewModel.Config as ClientEndpointConfigBase)?.Endpoint ?? string.Empty;
        VersionComboBox.SelectedIndex = (int)((AzureOpenAIClientConfig)ViewModel.Config).Version;
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

    private void InitializeApiVersion()
    {
        var versions = Enum.GetValues<AzureOpenAIVersion>();
        var json = JsonSerializer.Serialize(versions);
        VersionComboBox.ItemsSource = JsonSerializer.Deserialize<List<string>>(json);
    }

    private void OnVersionChanged(object sender, SelectionChangedEventArgs e)
    {
        var index = VersionComboBox.SelectedIndex;
        if (index < 0)
        {
            return;
        }

        ((AzureOpenAIClientConfig)ViewModel.Config).Version = (AzureOpenAIVersion)index;
    }

    private void OnPredefinedModelsButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
}
