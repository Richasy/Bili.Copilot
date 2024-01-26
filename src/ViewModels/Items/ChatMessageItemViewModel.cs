// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Data.Appearance;
using Bili.Copilot.Models.Data.Community;
using CommunityToolkit.Mvvm.ComponentModel;
using Humanizer;

namespace Bili.Copilot.ViewModels.Items;

/// <summary>
/// 聊天消息项视图模型.
/// </summary>
public sealed partial class ChatMessageItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private ChatMessage _data;

    [ObservableProperty]
    private string _slimTime;

    [ObservableProperty]
    private string _fullTime;

    [ObservableProperty]
    private bool _isMe;

    [ObservableProperty]
    private EmoteText _content;

    [ObservableProperty]
    private string _imageUri;

    [ObservableProperty]
    private bool _isImage;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatMessageItemViewModel"/> class.
    /// </summary>
    public ChatMessageItemViewModel(ChatMessage message, UserItemViewModel user = default)
    {
        Data = message;
        User = user;
        SlimTime = message.Time.Humanize();
        FullTime = message.Time.ToLocalTime().ToString("yyyy/MM/dd HH:mm");
        IsMe = user == default;
        Content = message.Content;
        IsImage = message.Type == Models.Constants.Bili.ChatMessageType.Image;

        if (IsImage)
        {
            ImageUri = message.Content.Text;
        }

        if (message.Type == Models.Constants.Bili.ChatMessageType.Unknown)
        {
            Content = new EmoteText(ResourceToolkit.GetLocalizedString(Models.Constants.App.StringNames.NotSupportContent), default);
        }
    }

    /// <summary>
    /// 用户信息.
    /// </summary>
    public UserItemViewModel User { get; }
}
