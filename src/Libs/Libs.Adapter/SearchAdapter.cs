// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Search;
using Bili.Copilot.Models.Data.Video;
using Bilibili.App.Interface.V1;

namespace Bili.Copilot.Libs.Adapter;

/// <summary>
/// 搜索数据适配器.
/// </summary>
public static class SearchAdapter
{
    /// <summary>
    /// 将热搜条目 <see cref="SearchRecommendItem"/> 转换为搜索建议条目.
    /// </summary>
    /// <param name="item">热搜条目.</param>
    /// <returns><see cref="SearchSuggest"/>.</returns>
    public static SearchSuggest ConvertToSearchSuggest(SearchRecommendItem item)
        => new(item.Position, item.DisplayName, item.Keyword, item.Icon);

    /// <summary>
    /// 将来自 Web 的搜索建议条目 <see cref="ResultItem"/> 转换为本地搜索建议条目.
    /// </summary>
    /// <param name="item">来自 Web 的搜索建议条目.</param>
    /// <returns><see cref="SearchSuggest"/>.</returns>
    public static SearchSuggest ConvertToSearchSuggest(ResultItem item)
        => new(item.Position, item.Title, item.Keyword);

    /// <summary>
    /// 将综合搜索结果响应 <see cref="ComprehensiveSearchResultResponse"/> 转换为综合数据集.
    /// </summary>
    /// <param name="response">综合搜索结果响应.</param>
    /// <returns><see cref="ComprehensiveSet"/>.</returns>
    public static ComprehensiveSet ConvertToComprehensiveSet(ComprehensiveSearchResultResponse response)
    {
        var metaList = new Dictionary<SearchModuleType, int>();
        foreach (var item in response.SubModuleList)
        {
            metaList.Add((SearchModuleType)item.Type, item.Total);
        }

        var isEnd = response.ItemList == null;
        var videos = isEnd
            ? new List<VideoInformation>()
            : response.ItemList.Where(p => p.Goto == ServiceConstants.Av).Select(VideoAdapter.ConvertToVideoInformation).ToList();
        var videoSet = new SearchSet<VideoInformation>(videos, isEnd);

        return new ComprehensiveSet(videoSet, metaList);
    }
}
