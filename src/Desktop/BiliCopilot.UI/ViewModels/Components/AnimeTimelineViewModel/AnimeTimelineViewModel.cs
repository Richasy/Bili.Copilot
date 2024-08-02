// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using BiliCopilot.UI.ViewModels.View;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 动漫时间线视图模型.
/// </summary>
public sealed partial class AnimeTimelineViewModel : ViewModelBase, IAnimeSectionDetailViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnimeTimelineViewModel"/> class.
    /// </summary>
    public AnimeTimelineViewModel(
        IEntertainmentDiscoveryService discoveryService)
    {
        _service = discoveryService;
        _logger = this.Get<ILogger<AnimeTimelineViewModel>>();
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Timelines.Count > 0)
        {
            return;
        }

        IsTimelineLoading = true;
        IReadOnlyList<TimelineInformation> bangumiTimelines = default;
        IReadOnlyList<TimelineInformation> domesticTimelines = default;
        var tasks = new List<Task>
        {
            Task.Run(async () =>
            {
                var (_, _, timelines) = await _service.GetBangumiTimelineAsync();
                bangumiTimelines = timelines;
            }),
            Task.Run(async () =>
            {
                var (_, _, timelines) = await _service.GetDomesticTimelineAsync();
                domesticTimelines = timelines;
            }),
        };

        try
        {
            await Task.WhenAll(tasks);

            var combinedTimelines = new List<TimelineItemViewModel>();
            var todayIndex = bangumiTimelines.ToList().IndexOf(bangumiTimelines.FirstOrDefault(p => p.IsToday));
            for (var i = 0; i < bangumiTimelines.Count; i++)
            {
                var domestic = domesticTimelines[i];
                var bangumi = bangumiTimelines[i];
                var seasons = new List<SeasonInformation>();
                if (domestic.Seasons is not null)
                {
                    seasons = seasons.Concat(domestic.Seasons).ToList();
                }

                if (bangumi.Seasons is not null)
                {
                    seasons = seasons.Concat(bangumi.Seasons).ToList();
                }

                seasons = seasons.OrderBy(p => p.GetExtensionIfNotNull<DateTimeOffset>(SeasonExtensionDataId.PublishTime)).ToList();
                var combined = new TimelineInformation(
                    bangumi.Date,
                    bangumi.DayOfWeek,
                    bangumi.TimeStamp,
                    bangumi.IsToday,
                    seasons);
                combinedTimelines.Add(new TimelineItemViewModel(combined));
            }

            foreach (var item in combinedTimelines)
            {
                Timelines.Add(item);
            }

            var todayItem = Timelines[todayIndex];
            SelectTimelineCommand.Execute(todayItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取动漫时间线失败.");
        }
        finally
        {
            IsTimelineLoading = false;
            TimelineInitialized?.Invoke(this, EventArgs.Empty);
        }
    }

    [RelayCommand]
    private void SelectTimeline(TimelineItemViewModel vm)
    {
        if (vm is null || SelectedTimeline == vm)
        {
            return;
        }

        var pageVM = this.Get<AnimePageViewModel>();
        pageVM.Title = vm.DayOfWeek;
        pageVM.Subtitle = vm.Date;
        SelectedTimeline = vm;
    }
}
