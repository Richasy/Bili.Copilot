// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 收藏分区详情视图模型接口.
/// </summary>
public interface IFavoriteSectionDetailViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// 首次加载数据.
    /// </summary>
    IAsyncRelayCommand TryFirstLoadCommand { get; }

    /// <summary>
    /// 刷新命令.
    /// </summary>
    IAsyncRelayCommand RefreshCommand { get; }
}
