// Copyright (c) Bili Copilot. All rights reserved.

using System.ComponentModel;
using BiliCopilot.UI.Models.Constants;
using CommunityToolkit.Mvvm.Input;
using Richasy.BiliKernel.Models.Search;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 搜索分区详情视图模型.
/// </summary>
public interface ISearchSectionDetailViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// 分区类型.
    /// </summary>
    public SearchSectionType SectionType { get; }

    /// <summary>
    /// 是否为空.
    /// </summary>
    public bool IsEmpty { get; }

    /// <summary>
    /// 首次加载数据.
    /// </summary>
    IAsyncRelayCommand TryFirstLoadCommand { get; }

    /// <summary>
    /// 刷新命令.
    /// </summary>
    IAsyncRelayCommand RefreshCommand { get; }

    /// <summary>
    /// 初始化.
    /// </summary>
    void Initialize(string keyword, SearchPartition partition);

    /// <summary>
    /// 清理数据.
    /// </summary>
    void Clear();
}
