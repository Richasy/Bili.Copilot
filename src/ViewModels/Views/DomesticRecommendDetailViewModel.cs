// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 国创推荐内容详情视图模型.
/// </summary>
public sealed class DomesticRecommendDetailViewModel : PgcRecommendDetailViewModel
{
    private DomesticRecommendDetailViewModel()
        : base(Models.Constants.Bili.PgcType.Domestic)
    {
    }

    /// <summary>
    /// 实例.
    /// </summary>
    public static DomesticRecommendDetailViewModel Instance { get; } = new();
}
