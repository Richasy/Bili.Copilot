// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 推荐视频列表.
/// </summary>
public sealed class RecommendVideoList(
    IList<VideoBase> items,
    long offset)
{
    /// <summary>
    /// 视频列表.
    /// </summary>
    public IList<VideoBase> Items { get; } = items;

    /// <summary>
    /// 下一个偏移量.
    /// </summary>
    public long Offset { get; } = offset;
}
