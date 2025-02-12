using Richasy.AgentKernel;
using Richasy.WinUIKernel.AI.ViewModels;

namespace BiliCopilot.UI.ViewModels.View;

public sealed partial class SettingsPageViewModel
{
    /// <inheritdoc/>
    public override async Task InitializeChatServicesAsync()
    {
        await base.InitializeChatServicesAsync();
        if (ChatServices.Count > 0)
        {
            return;
        }

        foreach (var provider in Enum.GetValues<ChatProviderType>())
        {
            ChatServices.Add(new ChatServiceItemViewModel(provider));
        }
    }

    /// <inheritdoc/>
    protected override async Task SaveChatServicesAsync()
    {
        await base.SaveChatServicesAsync();
        var configManager = this.Get<IChatConfigManager>();
        var dict = ChatServices.Where(p => p.Config != null).ToDictionary(item => item.ProviderType, item => item.Config!);
        await configManager.SaveChatConfigAsync(dict);
    }
}
