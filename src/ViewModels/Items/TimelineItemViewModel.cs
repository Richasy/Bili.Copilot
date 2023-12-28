// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.Pgc;

namespace Bili.Copilot.ViewModels.Items;

/// <summary>
/// 时间线项视图模型.
/// </summary>
public sealed partial class TimelineItemViewModel : SelectableViewModel<TimelineInformation>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TimelineItemViewModel"/> class.
    /// </summary>
    public TimelineItemViewModel(TimelineInformation data)
        : base(data)
    {
    }
}
