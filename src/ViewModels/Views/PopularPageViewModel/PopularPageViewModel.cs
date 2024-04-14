// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Video;
using Bili.Copilot.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 流行视频页面的视图模型.
/// </summary>
public sealed partial class PopularPageViewModel : InformationFlowViewModel<VideoItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PopularPageViewModel"/> class.
    /// </summary>
    private PopularPageViewModel()
    {
        _moduleCaches = new Dictionary<PopularType, IEnumerable<VideoInformation>>();
        _partitionCaches = new Dictionary<string, IEnumerable<VideoInformation>>();
        NavListColumnWidth = SettingsToolkit.ReadLocalSetting(SettingNames.PopularNavListColumnWidth, 280d);
        CurrentType = SettingsToolkit.ReadLocalSetting(SettingNames.LastPopularType, PopularType.Recommend);
        PartitionId = SettingsToolkit.ReadLocalSetting(SettingNames.LastPopularPartitionId, string.Empty);
        AttachIsRunningToAsyncCommand(p => IsInitializing = p, FirstLoadCommand);
        AttachIsRunningToAsyncCommand(p => IsOverlayLoading = p, LoadFeaturedCommand);
        Partitions = new ObservableCollection<PartitionItemViewModel>();
        CheckModuleState();
    }

    /// <inheritdoc/>
    protected override void BeforeReload()
    {
        if (IsRecommendShown)
        {
            HomeProvider.Instance.ResetRecommendState();
        }
        else if (IsHotShown)
        {
            HomeProvider.Instance.ResetHotState();
        }
    }

    /// <inheritdoc/>
    protected override string FormatException(string errorMsg)
    {
        var prefix = CurrentType switch
        {
            PopularType.Recommend => ResourceToolkit.GetLocalizedString(StringNames.RequestRecommendFailed),
            PopularType.Hot => ResourceToolkit.GetLocalizedString(StringNames.RequestPopularFailed),
            PopularType.Rank => ResourceToolkit.GetLocalizedString(StringNames.RankRequestFailed),
            PopularType.Partition => ResourceToolkit.GetLocalizedString(StringNames.PartitionRequestFailed),
            _ => string.Empty,
        };
        return $"{prefix}\n{errorMsg}";
    }

    /// <inheritdoc/>
    protected override async Task GetDataAsync()
    {
        IEnumerable<VideoInformation> videos = default;
        switch (CurrentType)
        {
            case PopularType.Recommend:
                var items = await HomeProvider.Instance.RequestRecommendVideosAsync();
                videos = items.OfType<VideoInformation>();
                break;
            case PopularType.Hot:
                videos = await HomeProvider.Instance.RequestHotVideosAsync();
                if (videos?.Count() < 7)
                {
                    var nextVideos = await HomeProvider.Instance.RequestHotVideosAsync();
                    videos = videos.Concat(nextVideos).Distinct();
                }

                break;
            case PopularType.Rank:
                if (Items.Count > 0)
                {
                    return;
                }

                videos = await HomeProvider.GetRankDetailAsync("0");
                break;
            case PopularType.Partition:
                if (Items.Count > 0)
                {
                    return;
                }

                videos = await HomeProvider.GetRankDetailAsync(PartitionId);
                break;
        }

        if (videos?.Count() > 0)
        {
            IsEmpty = false;
            foreach (var video in videos)
            {
                video.Publisher = default;
                var videoVM = new VideoItemViewModel(video);

                if (Items.Any(p => p.Data == video))
                {
                    continue;
                }

                Items.Add(videoVM);
            }

            var videoVMs = Items
                    .OfType<VideoItemViewModel>()
                    .Select(p => p.Data)
                    .ToList();

            if (CurrentType == PopularType.Partition)
            {
                _partitionCaches[PartitionId] = videoVMs;
            }
            else
            {
                _moduleCaches[CurrentType] = videoVMs;
            }
        }

        IsEmpty = Items.Count == 0;
    }

    [RelayCommand]
    private static async Task LoadFeaturedAsync()
    {
        var videos = await HomeProvider.GetFeaturedVideosAsync();
        if (videos?.Count() > 0)
        {
            AppViewModel.Instance.OpenPlaylistCommand.Execute(videos);
        }
        else
        {
            AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(StringNames.NoFeaturedVideo), InfoType.Error);
        }
    }

    [RelayCommand]
    private async Task FirstLoadAsync()
    {
        if (Partitions.Count > 0)
        {
            return;
        }

        var data = await HomeProvider.GetVideoPartitionIndexAsync();
        data.ToList().ForEach(p =>
        {
            Partitions.Add(new PartitionItemViewModel(p)
            {
                AdditionalText = TextToolkit.ConvertToTraditionalChineseIfNeeded("排行榜"),
            });
        });

        if (!string.IsNullOrEmpty(PartitionId))
        {
            CheckModuleState();
        }
    }

    [RelayCommand]
    private void ChangeModuleType(PopularType type)
    {
        CurrentType = type;
        if (type != PopularType.Partition)
        {
            PartitionId = string.Empty;
        }

        CheckModuleState();

        SettingsToolkit.WriteLocalSetting(SettingNames.LastPopularType, type);
        SettingsToolkit.WriteLocalSetting(SettingNames.LastPopularPartitionId, PartitionId);

        if (!IsInitialized)
        {
            return;
        }

        TryClear(Items);

        if (CurrentType == PopularType.Partition)
        {
            if (_partitionCaches.ContainsKey(PartitionId))
            {
                var data = _partitionCaches[PartitionId];
                foreach (var video in data)
                {
                    var videoVM = new VideoItemViewModel(video);
                    Items.Add(videoVM);
                }

                IsEmpty = Items.Count == 0;
            }
            else
            {
                _ = InitializeCommand.ExecuteAsync(default);
            }
        }
        else
        {
            if (_moduleCaches.ContainsKey(type))
            {
                var data = _moduleCaches[type];
                foreach (var video in data)
                {
                    var videoVM = new VideoItemViewModel(video);
                    Items.Add(videoVM);
                }

                IsEmpty = Items.Count == 0;
            }
            else
            {
                _ = InitializeCommand.ExecuteAsync(default);
            }
        }

        ScrollToTop();
    }

    [RelayCommand]
    private void OpenPartition(PartitionItemViewModel vm)
    {
        if (PartitionId == vm.Data.Id && Items.Count > 0)
        {
            return;
        }

        TryClear(Items);
        PartitionId = vm.Data.Id;
        ChangeModuleType(PopularType.Partition);
    }

    private void CheckModuleState()
    {
        IsRecommendShown = CurrentType == PopularType.Recommend;
        IsHotShown = CurrentType == PopularType.Hot;
        IsRankShown = CurrentType == PopularType.Rank;
        IsInPartition = CurrentType == PopularType.Partition;

        foreach (var item in Partitions)
        {
            item.IsSelected = item.Data.Id == PartitionId;
        }

        Title = CurrentType switch
        {
            PopularType.Recommend => ResourceToolkit.GetLocalizedString(StringNames.Recommend),
            PopularType.Hot => ResourceToolkit.GetLocalizedString(StringNames.Hot),
            PopularType.Rank => ResourceToolkit.GetLocalizedString(StringNames.Rank),
            PopularType.Partition => Partitions.FirstOrDefault(p => p.IsSelected)?.Data.Name ?? string.Empty,
            _ => string.Empty,
        };
    }

    partial void OnNavListColumnWidthChanged(double value)
    {
        if (value >= 180)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.PopularNavListColumnWidth, value);
        }
    }
}
