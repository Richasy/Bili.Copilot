// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Humanizer;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.User;
using Richasy.WinUI.Share.Base;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 聊天消息区块详情视图模型.
/// </summary>
public sealed partial class ChatMessageSectionDetailViewModel : ViewModelBase<ChatSession>, IMessageSectionDetailViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatMessageSectionDetailViewModel"/> class.
    /// </summary>
    public ChatMessageSectionDetailViewModel(
        ChatSession session)
        : base(session)
    {
        _service = this.Get<IMessageService>();
        _logger = this.Get<ILogger<ChatMessageSectionDetailViewModel>>();

        Avatar = session.User?.Avatar?.Uri;
        UserName = session.User?.Name;
        UnreadCount = session.UnreadCount;
        LastMessage = Data.LastMessage;
        LastMessageTime = Data.Time.Humanize(default, new System.Globalization.CultureInfo("zh-CN"));
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Messages?.Count > 0 || _preventLoadMore || IsEmpty)
        {
            return;
        }

        Messages = new();
        await LoadMessagesAsync();
    }

    [RelayCommand]
    private void ShowUserSpace()
        => this.Get<NavigationViewModel>().NavigateToOver(typeof(UserSpacePage), Data.User);

    [RelayCommand]
    private async Task RefreshAsync()
    {
        Messages = new();
        _preventLoadMore = false;
        IsEmpty = false;
        await LoadMessagesAsync();
    }

    [RelayCommand]
    private async Task SendMessageAsync()
    {
        if (string.IsNullOrEmpty(InputText) || IsSending)
        {
            return;
        }

        try
        {
            IsSending = true;
            var now = DateTimeOffset.Now;
            var msg = await _service.SendChatMessageAsync(InputText, Data.User);
            if (msg is not null)
            {
                Messages.Add(new ChatMessageItemViewModel(msg));
                InputText = string.Empty;
            }

            LastMessage = msg.Content.Text;
            LastMessageTime = now.Humanize(default, new System.Globalization.CultureInfo("zh-CN"));
            RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送消息时失败");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.FailedToSendMessage), InfoType.Error));
        }
        finally
        {
            IsSending = false;
            IsEmpty = Messages.Count == 0;
        }
    }

    private async Task LoadMessagesAsync()
    {
        if (IsLoading || _preventLoadMore)
        {
            return;
        }

        try
        {
            IsLoading = true;
            var (messages, maxCount, hasMore) = await _service.GetChatMessagesAsync(Data.User);
            if (messages is not null)
            {
                foreach (var item in messages)
                {
                    Messages.Add(new ChatMessageItemViewModel(item));
                }

                if (LastMessage != messages.Last().Content.Text)
                {
                    LastMessage = messages.Last().Content.Text;
                    LastMessageTime = messages.Last().Time.Humanize(default, new System.Globalization.CultureInfo("zh-CN"));
                }

                RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                _preventLoadMore = true;
                IsEmpty = true;
            }
        }
        catch (Exception ex)
        {
            _preventLoadMore = true;
            _logger.LogError(ex, "加载聊天消息时失败");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.FailedToLoadChatMessages), InfoType.Error));
        }
        finally
        {
            IsLoading = false;
            IsEmpty = Messages.Count == 0;
        }
    }
}
