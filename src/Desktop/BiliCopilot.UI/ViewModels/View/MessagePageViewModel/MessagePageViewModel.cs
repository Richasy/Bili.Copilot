﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.User;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 消息页面视图模型.
/// </summary>
public sealed partial class MessagePageViewModel : LayoutPageViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessagePageViewModel"/> class.
    /// </summary>
    public MessagePageViewModel(
        IMessageService service,
        ILogger<MessagePageViewModel> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <inheritdoc/>
    protected override string GetPageKey() => nameof(MessagePage);

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Sections.Count > 0)
        {
            RestoreSelection();
            SectionInitialized?.Invoke(this, EventArgs.Empty);
            return;
        }

        IsLoading = true;
        Sections.Add(new NotifyMessageSectionDetailViewModel(NotifyMessageType.Reply, _service));
        Sections.Add(new NotifyMessageSectionDetailViewModel(NotifyMessageType.At, _service));
        Sections.Add(new NotifyMessageSectionDetailViewModel(NotifyMessageType.Like, _service));
        await LoadChatSessionsAsync();
        IsLoading = false;
        RestoreSelection();
        SectionInitialized?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        Sections.Clear();
        _chatSessionOffset = default;
        _preventLoadMore = false;
        await InitializeAsync();
    }

    [RelayCommand]
    private async Task SelectSectionAsync(IMessageSectionDetailViewModel vm)
    {
        if (vm is null || vm == CurrentSection)
        {
            return;
        }

        var sectionSettingValue = string.Empty;
        if (vm is NotifyMessageSectionDetailViewModel nfVM)
        {
            sectionSettingValue = $"nf_{nfVM.Type}";
        }
        else if (vm is ChatMessageSectionDetailViewModel chatVM)
        {
            sectionSettingValue = $"chat_{chatVM.Data.User.Id}";
        }

        SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.LastSelectedMessageSection, sectionSettingValue);
        CurrentSection = vm;
        await CurrentSection.InitializeCommand.ExecuteAsync(default);
        this.Get<AccountViewModel>().UpdateUnreadCommand.Execute(default);
    }

    [RelayCommand]
    private async Task UpdateUnreadCountAsync()
    {
        try
        {
            var unread = await _service.GetUnreadInformationAsync();
            foreach (var item in Sections.OfType<NotifyMessageSectionDetailViewModel>().ToList())
            {
                if (item.Type == NotifyMessageType.Like)
                {
                    item.UnreadCount = unread.LikeCount;
                }
                else if (item.Type == NotifyMessageType.Reply)
                {
                    item.UnreadCount = unread.ReplyCount;
                }
                else if (item.Type == NotifyMessageType.At)
                {
                    item.UnreadCount = unread.AtCount;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "无法获取未读消息数量.");
        }
    }

    [RelayCommand]
    private async Task LoadChatSessionsAsync()
    {
        if (IsChatSessionLoading || _preventLoadMore)
        {
            return;
        }

        try
        {
            IsChatSessionLoading = true;
            var (sessions, offset, hasMore) = await _service.GetChatSessionsAsync(_chatSessionOffset);
            _preventLoadMore = !hasMore;
            _chatSessionOffset = offset;
            if (sessions is not null)
            {
                foreach (var item in sessions)
                {
                    Sections.Add(new ChatMessageSectionDetailViewModel(item));
                }

                ChatSessionsUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception ex)
        {
            _preventLoadMore = true;
            _logger.LogError(ex, "加载聊天会话时失败.");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.FailedToLoadChatSessions), InfoType.Error));
        }
        finally
        {
            IsChatSessionLoading = false;
        }
    }

    private void RestoreSelection()
    {
        var lastSelectedSection = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.LastSelectedMessageSection, string.Empty);
        UpdateUnreadCountCommand.Execute(default);
        if (string.IsNullOrEmpty(lastSelectedSection))
        {
            SelectSectionCommand.Execute(Sections.First());
            return;
        }

        var isSelected = false;
        if (lastSelectedSection.StartsWith("nf"))
        {
            var isSuccess = Enum.TryParse<NotifyMessageType>(lastSelectedSection.Replace("nf_", string.Empty), out var type);
            if (isSuccess)
            {
                var section = Sections.OfType<NotifyMessageSectionDetailViewModel>().FirstOrDefault(p => p.Type == type);
                if (section is not null)
                {
                    isSelected = true;
                    SelectSectionCommand.Execute(section);
                }
            }
        }
        else if (lastSelectedSection.StartsWith("chat"))
        {
            var id = lastSelectedSection.Replace("chat_", string.Empty);
            var section = Sections.OfType<ChatMessageSectionDetailViewModel>().FirstOrDefault(p => p.Data.User.Id == id);
            if (section is not null)
            {
                isSelected = true;
                SelectSectionCommand.Execute(section);
            }
        }

        if (!isSelected && CurrentSection is null)
        {
            SelectSectionCommand.Execute(Sections.First());
        }
    }
}
