// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.Community;

namespace Bili.Copilot.ViewModels.Items;

/// <summary>
/// 分区项视图模型.
/// </summary>
public sealed partial class PartitionItemViewModel : SelectableViewModel<Partition>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PartitionItemViewModel"/> class.
    /// </summary>
    public PartitionItemViewModel(Partition data)
        : base(data)
    {
    }
}
