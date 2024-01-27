// Copyright (c) Bili Copilot. All rights reserved.

using System;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 国创推荐内容详情视图模型.
/// </summary>
public sealed class DomesticRecommendDetailViewModel : PgcRecommendDetailViewModel
{
    private static readonly Lazy<DomesticRecommendDetailViewModel> _lazyInstance = new(() => new DomesticRecommendDetailViewModel());

    private DomesticRecommendDetailViewModel()
        : base(Models.Constants.Bili.PgcType.Domestic)
    {
    }

    /// <summary>
    /// 实例.
    /// </summary>
    public static DomesticRecommendDetailViewModel Instance => _lazyInstance.Value;
}
