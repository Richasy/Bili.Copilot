// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Bili.Copilot.Models.Data.Appearance;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels.Items;

/// <summary>
/// PGC索引筛选视图模型.
/// </summary>
public sealed partial class IndexFilterItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private Filter _data;

    [ObservableProperty]
    private int _selectedIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexFilterItemViewModel"/> class.
    /// </summary>
    public IndexFilterItemViewModel(Filter data, Condition selectedItem = null)
    {
        Data = data;
        SelectedIndex = selectedItem == null ? 0 : data.Conditions.ToList().IndexOf(selectedItem);
    }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is IndexFilterItemViewModel model && EqualityComparer<Filter>.Default.Equals(Data, model.Data);

    /// <inheritdoc/>
    public override int GetHashCode() => Data.GetHashCode();
}
