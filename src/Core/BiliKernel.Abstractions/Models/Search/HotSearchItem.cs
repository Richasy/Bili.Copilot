// Copyright (c) Richasy. All rights reserved.

using System;
using Richasy.BiliKernel.Models.Appearance;

namespace Richasy.BiliKernel.Models.Search;

/// <summary>
/// 热搜条目.
/// </summary>
public sealed class HotSearchItem : SearchSuggestItemBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HotSearchItem"/> class.
    /// </summary>
    public HotSearchItem(
        string id,
        int index,
        string keyword,
        string text,
        bool? isLive = default,
        string? liveRoomId = default,
        BiliImage? icon = default)
    {
        Id = id;
        Index = index;
        Keyword = keyword;
        Text = text;
        IsLive = isLive;
        LiveRoomId = liveRoomId;
        Icon = icon;
    }

    /// <summary>
    /// 热搜标识符.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 是否为直播间.
    /// </summary>
    public bool? IsLive { get; }

    /// <summary>
    /// 直播间Id.
    /// </summary>
    public string? LiveRoomId { get; }

    /// <summary>
    /// 额外显示的图标.
    /// </summary>
    public BiliImage? Icon { get; }

    /// <summary>
    /// 索引.
    /// </summary>
    public int Index { get; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is HotSearchItem item && Id == item.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
