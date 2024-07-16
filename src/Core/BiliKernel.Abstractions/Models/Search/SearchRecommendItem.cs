// Copyright (c) Richasy. All rights reserved.

namespace Richasy.BiliKernel.Models.Search;

/// <summary>
/// 搜索推荐条目.
/// </summary>
public sealed class SearchRecommendItem : SearchSuggestItemBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchRecommendItem"/> class.
    /// </summary>
    public SearchRecommendItem(
        string id,
        int index,
        string keyword,
        string text)
    {
        Id = id;
        Index = index;
        Keyword = keyword;
        Text = text;
    }

    /// <summary>
    /// 索引.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// 标识符.
    /// </summary>
    public string Id { get; set; }
}
