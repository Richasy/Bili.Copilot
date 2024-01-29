// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Data.Local;
using static Bili.Copilot.Models.App.Constants.AppConstants;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 分区及标签的相关定义和扩展方法.
/// </summary>
public partial class HomeProvider
{
    private static readonly Lazy<HomeProvider> _lazyInstance = new(() => new HomeProvider());
    private readonly Dictionary<string, (int OffsetId, int PageNumber)> _cacheVideoPartitionOffsets;

    private long _recommendOffsetId;
    private long _hotOffsetId;
    private int _videoPartitionOffsetId = 0;
    private int _videoPartitionPageNumber = 1;
    private string _currentPartitionId = string.Empty;

    /// <summary>
    /// 实例.
    /// </summary>
    public static HomeProvider Instance => _lazyInstance.Value;

    private static async Task<IEnumerable<Models.Data.Community.Partition>> GetPartitionCacheAsync()
    {
        var cacheData = await FileToolkit.ReadLocalDataAsync<Cache<List<Models.Data.Community.Partition>>>(
            Location.PartitionCache,
            folderName: Location.ServerFolder);

        return cacheData == null || cacheData.ExpiryTime < System.DateTimeOffset.Now ? (IEnumerable<Models.Data.Community.Partition>)null : cacheData.Data;
    }

    private static async Task CachePartitionsAsync(IEnumerable<Models.Data.Community.Partition> data)
    {
        var localCache = new Cache<List<Models.Data.Community.Partition>>(System.DateTimeOffset.Now.AddDays(1), data.ToList());
        await FileToolkit.WriteLocalDataAsync(Location.PartitionCache, localCache, Location.ServerFolder);
    }

    private void UpdateVideoPartitionCache()
    {
        _ = _cacheVideoPartitionOffsets.Remove(_currentPartitionId);
        _cacheVideoPartitionOffsets.Add(_currentPartitionId, (_videoPartitionOffsetId, _videoPartitionPageNumber));
    }

    private void RetriveCachedSubPartitionOffset(string partitionId)
    {
        if (_cacheVideoPartitionOffsets.ContainsKey(partitionId))
        {
            var (oid, pn) = _cacheVideoPartitionOffsets[partitionId];
            _videoPartitionOffsetId = oid;
            _videoPartitionPageNumber = pn;
        }

        _currentPartitionId = partitionId;
    }
}
