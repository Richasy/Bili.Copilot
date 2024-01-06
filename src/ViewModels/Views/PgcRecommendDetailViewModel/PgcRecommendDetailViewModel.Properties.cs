// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// PGC推荐内容详情视图模型.
/// </summary>
public partial class PgcRecommendDetailViewModel
{
    private readonly PgcType _type;
    private bool _isEnd;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private string _title;

    /// <summary>
    /// 请求滚动到顶部.
    /// </summary>
    public event EventHandler RequestScrollToTop;

    /// <summary>
    /// 筛选条件.
    /// </summary>
    public ObservableCollection<IndexFilterItemViewModel> Filters { get; }
}
