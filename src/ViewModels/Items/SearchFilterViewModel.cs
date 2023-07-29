// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Bili.Copilot.Models.Data.Appearance;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 搜索过滤视图模型.
/// </summary>
public sealed partial class SearchFilterViewModel : ViewModelBase
{
    /// <summary>
    /// 筛选器.
    /// </summary>
    [ObservableProperty]
    private Filter _filter;

    /// <summary>
    /// 当前值.
    /// </summary>
    [ObservableProperty]
    private Condition _currentCondition;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchFilterViewModel"/> class.
    /// </summary>
    public SearchFilterViewModel(Filter filter, Condition currentCondition = null)
    {
        Filter = filter;
        CurrentCondition = currentCondition ?? filter.Conditions.First();
    }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is SearchFilterViewModel model && EqualityComparer<Filter>.Default.Equals(Filter, model.Filter);

    /// <inheritdoc/>
    public override int GetHashCode() => Filter.GetHashCode();
}
