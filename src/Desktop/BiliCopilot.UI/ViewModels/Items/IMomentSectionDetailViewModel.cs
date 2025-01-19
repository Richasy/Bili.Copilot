// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 动态分区详情视图模型.
/// </summary>
public interface IMomentSectionDetailViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// 分区类型.
    /// </summary>
    MomentSectionType SectionType { get; }

    /// <summary>
    /// 初始化命令.
    /// </summary>
    IAsyncRelayCommand InitializeCommand { get; }

    /// <summary>
    /// 刷新命令.
    /// </summary>
    IRelayCommand RefreshCommand { get; }
}
