// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Community;
using Bili.Copilot.ViewModels.Components;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 消息页面视图模型.
/// </summary>
public sealed partial class MessageDetailViewModel : InformationFlowViewModel<MessageItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageDetailViewModel"/> class.
    /// </summary>
    private MessageDetailViewModel()
    {
        _caches = new Dictionary<MessageType, (IEnumerable<MessageInformation> Items, bool IsEnd)>();
        NavListColumnWidth = SettingsToolkit.ReadLocalSetting(SettingNames.MessageNavListColumnWidth, 280d);
        MessageTypes = new ObservableCollection<MessageHeaderViewModel>
        {
            GetMessageHeader(MessageType.Reply),
            GetMessageHeader(MessageType.At),
            GetMessageHeader(MessageType.Like),
        };

        InitializeMessageCount();
        AccountViewModel.Instance.PropertyChanged += OnAccountViewModelPropertyChanged;
    }

    /// <inheritdoc/>
    protected override void BeforeReload()
    {
        AccountProvider.Instance.ClearMessageStatus();
        if (_shouldClearCache)
        {
            _caches.Clear();
        }

        _isEnd = false;
        IsEmpty = false;
        _shouldClearCache = true;
    }

    /// <inheritdoc/>
    protected override async Task GetDataAsync()
    {
        if (_isEnd)
        {
            return;
        }

        if (CurrentType == default)
        {
            SelectTypeCommand.Execute(MessageTypes.First());
        }

        var view = await AccountProvider.Instance.GetMyMessagesAsync(CurrentType.Type);
        _isEnd = view.IsFinished;
        foreach (var item in view.Messages)
        {
            var messageVM = new MessageItemViewModel(item);
            Items.Add(messageVM);
        }

        _caches.Remove(CurrentType.Type);
        _caches.Add(CurrentType.Type, new(Items.Select(p => p.Data).ToList(), _isEnd));
        IsEmpty = Items.Count == 0;
        AccountViewModel.Instance.InitializeUnreadCommand.Execute(default);
    }

    /// <inheritdoc/>
    protected override string FormatException(string errorMsg)
        => $"{ResourceToolkit.GetLocalizedString(StringNames.RequestMessageFailed)}\n{errorMsg}";

    private static MessageHeaderViewModel GetMessageHeader(MessageType type)
    {
        var vm = new MessageHeaderViewModel(type);
        return vm;
    }

    [RelayCommand]
    private void SelectType(MessageHeaderViewModel type)
    {
        TryClear(Items);
        _isEnd = false;
        CurrentType = type;
        foreach (var item in MessageTypes)
        {
            item.IsSelected = type.Equals(item);
        }

        if (IsInChatSession)
        {
            ExitChatSession();
        }

        if (_caches.TryGetValue(CurrentType.Type, out var data) && data.Items.Count() > 0)
        {
            foreach (var item in data.Items)
            {
                var messageVM = new MessageItemViewModel(item);
                Items.Add(messageVM);
            }

            _isEnd = data.IsEnd;
            IsEmpty = Items.Count == 0;
        }
        else
        {
            _shouldClearCache = false;
            _ = InitializeCommand.ExecuteAsync(null);
        }
    }

    [RelayCommand]
    private void EnterChatSession()
    {
        IsInChatSession = true;
        foreach (var item in MessageTypes)
        {
            item.IsSelected = false;
        }
    }

    [RelayCommand]
    private void ExitChatSession()
    {
        IsInChatSession = false;
        foreach (var item in MessageTypes)
        {
            item.IsSelected = CurrentType.Equals(item);
        }

        foreach (var item in ChatSessionListModuleViewModel.Instance.Items)
        {
            item.IsSelected = false;
        }

        ChatSessionListModuleViewModel.Instance.SelectedSession = default;
    }

    private void InitializeMessageCount()
    {
        var unreadInfo = AccountViewModel.Instance.UnreadInformation;
        if (unreadInfo != null)
        {
            MessageTypes.First(p => p.Type == MessageType.Reply).Count = unreadInfo.ReplyCount;
            MessageTypes.First(p => p.Type == MessageType.At).Count = unreadInfo.AtCount;
            MessageTypes.First(p => p.Type == MessageType.Like).Count = unreadInfo.LikeCount;
        }
        else
        {
            foreach (var item in MessageTypes)
            {
                item.Count = 0;
            }
        }
    }

    private void OnAccountViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AccountViewModel.Instance.UnreadInformation))
        {
            InitializeMessageCount();
        }
    }

    partial void OnNavListColumnWidthChanged(double value)
    {
        if (value >= 240)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.MessageNavListColumnWidth, value);
        }
    }
}
