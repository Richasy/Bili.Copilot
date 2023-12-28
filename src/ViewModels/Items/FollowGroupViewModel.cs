// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.Community;

namespace Bili.Copilot.ViewModels.Items;

/// <summary>
/// 关注分组视图模型.
/// </summary>
public sealed partial class FollowGroupViewModel : SelectableViewModel<FollowGroup>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FollowGroupViewModel"/> class.
    /// </summary>
    public FollowGroupViewModel(FollowGroup data)
        : base(data)
    {
    }
}
