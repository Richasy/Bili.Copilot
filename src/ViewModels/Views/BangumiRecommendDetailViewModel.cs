// Copyright (c) Bili Copilot. All rights reserved.

using System;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 番剧推荐内容详情视图模型.
/// </summary>
public sealed class BangumiRecommendDetailViewModel : PgcRecommendDetailViewModel
{
    private static readonly Lazy<BangumiRecommendDetailViewModel> _lazyInstance = new Lazy<BangumiRecommendDetailViewModel>(() => new BangumiRecommendDetailViewModel());

    /// <summary>
    /// Initializes a new instance of the <see cref="BangumiRecommendDetailViewModel"/> class.
    /// </summary>
    private BangumiRecommendDetailViewModel()
        : base(Models.Constants.Bili.PgcType.Bangumi)
    {
    }

    /// <summary>
    /// 实例.
    /// </summary>
    public static BangumiRecommendDetailViewModel Instance => _lazyInstance.Value;
}
