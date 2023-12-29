// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using Bili.Copilot.Models.Data.Community;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels.Items;

/// <summary>
/// 分区项视图模型.
/// </summary>
public sealed partial class PartitionItemViewModel : SelectableViewModel<Partition>
{
    [ObservableProperty]
    private string _additionalText;

    [ObservableProperty]
    private bool _isExpanded;

    /// <summary>
    /// Initializes a new instance of the <see cref="PartitionItemViewModel"/> class.
    /// </summary>
    public PartitionItemViewModel(Partition data)
        : base(data)
    {
        if (data.Children != null)
        {
            Children = new ObservableCollection<PartitionItemViewModel>();
            foreach (var child in data.Children)
            {
                Children.Add(new PartitionItemViewModel(child));
            }
        }
    }

    /// <summary>
    /// 子分区集合.
    /// </summary>
    public ObservableCollection<PartitionItemViewModel> Children { get; }

    /// <inheritdoc/>
    public override string ToString()
        => Data.Name;
}
