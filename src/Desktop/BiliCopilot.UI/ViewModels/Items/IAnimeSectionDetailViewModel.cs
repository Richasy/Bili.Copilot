// Copyright (c) Bili Copilot. All rights reserved.

using System.ComponentModel;
using BiliCopilot.UI.Models.Constants;
using CommunityToolkit.Mvvm.Input;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 动漫分区详情视图模型.
/// </summary>
public interface IAnimeSectionDetailViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// 分区类型.
    /// </summary>
    public AnimeSectionType SectionType { get; }

    /// <summary>
    /// 初始化命令.
    /// </summary>
    public IAsyncRelayCommand InitializeCommand { get; }
}
