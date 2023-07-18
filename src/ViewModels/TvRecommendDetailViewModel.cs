// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 电视剧推荐内容详情视图模型.
/// </summary>
public sealed class TvRecommendDetailViewModel : PgcRecommendDetailViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TvRecommendDetailViewModel"/> class.
    /// </summary>
    private TvRecommendDetailViewModel()
        : base(Models.Constants.Bili.PgcType.TV)
    {
    }

    /// <summary>
    /// 实例.
    /// </summary>
    public static TvRecommendDetailViewModel Instance { get; } = new();
}
