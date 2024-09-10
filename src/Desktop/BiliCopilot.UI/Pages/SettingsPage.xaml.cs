﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Models;
using BiliCopilot.UI.Controls.AI;
using BiliCopilot.UI.Controls.Settings;
using BiliCopilot.UI.ViewModels.Items;
using BiliCopilot.UI.ViewModels.View;
using Microsoft.UI.Xaml.Controls.Primitives;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Pages;

/// <summary>
/// 设置页面.
/// </summary>
public sealed partial class SettingsPage : SettingsPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPage"/> class.
    /// </summary>
    public SettingsPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        ViewModel.InitializeCommand.Execute(default);
        SectionSelector.SelectedItem = SectionSelector.Items[0];
    }

    /// <inheritdoc/>
    protected override void OnPageUnloaded()
        => ViewModel.SaveOnlineChatServicesCommand.Execute(default);

    private static FrameworkElement CreateForm<TControl>(AIServiceItemViewModel vm)
    {
        var control = Activator.CreateInstance<TControl>();
        if (control is AIServiceConfigControlBase form)
        {
            form.ViewModel = vm;
        }

        return control as FrameworkElement;
    }

    private void OnJoinGroupButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);

    private async void OnSectionSelectorChangedAsync(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        var index = Convert.ToInt32(sender.SelectedItem.Tag);
        if (index == 0)
        {
            GenericContainer.Visibility = Visibility.Visible;
            AIContainer.Visibility = Visibility.Collapsed;
        }
        else
        {
            GenericContainer.Visibility = Visibility.Collapsed;
            AIContainer.Visibility = Visibility.Visible;
            if (AIPanel.Children.Count == 0)
            {
                await LoadAIServicesAsync();
            }
        }
    }

    private async Task LoadAIServicesAsync()
    {
        await ViewModel.InitializeOnlineChatServicesCommand.ExecuteAsync(default);
        foreach (var item in ViewModel.OnlineChatServices)
        {
            var section = item.ProviderType switch
            {
                ProviderType.Moonshot
                or ProviderType.ZhiPu
                or ProviderType.LingYi
                or ProviderType.DeepSeek
                or ProviderType.OpenRouter
                or ProviderType.Groq
                or ProviderType.MistralAI
                or ProviderType.TogetherAI
                or ProviderType.Perplexity
                or ProviderType.SiliconFlow
                or ProviderType.DashScope => CreateForm<ModelClientConfigSettingSection>(item),
                ProviderType.Anthropic
                or ProviderType.Gemini
                or ProviderType.Ollama => CreateForm<ModelClientEndpointConfigSettingSection>(item),
                ProviderType.OpenAI => CreateForm<OpenAIChatConfigSettingSection>(item),
                ProviderType.AzureOpenAI => CreateForm<AzureOpenAIChatConfigSettingSection>(item),
                ProviderType.QianFan => CreateForm<QianFanChatConfigSettingSection>(item),
                ProviderType.HunYuan => CreateForm<HunYuanChatConfigSettingSection>(item),
                ProviderType.SparkDesk => CreateForm<SparkDeskChatConfigSettingSection>(item),
                ProviderType.DouBao => CreateForm<DouBaoChatConfigSettingSection>(item),
                _ => throw new NotImplementedException(),
            };

            if (section is not null)
            {
                AIPanel.Children.Add(section);
            }
        }
    }
}

/// <summary>
/// 设置页面基类.
/// </summary>
public abstract class SettingsPageBase : LayoutPageBase<SettingsPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPageBase"/> class.
    /// </summary>
    protected SettingsPageBase() => ViewModel = this.Get<SettingsPageViewModel>();
}
