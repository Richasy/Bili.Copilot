// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Community;
using Bili.Copilot.Models.Data.Live;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 直播分区详情视图模型.
/// </summary>
public sealed partial class LivePartitionDetailViewModel : InformationFlowViewModel<LiveItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LivePartitionDetailViewModel"/> class.
    /// </summary>
    private LivePartitionDetailViewModel()
    {
        _caches = new Dictionary<LiveTag, IEnumerable<LiveInformation>>();
        Tags = new ObservableCollection<LiveTag>();

        AttachIsRunningToAsyncCommand(p => IsReloading = p, ReloadCommand, SelectTagCommand, InitializeCommand);
    }

    /// <inheritdoc/>
    protected override void BeforeReload()
        => LiveProvider.Instance.ResetPartitionDetailState();

    /// <inheritdoc/>
    protected override string FormatException(string errorMsg)
        => $"{ResourceToolkit.GetLocalizedString(StringNames.RequestSubPartitionFailed)}\n{errorMsg}";

    /// <inheritdoc/>
    protected override async Task GetDataAsync()
    {
        if (_totalCount >= 0 && Items.Count >= _totalCount)
        {
            return;
        }

        IsEmpty = false;
        var sortType = CurrentTag == null
            ? string.Empty
            : CurrentTag.SortType;
        var data = await LiveProvider.Instance.GetLiveAreaDetailAsync(OriginPartition.Id, OriginPartition.ParentId, sortType);

        if (data.Tags?.Count() > 0 && Tags.Count == 0)
        {
            data.Tags.ToList().ForEach(Tags.Add);
            await Task.Delay(100);
            CurrentTag = Tags.First();
        }

        _totalCount = data.TotalCount;

        var canScrollToTop = Items.Count == 0;

        if (data.Lives?.Count() > 0)
        {
            foreach (var live in data.Lives)
            {
                var liveVM = new LiveItemViewModel(live);
                Items.Add(liveVM);
            }

            var lives = Items
                    .Select(p => p.Data)
                    .ToList();
            if (_caches.ContainsKey(CurrentTag))
            {
                _caches[CurrentTag] = lives;
            }
            else
            {
                _caches.Add(CurrentTag, lives);
            }
        }

        IsEmpty = Items.Count == 0;
        if (canScrollToTop && !IsEmpty)
        {
            RequestScrollToTop?.Invoke(this, EventArgs.Empty);
        }
    }

    [RelayCommand]
    private void SetPartition(Partition partition)
    {
        OriginPartition = partition;
        _caches.Clear();
        TryClear(Tags);
        LiveProvider.Instance.ResetPartitionDetailState();
        CurrentTag = default;
        _totalCount = -1;
        IsEmpty = false;
        TryClear(Items);
        InitializeCommand.Execute(default);
    }

    [RelayCommand]
    private async Task SelectTagAsync(LiveTag tag)
    {
        await Task.Delay(100);
        TryClear(Items);
        CurrentTag = tag;
        if (_caches.ContainsKey(tag))
        {
            var data = _caches[tag];
            foreach (var live in data)
            {
                var liveVM = new LiveItemViewModel(live);
                Items.Add(liveVM);
            }

            RequestScrollToTop?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            _ = ReloadCommand.ExecuteAsync(null);
        }
    }
}
