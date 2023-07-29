// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 电影推荐内容详情视图模型.
/// </summary>
public sealed class MovieRecommendDetailViewModel : PgcRecommendDetailViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MovieRecommendDetailViewModel"/> class.
    /// </summary>
    private MovieRecommendDetailViewModel()
           : base(Models.Constants.Bili.PgcType.Movie)
    {
    }

    /// <summary>
    /// 实例.
    /// </summary>
    public static MovieRecommendDetailViewModel Instance { get; } = new();
}
