// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.Bili;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 关注页面视图模型.
/// </summary>
public sealed class FollowsDetailViewModel : RelationDetailViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FollowsDetailViewModel"/> class.
    /// </summary>
    public FollowsDetailViewModel()
        : base(RelationType.Follows)
    {
    }
}
