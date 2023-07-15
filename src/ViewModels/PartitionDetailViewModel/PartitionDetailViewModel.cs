// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Community;
using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 分区详情视图模型.
/// </summary>
public sealed partial class PartitionDetailViewModel : InformationFlowViewModel<VideoItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PartitionDetailViewModel"/> class.
    /// </summary>
    private PartitionDetailViewModel()
    {
        _caches = new Dictionary<Partition, IEnumerable<VideoInformation>>();

        Banners = new ObservableCollection<BannerViewModel>();
        SubPartitions = new ObservableCollection<Partition>();
        RecommendPartitions = new ObservableCollection<Partition>();
        SortTypes = new ObservableCollection<VideoSortType>()
        {
            VideoSortType.Default,
            VideoSortType.Newest,
            VideoSortType.Play,
            VideoSortType.Reply,
            VideoSortType.Danmaku,
            VideoSortType.Favorite,
        };

        DetailType = SettingsToolkit.ReadLocalSetting(SettingNames.LastVideoPartitionDetailType, VideoPartitionDetailType.Recommend);
        CheckModuleState();
    }

    /// <inheritdoc/>
    protected override void BeforeReload()
        => HomeProvider.Instance.ResetSubPartitionState();

    /// <inheritdoc/>
    protected override string FormatException(string errorMsg)
        => $"{ResourceToolkit.GetLocalizedString(StringNames.RequestSubPartitionFailed)}\n{errorMsg}";

    /// <inheritdoc/>
    protected override async Task GetDataAsync()
    {
        var partition = IsSubPartitionShown ? CurrentSubPartition : CurrentRecommendPartition;

        IEnumerable<VideoInformation> videos = default;
        if (partition.Id == "-1")
        {
            // 排行榜数据.
            videos = await HomeProvider.GetRankDetailAsync(partition.ParentId);
        }
        else
        {
            var isRecommend = partition.Id == OriginPartition.Id;
            var data = await HomeProvider.Instance.GetVideoSubPartitionDataAsync(partition.Id, isRecommend, SortType);
            if (data.Banners?.Count() > 0)
            {
                foreach (var item in data.Banners)
                {
                    if (!Banners.Any(p => p.Uri == item.Uri))
                    {
                        var vm = new BannerViewModel(item);
                        Banners.Add(vm);
                    }
                }
            }

            videos = data.Videos;
        }

        CheckBannerState();
        if (videos?.Count() > 0)
        {
            IsEmpty = false;
            foreach (var video in videos)
            {
                video.Publisher = default;
                var videoVM = new VideoItemViewModel(video);
                Items.Add(videoVM);
            }

            var videoVMs = Items
                    .OfType<VideoItemViewModel>()
                    .Select(p => p.Data)
                    .ToList();
            if (_caches.ContainsKey(partition))
            {
                _caches[partition] = videoVMs;
            }
            else
            {
                _caches.Add(partition, videoVMs);
            }
        }
        else
        {
            IsEmpty = Items.Count == 0;
        }
    }

    [RelayCommand]
    private void LoadPartition(Partition partition)
    {
        OriginPartition = partition;
        _caches.Clear();
        HomeProvider.Instance.ClearPartitionState();
        TryClear(SubPartitions);
        TryClear(RecommendPartitions);
        TryClear(Banners);

        // 子分区中不包含推荐分区.
        partition.Children.Where(p => p.Id != p.ParentId).ToList().ForEach(SubPartitions.Add);

        // 合并推荐分区及排行榜分区.
        var rcmdPartition = partition.Children.First(p => p.Id == p.ParentId);
        var rankPartition = new Partition("-1", ResourceToolkit.GetLocalizedString(StringNames.Rank), parentId: rcmdPartition.ParentId);
        RecommendPartitions.Add(rcmdPartition);
        RecommendPartitions.Add(rankPartition);

        SelectSubPartition(IsSubPartitionShown ? SubPartitions[0] : RecommendPartitions[0]);
    }

    [RelayCommand]
    private void SelectSubPartition(Partition subPartition)
    {
        TryClear(Items);

        if (IsRecommendShown)
        {
            CurrentRecommendPartition = subPartition;
        }
        else
        {
            CurrentSubPartition = subPartition;
        }

        if (_caches.ContainsKey(subPartition))
        {
            var data = _caches[subPartition];
            foreach (var video in data)
            {
                var videoVM = new VideoItemViewModel(video);
                Items.Add(videoVM);
            }

            CheckBannerState();
            IsEmpty = Items.Count == 0;
        }
        else
        {
            InitializeCommand.ExecuteAsync(null);
        }
    }

    private void CheckBannerState()
        => IsShowBanner = Banners.Count > 0 && IsRecommendShown && CurrentRecommendPartition?.Id == OriginPartition.Id;

    private void CheckModuleState()
    {
        IsRecommendShown = DetailType == VideoPartitionDetailType.Recommend;
        IsSubPartitionShown = DetailType == VideoPartitionDetailType.SubPartition;
    }

    partial void OnCurrentSubPartitionChanged(Partition value)
    {
        IsShowBanner = Banners.Count > 0;
    }

    partial void OnDetailTypeChanged(VideoPartitionDetailType value)
    {
        CheckModuleState();
        SettingsToolkit.WriteLocalSetting(SettingNames.LastVideoPartitionDetailType, value);

        if (!IsInitialized)
        {
            return;
        }

        if (IsRecommendShown)
        {
            SelectSubPartitionCommand.Execute(CurrentRecommendPartition ?? RecommendPartitions.First());
        }
        else if (IsSubPartitionShown)
        {
            SelectSubPartitionCommand.Execute(CurrentSubPartition ?? SubPartitions.First());
        }
    }
}
