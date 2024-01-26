// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Data.Community;
using CommunityToolkit.Mvvm.ComponentModel;
using Humanizer;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

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

    [ObservableProperty]
    private string _lastMessage;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionItemViewModel"/> class.
    /// </summary>
    public ChatSessionItemViewModel(ChatSession data)
        : base(data)
    {
        User = new UserItemViewModel(data.Profile);
        var time = DateTimeOffset.FromUnixTimeSeconds(data.Timestamp);
        SessionTime = time.ToLocalTime().ToString("yyyy/MM/dd HH:mm");
        SessionTimeText = TextToolkit.ConvertToTraditionalChineseIfNeeded(time.Humanize());
        UnreadCount = data.UnreadCount;
        LastMessage = data.LastMessage;
    }

    /// <summary>
    /// 更新.
    /// </summary>
    /// <param name="content">最后一条消息.</param>
    public void Update(string content)
    {
        LastMessage = content;
        var time = DateTimeOffset.Now;
        SessionTime = time.ToString("yyyy/MM/dd HH:mm");
        SessionTimeText = TextToolkit.ConvertToTraditionalChineseIfNeeded(time.Humanize());
    }
}
