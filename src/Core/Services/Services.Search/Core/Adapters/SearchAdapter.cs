// Copyright (c) Richasy. All rights reserved.

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
}
