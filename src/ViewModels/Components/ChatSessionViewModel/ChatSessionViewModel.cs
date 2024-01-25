// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels.Components;

/// <summary>
/// 聊天会话视图模型.
/// </summary>
public sealed partial class ChatSessionViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionViewModel"/> class.
    /// </summary>
    public ChatSessionViewModel()
    {
        Messages = new ObservableCollection<ChatMessageItemViewModel>();
        AttachIsRunningToAsyncCommand(p => IsReloading = p, InitializeCommand, ReloadCommand);
    }

    [RelayCommand]
    private async Task InitializeAsync(UserItemViewModel userVM)
    {
        if (User == userVM)
        {
            return;
        }

        User = userVM;
        await ReloadAsync();
    }

    [RelayCommand]
    private async Task ReloadAsync()
    {
        TryClear(Messages);

        try
        {
            var messages = await AccountProvider.GetChatMessagesAsync(User.Data.Id);
            foreach (var item in messages.Messages)
            {
                var user = item.SenderId == User.Data.Id ? User : default;
                Messages.Add(new ChatMessageItemViewModel(item, user));
            }

            RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
        }
        catch (System.Exception ex)
        {
            LogException(ex);
            AppViewModel.Instance.ShowTip(ex.Message, Models.Constants.App.InfoType.Error);
        }
    }
}
