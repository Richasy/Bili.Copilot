// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using Bili.Copilot.Models.Data.Community;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 消息条目视图模型.
/// </summary>
public sealed partial class MessageItemViewModel
{
    [ObservableProperty]
    private MessageInformation _data;

    [ObservableProperty]
    private string _publishTime;

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is MessageItemViewModel model && EqualityComparer<MessageInformation>.Default.Equals(Data, model.Data);

    /// <inheritdoc/>
    public override int GetHashCode() => Data.GetHashCode();
}
