﻿// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.Local;

namespace Bili.Copilot.Models.Data.Video;

/// <summary>
/// 视频集，包含视频列表和视频总数.
/// </summary>
public sealed class VideoSet : ContentSet<VideoInformation>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoSet"/> class.
    /// </summary>
    /// <param name="items">视频列表.</param>
    /// <param name="totalCount">视频总数.</param>
    public VideoSet(
        IEnumerable<VideoInformation> items,
        int totalCount)
    {
        Items = items;
        TotalCount = totalCount;
    }
}
