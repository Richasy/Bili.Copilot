// Copyright (c) Bili Copilot. All rights reserved.

using System.ComponentModel;
using BiliCopilot.UI.Models.Constants;
using CommunityToolkit.Mvvm.Input;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// PGC分区详情视图模型.
/// </summary>
public interface IPgcSectionDetailViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// 分区类型.
    /// </summary>
    public PgcSectionType SectionType { get; }

    /// <summary>
    /// 初始化命令.
    /// </summary>
    public IAsyncRelayCommand InitializeCommand { get; }

    /// <summary>
    /// 刷新命令.
    /// </summary>
    public IAsyncRelayCommand RefreshCommand { get; }
}
