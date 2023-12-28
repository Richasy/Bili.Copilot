// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 消息头部视图模型.
/// </summary>
public sealed partial class MessageHeaderViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private int _count;

    [ObservableProperty]
    private bool _isShowBadge;

    [ObservableProperty]
    private bool _isSelected;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageHeaderViewModel"/> class.
    /// </summary>
    public MessageHeaderViewModel(MessageType type, int count = 0)
    {
        Type = type;
        Title = type switch
        {
            MessageType.Reply => ResourceToolkit.GetLocalizedString(StringNames.Reply),
            MessageType.At => ResourceToolkit.GetLocalizedString(StringNames.AtMe),
            MessageType.Like => ResourceToolkit.GetLocalizedString(StringNames.LikeMe),
            _ => string.Empty,
        };
        Count = count;
        IsShowBadge = Count > 0;
    }

    /// <summary>
    /// 消息类型.
    /// </summary>
    public MessageType Type { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is MessageHeaderViewModel model && Type == model.Type;

    /// <inheritdoc/>
    public override int GetHashCode() => Type.GetHashCode();

    partial void OnCountChanged(int value)
        => IsShowBadge = value > 0;
}
