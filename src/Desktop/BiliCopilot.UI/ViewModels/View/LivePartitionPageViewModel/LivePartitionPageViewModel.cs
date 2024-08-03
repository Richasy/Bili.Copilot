// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 直播分区页面视图模型.
/// </summary>
public sealed partial class LivePartitionPageViewModel : LayoutPageViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LivePartitionPageViewModel"/> class.
    /// </summary>
    public LivePartitionPageViewModel(
        ILiveDiscoveryService service,
        ILogger<LivePartitionPageViewModel> logger)
    {
        _service = service;
        _logger = logger;

        IsNavManualHideChanged(IsNavColumnManualHide);
    }

    /// <inheritdoc/>
    protected override string GetPageKey() => nameof(LivePartitionPage);

    /// <inheritdoc/>
    protected override void IsNavManualHideChanged(bool value)
    {
        base.IsNavManualHideChanged(value);
        IsSubNavEnabled = !value && SelectedMainSection?.Children?.Count > 0;
        SubNavColumnWidth = !IsSubNavEnabled ? 0 : SettingsToolkit.ReadLocalSetting($"{GetPageKey()}SubNavColumnWidth", 200d);
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (MainSections.Count > 1)
        {
            return;
        }

        IsSectionLoading = true;
        try
        {
            MainSections.Clear();
            var rcmdSection = new Partition("_", ResourceToolkit.GetLocalizedString(StringNames.RecommendLive));
            MainSections.Add(new PartitionViewModel(rcmdSection));
            var sections = await _service.GetLivePartitionsAsync();
            foreach (var section in sections)
            {
                MainSections.Add(new PartitionViewModel(section));
            }

            await SelectMainSectionAsync(MainSections.First());
            SectionInitialized?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取直播间分区时出现异常");
        }
        finally
        {
            IsSectionLoading = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        if (SelectedDetail is null)
        {
            _recommendOffset = 0;
            RecommendRooms.Clear();
            await LoadRecommendRoomsAsync();
        }
        else
        {
            await SelectedDetail.RefreshCommand.ExecuteAsync(default);
        }
    }

    [RelayCommand]
    private async Task SelectMainSectionAsync(PartitionViewModel vm)
    {
        if (vm is null || vm.Equals(SelectedMainSection))
        {
            return;
        }

        SelectedMainSection = vm;
        SelectedSubSection = default;
        IsNavManualHideChanged(IsNavColumnManualHide);
        if (vm.Data.Id == "_")
        {
            // 更新信息流.
            SelectedSubSection = vm;
            SelectedDetail = default;
            if (RecommendRooms.Count == 0)
            {
                await LoadRecommendRoomsAsync();
            }
        }
        else
        {
            var isSubpartitionEmpty = SubPartitions is null || SubPartitions.Count == 0;
            SubPartitions = vm.Children;
            if (_partitionSelectedCache.TryGetValue(vm.Data.Id, out var subSectionId))
            {
                var subSection = SubPartitions.FirstOrDefault(p => p.Data.Id == subSectionId);
                SelectSubSection(subSection);
            }
            else
            {
                SelectSubSection(SubPartitions.FirstOrDefault());
            }

            if (isSubpartitionEmpty)
            {
                // 等待 UI 渲染完毕.
                await Task.Delay(400);
            }
        }

        MainSectionChanged?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void SelectSubSection(PartitionViewModel vm)
    {
        if (vm is null || vm.Equals(SelectedSubSection))
        {
            return;
        }

        SelectedSubSection = vm;
        _partitionSelectedCache[SelectedMainSection.Data.Id] = vm.Data.Id;
        if (_partitionCache.TryGetValue(vm, out var detail))
        {
            SelectedDetail = detail;
        }
        else
        {
            var newVM = new LivePartitionDetailViewModel(vm, _service);
            _partitionCache.Add(vm, newVM);
            SelectedDetail = newVM;
            newVM.InitializeCommand.Execute(default);
        }
    }

    [RelayCommand]
    private async Task LoadRecommendRoomsAsync()
    {
        if (IsRecommendLoading)
        {
            return;
        }

        IsRecommendLoading = true;
        try
        {
            var (follows, recommends, nextPage) = await _service.GetFeedAsync(_recommendOffset);
            if (recommends is not null)
            {
                foreach (var item in recommends)
                {
                    RecommendRooms.Add(new LiveItemViewModel(item));
                }
            }

            if (follows is not null)
            {
                foreach (var item in follows)
                {
                    if (!FollowRooms.Any(p => p.Equals(item)))
                    {
                        FollowRooms.Add(new LiveItemViewModel(item));
                    }
                }
            }

            _recommendOffset = nextPage;
            RecommendUpdated?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试加载推荐直播间时出错.");
        }
        finally
        {
            IsRecommendLoading = false;
        }
    }

    partial void OnSubNavColumnWidthChanged(double value)
    {
        if (value > 0)
        {
            SettingsToolkit.WriteLocalSetting($"{GetPageKey()}SubNavColumnWidth", value);
        }
    }
}
