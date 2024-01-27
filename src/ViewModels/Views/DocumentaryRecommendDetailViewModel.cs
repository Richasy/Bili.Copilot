// Copyright (c) Bili Copilot. All rights reserved.

using System;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 纪录片推荐内容详情视图模型.
/// </summary>
public sealed class DocumentaryRecommendDetailViewModel : PgcRecommendDetailViewModel
{
    private static readonly Lazy<DocumentaryRecommendDetailViewModel> _lazyInstance = new(() => new DocumentaryRecommendDetailViewModel());

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentaryRecommendDetailViewModel"/> class.
    /// </summary>
    private DocumentaryRecommendDetailViewModel()
        : base(Models.Constants.Bili.PgcType.Documentary)
    {
    }

    /// <summary>
    /// 实例.
    /// </summary>
    public static DocumentaryRecommendDetailViewModel Instance => _lazyInstance.Value;
}
