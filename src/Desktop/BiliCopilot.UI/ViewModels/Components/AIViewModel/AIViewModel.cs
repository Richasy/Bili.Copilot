// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliAgent.Models;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// AI 视图模型.
/// </summary>
public sealed partial class AIViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AIViewModel"/> class.
    /// </summary>
    public AIViewModel(
        IAgentClient client,
        ILogger<AIViewModel> logger)
    {
        _client = client;
        _logger = logger;
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Services is null)
        {
            await ReloadAvailableServicesAsync();
        }
    }

    [RelayCommand]
    private void Clear()
    {
        SelectedService = default;
        SelectedModel = default;
        if (Services is List<AIServiceItemViewModel> services)
        {
            services.Clear();
            Services = default;
        }

        if (Models is List<ChatModelItemViewModel> models)
        {
            models.Clear();
            Models = default;
        }
    }

    [RelayCommand]
    private async Task ReloadAvailableServicesAsync()
    {
        await ConfigToolkit.ResetConfigFactoryAsync();
        var providers = Enum.GetValues<ProviderType>();
        var services = new List<AIServiceItemViewModel>();
        foreach (var p in providers)
        {
            var service = await ConfigToolkit.GetChatConfigAsync(p);
            if (service.IsValid())
            {
                services.Add(new AIServiceItemViewModel(p));
            }
        }

        Services = services;
        IsNoService = services.Count == 0;

        if (!IsNoService)
        {
            var lastSelectService = SettingsToolkit.ReadLocalSetting(UI.Models.Constants.SettingNames.LastSelectedAIService, ProviderType.OpenAI.ToString());
            var service = Services.FirstOrDefault(p => p.ProviderType.ToString() == lastSelectService) ?? Services.FirstOrDefault();
            SelectService(service);
        }
    }

    [RelayCommand]
    private void SelectService(AIServiceItemViewModel service)
    {
        if (SelectedService == service)
        {
            return;
        }

        SelectedService = service;
        if (service is null)
        {
            Models = default;
            IsNoModel = true;
            return;
        }

        SettingsToolkit.WriteLocalSetting(UI.Models.Constants.SettingNames.LastSelectedAIService, service.ProviderType.ToString());
        var models = _client.GetModels(service.ProviderType);
        Models = models.Select(p => new ChatModelItemViewModel(p)).ToList();
        IsNoModel = Models.Count == 0;
        if (!IsNoModel)
        {
            var lastSelectModel = SettingsToolkit.ReadLocalSetting($"LastSelected{service.ProviderType}Model", string.Empty);
            var model = Models.FirstOrDefault(p => p.Id == lastSelectModel) ?? Models.FirstOrDefault();
            SelectModel(model);
        }
        else
        {
            SelectedModel = default;
        }
    }

    [RelayCommand]
    private void SelectModel(ChatModelItemViewModel model)
    {
        if (model is null || SelectedModel == model)
        {
            return;
        }

        SelectedModel = model;
        SettingsToolkit.WriteLocalSetting($"LastSelected{SelectedService.ProviderType}Model", model.Id);
    }
}
