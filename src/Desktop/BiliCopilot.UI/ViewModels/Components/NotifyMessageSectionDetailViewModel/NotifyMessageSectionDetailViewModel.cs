// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.User;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 通知消息区块详情视图模型.
/// </summary>
public sealed partial class NotifyMessageSectionDetailViewModel : ViewModelBase, IMessageSectionDetailViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyMessageSectionDetailViewModel"/> class.
    /// </summary>
    public NotifyMessageSectionDetailViewModel(
        NotifyMessageType type,
        IMessageService service)
    {
        _service = service;
        _logger = this.Get<ILogger<NotifyMessageSectionDetailViewModel>>();
        Type = type;
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsEmpty = false;
        Items.Clear();
        _offsetId = 0;
        _offsetTime = 0;
        _preventLoadMore = false;
        await LoadItemsAsync();
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Items.Count > 0 || _preventLoadMore)
        {
            return;
        }

        await LoadItemsAsync();
    }

    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        if (IsEmpty || IsLoading || _preventLoadMore)
        {
            return;
        }

        try
        {
            IsLoading = true;
            var (messages, offsetId, offsetTime, hasMore) = await _service.GetNotifyMessagesAsync(Type, _offsetId, _offsetTime);
            _preventLoadMore = messages is null || messages.Count == 0 || !hasMore;
            if (messages != null)
            {
                foreach (var item in messages)
                {
                    Items.Add(new NotifyMessageItemViewModel(item));
                }
            }

            _offsetId = offsetId;
            _offsetTime = offsetTime;
            UnreadCount = 0;
            ListUpdated?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"加载{Type}消息失败.");
        }
        finally
        {
            IsLoading = false;
            IsEmpty = Items.Count == 0;
        }
    }
}
