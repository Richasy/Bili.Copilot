// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Pgc;
using Bili.Copilot.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 动漫时间线视图模型.
/// </summary>
public sealed partial class TimelineViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TimelineViewModel"/> class.
    /// </summary>
    private TimelineViewModel()
    {
        TimelineCollection = new ObservableCollection<TimelineItemViewModel>();
        SeasonCollection = new ObservableCollection<SeasonInformation>();

        AttachIsRunningToAsyncCommand(p => IsReloading = p, ReloadCommand);
        AttachExceptionHandlerToAsyncCommand(DisplayException, ReloadCommand, InitializeCommand);
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (TimelineCollection.Count > 0 || IsReloading)
        {
            return;
        }

        IsReloading = true;
        try
        {
            await ReloadAsync();
        }
        catch (Exception ex)
        {
            LogException(ex);
        }

        IsReloading = false;
    }

    [RelayCommand]
    private async Task ReloadAsync()
    {
        TryClear(TimelineCollection);
        TryClear(SeasonCollection);
        var bangumiTimelines = await PgcProvider.GetPgcTimelinesAsync(PgcType.Bangumi);
        var domesticTimelines = await PgcProvider.GetPgcTimelinesAsync(PgcType.Domestic);
        foreach (var bangumiTimeline in bangumiTimelines.Timelines)
        {
            var seasons = new List<SeasonInformation>();
            if (bangumiTimeline.Seasons != null)
            {
                seasons.AddRange(bangumiTimeline.Seasons);
            }

            var domesticTimeline = domesticTimelines.Timelines.FirstOrDefault(p => p.Date == bangumiTimeline.Date);
            if (domesticTimeline != null && domesticTimeline.Seasons != null && domesticTimeline.Seasons.Count() > 0)
            {
                seasons.AddRange(domesticTimeline.Seasons);
            }

            seasons = seasons.Any() ? seasons : null;
            TimelineCollection.Add(new TimelineItemViewModel(new TimelineInformation(bangumiTimeline.Date, bangumiTimeline.DayOfWeek, bangumiTimeline.IsToday, seasons)));
        }

        OpenTimeline(TimelineCollection.FirstOrDefault(p => p.Data.IsToday));
    }

    [RelayCommand]
    private void OpenTimeline(TimelineItemViewModel vm)
    {
        if (vm == null)
        {
            return;
        }

        foreach (var item in TimelineCollection)
        {
            item.IsSelected = item.Equals(vm);
        }

        SelectedTimeline = vm;

        TryClear(SeasonCollection);
        foreach (var item in vm.Data.Seasons)
        {
            SeasonCollection.Add(item);
        }

        IsSeasonEmpty = SeasonCollection.Count == 0;
    }

    private void DisplayException(Exception exception)
    {
        IsError = true;
        IsReloading = false;
        var msg = exception is ServiceException se
            ? se.GetMessage()
            : exception.Message;
        ErrorText = $"{ResourceToolkit.GetLocalizedString(StringNames.RequestPgcTimeLineFailed)}\n{msg}";
        LogException(exception);
    }
}
