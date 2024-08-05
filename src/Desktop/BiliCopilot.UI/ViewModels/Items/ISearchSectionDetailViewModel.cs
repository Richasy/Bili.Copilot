// Copyright (c) Bili Copilot. All rights reserved.

using System.ComponentModel;
using BiliCopilot.UI.Models.Constants;
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
    /// 初始化.
    /// </summary>
    internal void Initialize(string keyword, SearchPartition partition);
}
