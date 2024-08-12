// Copyright (c) Bili Copilot. All rights reserved.

using Humanizer;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Models.User;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 对话消息条目视图模型.
/// </summary>
public sealed partial class ChatMessageItemViewModel : ViewModelBase<ChatMessage>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatMessageItemViewModel"/> class.
    /// </summary>
    public ChatMessageItemViewModel(ChatMessage data)
        : base(data)
    {
        RelativeTime = data.Time.Humanize();
        ActualTime = data.Time?.ToString("yyyy/MM/dd HH:mm:ss");
        Content = data.Content;
        IsMe = data.SenderId == this.Get<IBiliTokenResolver>().GetToken().UserId.ToString();
    }
}
