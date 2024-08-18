// Copyright (c) Bili Copilot. All rights reserved.

using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 消息区块详情视图模型接口.
/// </summary>
public interface IMessageSectionDetailViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// 初始化命令.
    /// </summary>
    IAsyncRelayCommand InitializeCommand { get; }

    /// <summary>
    /// 刷新命令.
    /// </summary>
    IAsyncRelayCommand RefreshCommand { get; }

    /// <summary>
    /// 未读消息数量.
    /// </summary>
    int UnreadCount { get; }
}
