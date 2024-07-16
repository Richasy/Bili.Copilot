// Copyright (c) Richasy. All rights reserved.

using Bilibili.Polymer.App.Search.V1;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Services.Search.Core;

internal static class LiveAdapter
{
    public static LiveInformation ToLiveInformation(this Item item)
    {
        var liveId = item.Param;
        return default;
    }
}
