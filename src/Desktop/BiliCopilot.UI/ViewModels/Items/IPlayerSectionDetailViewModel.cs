// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 播放器区块详情视图模型.
/// </summary>
public interface IPlayerSectionDetailViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// 尝试首次加载命令.
    /// </summary>
    IAsyncRelayCommand TryFirstLoadCommand { get; }

    /// <summary>
    /// 标题.
    /// </summary>
    string Title { get; }
}
