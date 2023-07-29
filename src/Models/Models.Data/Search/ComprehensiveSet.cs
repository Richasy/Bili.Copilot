// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Video;

namespace Bili.Copilot.Models.Data.Search;

/// <summary>
/// 综合搜索结果集.
/// </summary>
public sealed class ComprehensiveSet
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComprehensiveSet"/> class.
    /// </summary>
    /// <param name="videoSet">视频条目.</param>
    /// <param name="metadata">元数据.</param>
    public ComprehensiveSet(
        SearchSet<VideoInformation> videoSet,
        Dictionary<SearchModuleType, int> metadata = default)
    {
        Metadata = metadata;
        VideoSet = videoSet;
    }

    /// <summary>
    /// 元数据，包含子模块的结果总数.
    /// </summary>
    public Dictionary<SearchModuleType, int> Metadata { get; }

    /// <summary>
    /// 综合搜索的视频结果.
    /// </summary>
    public SearchSet<VideoInformation> VideoSet { get; }
}
