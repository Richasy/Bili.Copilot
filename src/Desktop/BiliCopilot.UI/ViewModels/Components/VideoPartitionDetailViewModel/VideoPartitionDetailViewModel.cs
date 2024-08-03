// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 视频分区详情视图模型.
/// </summary>
public sealed partial class VideoPartitionDetailViewModel : ViewModelBase<PartitionViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPartitionDetailViewModel"/> class.
    /// </summary>
    public VideoPartitionDetailViewModel(
        PartitionViewModel partition,
        IVideoDiscoveryService service)
        : base(partition)
    {
        _service = service;
        _logger = this.Get<ILogger<VideoPartitionDetailViewModel>>();
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Videos.Count > 0)
        {
            return;
        }

        if (Data.Children is not null)
        {
            var children = Data.Children.ToList();
            var rcmdPartition = new Partition(Data.Data.Id, ResourceToolkit.GetLocalizedString(StringNames.Recommend));
            children.Insert(0, new PartitionViewModel(rcmdPartition));
            Children = [.. children];
        }

        SortTypes = [.. Enum.GetValues<PartitionVideoSortType>()];
        SelectedSortType = PartitionVideoSortType.Default;
        await ChangeChildPartitionAsync(Children.First());
        Initialized?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task ChangeSortTypeAsync(PartitionVideoSortType sortType)
    {
        if (IsRecommend)
        {
            return;
        }

        SelectedSortType = sortType;
        _childPartitionVideoCache.Clear();
        Videos.Clear();
        _childPartitionOffsetCache.Clear();
        await LoadVideosAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        if (IsRecommend)
        {
            _recommendOffset = 0;
            _recommendVideoCache?.Clear();
        }
        else
        {
            _childPartitionVideoCache.Remove(CurrentPartition.Data.Id);
            _childPartitionOffsetCache.Remove(CurrentPartition.Data.Id);
        }

        Videos.Clear();
        await LoadVideosAsync();
    }

    [RelayCommand]
    private async Task ChangeChildPartitionAsync(PartitionViewModel partition)
    {
        if (partition is null || partition.Data.Equals(CurrentPartition?.Data))
        {
            return;
        }

        IsRecommend = partition.Data.Id == Data.Data.Id;
        if (Videos.Count > 0)
        {
            if (IsRecommend)
            {
                _recommendVideoCache = Videos.ToList();
            }
            else
            {
                _childPartitionVideoCache[CurrentPartition.Data.Id] = Videos.ToList();
            }
        }

        CurrentPartition = partition;
        Videos.Clear();
        if (IsRecommend && _recommendVideoCache is not null && _recommendVideoCache.Count > 0)
        {
            foreach (var item in _recommendVideoCache)
            {
                Videos.Add(item);
            }
        }
        else if (_childPartitionVideoCache.TryGetValue(CurrentPartition.Data.Id, out var cache))
        {
            foreach (var item in cache)
            {
                Videos.Add(item);
            }
        }

        if (Videos.Count == 0)
        {
            await LoadVideosAsync();
        }
        else
        {
            VideoListUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

    [RelayCommand]
    private async Task LoadVideosAsync()
    {
        if (IsVideoLoading)
        {
            return;
        }

        IsVideoLoading = true;
        if (IsRecommend)
        {
            await LoadRecommendVideosAsync();
        }
        else
        {
            await LoadChildPartitionVideosAsync();
        }

        IsVideoLoading = false;
        VideoListUpdated?.Invoke(this, EventArgs.Empty);
    }

    private async Task LoadRecommendVideosAsync()
    {
        try
        {
            var (videos, offset) = await _service.GetPartitionRecommendVideoListAsync(Data.Data, _recommendOffset);
            _recommendOffset = offset;
            TryAddVideos(videos, VideoCardStyle.Partition);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试加载分区推荐视频时出错.");
        }
    }

    private async Task LoadChildPartitionVideosAsync()
    {
        try
        {
            var offset = 0L;
            var pn = 0;
            if (_childPartitionOffsetCache.TryGetValue(CurrentPartition.Data.Id, out var offsetCache))
            {
                offset = offsetCache.Offset;
                pn = offsetCache.PageNumber;
            }

            var (videos, nextOffset, nextPn) = await _service.GetChildPartitionVideoListAsync(CurrentPartition.Data, offset, pn, SelectedSortType);
            _childPartitionOffsetCache[CurrentPartition.Data.Id] = (nextOffset, nextPn);
            TryAddVideos(videos, VideoCardStyle.Partition);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"尝试加载 {Data.Data.Name}/{CurrentPartition.Data.Name} 分区视频时出错.");
        }
    }

    private void TryAddVideos(IReadOnlyList<VideoInformation> videos, VideoCardStyle style)
    {
        if (videos is not null)
        {
            foreach (var item in videos)
            {
                Videos.Add(new VideoItemViewModel(item, style));
            }
        }
    }
}
