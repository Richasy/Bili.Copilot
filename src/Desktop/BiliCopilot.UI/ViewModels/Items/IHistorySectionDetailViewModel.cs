// Copyright (c) Bili Copilot. All rights reserved.

using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Richasy.BiliKernel.Models;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 历史记录详情区块.
/// </summary>
public interface IHistorySectionDetailViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// 历史记录类型.
    /// </summary>
    ViewHistoryTabType Type { get; }

    /// <summary>
    /// 是否为空.
    /// </summary>
    public bool IsEmpty { get; }

    /// <summary>
    /// 是否正在加载.
    /// </summary>
    public bool IsLoading { get; }

    /// <summary>
    /// 首次加载数据.
    /// </summary>
    IAsyncRelayCommand TryFirstLoadCommand { get; }

    /// <summary>
    /// 刷新命令.
    /// </summary>
    IAsyncRelayCommand RefreshCommand { get; }
}
