// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using Bili.Copilot.Models.Constants.Bili;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 提供专栏文章相关的操作.
/// </summary>
public partial class ArticleProvider
{
    private readonly Dictionary<string, (ArticleSortType Sort, int PageNumber)> _partitionCache;

    /// <summary>
    /// 实例.
    /// </summary>
    public static ArticleProvider Instance { get; } = new();
}
