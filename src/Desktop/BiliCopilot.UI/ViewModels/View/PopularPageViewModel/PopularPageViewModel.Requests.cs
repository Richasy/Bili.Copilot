﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 流行视频页面视图模型.
/// </summary>
public sealed partial class PopularPageViewModel
{
    private async Task LoadRecommendVideosAsync()
    {
        try
        {
            var (videos, nextOffset) = await _service.GetRecommendVideoListAsync(_recommendOffset).ConfigureAwait(true);
            _recommendOffset = nextOffset;
            TryAddVideos(videos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试加载推荐视频时出错.");
        }
    }

    private async Task LoadHotVideosAsync()
    {
        try
        {
            var (videos, nextOffset) = await _service.GetHotVideoListAsync(_hotOffset).ConfigureAwait(true);
            _hotOffset = nextOffset;
            TryAddVideos(videos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试加载热门视频时出错.");
        }
    }

    private async Task LoadTotalRankVideosAsync()
    {
        try
        {
            var videos = await _service.GetGlobalRankingListAsync().ConfigureAwait(true);
            TryAddVideos(videos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试加载全站排行榜视频时出错.");
        }
    }

    private async Task LoadPartitionRankVideosAsync(Partition partition)
    {
        try
        {
            var videos = await _service.GetPartitionRankingListAsync(partition).ConfigureAwait(true);
            TryAddVideos(videos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"尝试加载 {partition.Name} 分区排行榜视频时出错.");
        }
    }

    private async Task LoadPartitionsAsync()
    {
        IsPartitionLoading = true;
        var partitions = await _service.GetVideoPartitionsAsync().ConfigureAwait(true);
        if (partitions != null)
        {
            foreach (var item in partitions)
            {
                Sections.Add(new PopularRankPartitionViewModel(item));
            }
        }

        IsPartitionLoading = false;
    }

    private void TryAddVideos(IReadOnlyList<VideoInformation> videos)
    {
        if (videos is not null)
        {
            foreach (var item in videos)
            {
                Videos.Add(new VideoItemViewModel(item));
            }
        }
    }
}
