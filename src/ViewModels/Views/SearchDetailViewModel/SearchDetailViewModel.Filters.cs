// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Appearance;
using static Bili.Copilot.Models.App.Constants.ServiceConstants.Search;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 搜索页面视图模型.
/// </summary>
public sealed partial class SearchDetailViewModel
{
    private static SearchFilterViewModel GetFilterViewModel(Filter filter, Condition condition = null)
        => new SearchFilterViewModel(filter, condition);

    private async Task InitializeFiltersAsync(SearchModuleType type)
    {
        _ = _filters.Remove(type);
        if (type == SearchModuleType.Video)
        {
            await InitializeVideoFiltersAsync();
        }
        else if (type == SearchModuleType.Article)
        {
            await InitializeArticleFiltersAsync();
        }
        else if (type == SearchModuleType.User)
        {
            InitializeUserFilters();
        }
        else
        {
            _filters.Add(type, new List<SearchFilterViewModel>());
        }
    }

    private async Task InitializeVideoFiltersAsync()
    {
        var orderFilter = new Filter("排序", OrderType, new List<Condition>
        {
            new Condition(ResourceToolkit.GetLocalizedString(StringNames.SortByDefault), "default"),
            new Condition(ResourceToolkit.GetLocalizedString(StringNames.SortByPlay), "view"),
            new Condition(ResourceToolkit.GetLocalizedString(StringNames.SortByNewest), "pubdate"),
            new Condition(ResourceToolkit.GetLocalizedString(StringNames.SortByDanmaku), "danmaku"),
        });

        var durationFilter = new Filter("时长", Duration, new List<Condition>
        {
            new Condition(ResourceToolkit.GetLocalizedString(StringNames.FilterByTotalDuration), "0"),
            new Condition(ResourceToolkit.GetLocalizedString(StringNames.FilterByLessThan10Min), "1"),
            new Condition(ResourceToolkit.GetLocalizedString(StringNames.FilterByLessThan30Min), "2"),
            new Condition(ResourceToolkit.GetLocalizedString(StringNames.FilterByLessThan60Min), "3"),
            new Condition(ResourceToolkit.GetLocalizedString(StringNames.FilterByGreaterThan60Min), "4"),
        });

        var totalPartitions = await HomeProvider.GetVideoPartitionIndexAsync();
        var partitionConditions = totalPartitions
            .Select(p => new Condition(p.Name, p.Id))
            .ToList();
        partitionConditions.Insert(0, new Condition(ResourceToolkit.GetLocalizedString(StringNames.Total), "0"));
        var partitionFilter = new Filter("分区", PartitionId, partitionConditions);

        var orderVM = GetFilterViewModel(orderFilter);
        var durationVM = GetFilterViewModel(durationFilter);
        var partitionVM = GetFilterViewModel(partitionFilter);
        _filters.Add(SearchModuleType.Video, new List<SearchFilterViewModel> { orderVM, durationVM, partitionVM });
    }

    private async Task InitializeArticleFiltersAsync()
    {
        var orderFilter = new Filter("排序", OrderType, new List<Condition>
        {
            new Condition(ResourceToolkit.GetLocalizedString(StringNames.SortByDefault), string.Empty),
            new Condition(ResourceToolkit.GetLocalizedString(StringNames.SortByNewest), "pubdate"),
            new Condition(ResourceToolkit.GetLocalizedString(StringNames.SortByRead), "click"),
            new Condition(ResourceToolkit.GetLocalizedString(StringNames.SortByReply), "scores"),
            new Condition(ResourceToolkit.GetLocalizedString(StringNames.SortByLike), "attention"),
        });

        var totalPartitions = await ArticleProvider.GetPartitionsAsync();
        var partitionConditions = totalPartitions
            .Select(p => new Condition(p.Name, p.Id))
            .ToList();
        var partitionFilter = new Filter("分区", PartitionId, partitionConditions);

        var orderVM = GetFilterViewModel(orderFilter);
        var partitionVM = GetFilterViewModel(partitionFilter);
        _filters.Add(SearchModuleType.Article, new List<SearchFilterViewModel> { orderVM, partitionVM });
    }

    private void InitializeUserFilters()
    {
        var orderFilter = new Filter("排序", OrderType, new List<Condition>
        {
            new Condition(ResourceToolkit.GetLocalizedString(StringNames.SortByDefault), "totalrank_0"),
            new Condition(ResourceToolkit.GetLocalizedString(StringNames.SortByFansHTL), "fan_0"),
            new Condition(ResourceToolkit.GetLocalizedString(StringNames.SortByFansLTH), "fan_1"),
            new Condition(ResourceToolkit.GetLocalizedString(StringNames.SortByLevelHTL), "level_0"),
            new Condition(ResourceToolkit.GetLocalizedString(StringNames.SortByLevelLTH), "level_1"),
        });

        var typeFilter = new Filter("用户类型", UserType, new List<Condition>
        {
            new Condition(ResourceToolkit.GetLocalizedString(StringNames.TotalUser), "0"),
            new Condition(ResourceToolkit.GetLocalizedString(StringNames.UpMaster), "1"),
            new Condition(ResourceToolkit.GetLocalizedString(StringNames.NormalUser), "2"),
            new Condition(ResourceToolkit.GetLocalizedString(StringNames.OfficialUser), "3"),
        });

        var orderVM = GetFilterViewModel(orderFilter);
        var typeVM = GetFilterViewModel(typeFilter);
        _filters.Add(SearchModuleType.User, new List<SearchFilterViewModel> { orderVM, typeVM });
    }
}
