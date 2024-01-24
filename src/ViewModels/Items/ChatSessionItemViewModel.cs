// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Data.Community;
using CommunityToolkit.Mvvm.ComponentModel;
using Humanizer;

namespace Bili.Copilot.ViewModels.Items;

/// <summary>
/// 聊天会话项视图模型.
/// </summary>
public sealed partial class ChatSessionItemViewModel : SelectableViewModel<ChatSession>
{
    [ObservableProperty]
    private string _sessionTime;

    [ObservableProperty]
    private string _sessionTimeText;

    [ObservableProperty]
    private UserItemViewModel _user;

    [ObservableProperty]
    private int _unreadCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionItemViewModel"/> class.
    /// </summary>
    public ChatSessionItemViewModel(ChatSession data)
        : base(data)
    {
        User = new UserItemViewModel(data.Profile);
        var time = DateTimeOffset.FromUnixTimeSeconds(data.Timestamp);
        SessionTime = time.ToString("yyyy/MM/dd HH:mm");
        SessionTimeText = TextToolkit.ConvertToTraditionalChineseIfNeeded(time.Humanize());
        UnreadCount = data.UnreadCount;
    }
}
