// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.Bili;

namespace Bili.Copilot.ViewModels.Views;

/// <summary>
/// 粉丝页面视图模型.
/// </summary>
public sealed class FansDetailViewModel : RelationDetailViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FansDetailViewModel"/> class.
    /// </summary>
    public FansDetailViewModel()
        : base(RelationType.Fans)
    {
    }
}
