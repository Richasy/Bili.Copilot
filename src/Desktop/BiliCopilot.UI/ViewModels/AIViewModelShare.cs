// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using BiliAgent.Models;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;

namespace BiliCopilot.UI.ViewModels;

internal static class AIViewModelShare
{
    public static async Task InitializeOnlineChatServicesAsync(ObservableCollection<AIServiceItemViewModel> onlineChatServices)
    {
        if (onlineChatServices.Count > 0)
        {
            return;
        }

        foreach (var service in Enum.GetValues<ProviderType>())
        {
            var config = await ConfigToolkit.GetChatConfigAsync(service);
            var serviceVM = new AIServiceItemViewModel(service);
            serviceVM.SetConfig(config);
            onlineChatServices.Add(serviceVM);
        }
    }

    public static async Task SaveOnlineChatServicesAsync(ObservableCollection<AIServiceItemViewModel> onlineChatServices)
        => await ConfigToolkit.SaveChatConfigAsync(onlineChatServices.Select(p => (p.ProviderType, p.Config)).ToDictionary());
}
