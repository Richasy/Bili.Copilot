// Copyright (c) Richasy. All rights reserved.

using Bilibili.Polymer.App.Search.V1;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models.Search;

namespace Richasy.BiliKernel.Services.Search.Core;

internal static class SearchAdapter
{
    public static HotSearchItem ToHotSearchItem(this WebHotSearchItem item)
    {
        return new HotSearchItem(
            item.HotId.ToString(),
            item.Position,
            item.Keyword,
            item.ShowName,
            item.ShowLiveIcon,
            item.ResourceId?.ToString(),
            item.Icon?.ToImage());
    }

    public static SearchRecommendItem ToSearchRecommendItem(this WebSearchRecommendItem item)
    {
        return new SearchRecommendItem(
            item.Id.ToString(),
            item.Position,
            item.Keyword,
            item.Title
        );
    }

    public static SearchPartition ToSearchPartition(this Nav nav)
    {
        return new SearchPartition(
            nav.Type,
            nav.Name,
            nav.Pages is 0 ? default : nav.Pages,
            nav.Total is 0 ? default : nav.Total);
    }

    public static SearchResultItem ToSearchResultItem(this Item item)
    {
        var season = item.ToSeasonInformation();
        var article = item.ToArticleInformation();
        var live = item.ToLiveInformation();
        var user = item.ToUserCard();
        return new SearchResultItem(default, season, live, article, user);
    }
}
