// Copyright (c) Richasy. All rights reserved.

namespace Richasy.BiliKernel.Models.Search;

/// <summary>
/// 搜索建议条目基类.
/// </summary>
public abstract class SearchSuggestItemBase
{
    /// <summary>
    /// 显示文本.
    /// </summary>
    public string Text { get; protected set; }

    /// <summary>
    /// 搜索关键词.
    /// </summary>
    public string Keyword { get; protected set; }
}
