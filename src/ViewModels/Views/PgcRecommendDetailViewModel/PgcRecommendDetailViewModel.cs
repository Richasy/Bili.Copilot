// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Pgc;
using Bili.Copilot.ViewModels.Items;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// PGC推荐内容详情视图模型.
/// </summary>
public partial class PgcRecommendDetailViewModel : InformationFlowViewModel<SeasonItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgcRecommendDetailViewModel"/> class.
    /// </summary>
    /// <param name="type">PGC类型.</param>
    internal PgcRecommendDetailViewModel(PgcType type)
    {
        _type = type;
        Title = _type switch
        {
            PgcType.Bangumi => ResourceToolkit.GetLocalizedString(StringNames.Bangumi),
            PgcType.Domestic => ResourceToolkit.GetLocalizedString(StringNames.DomesticAnime),
            PgcType.TV => ResourceToolkit.GetLocalizedString(StringNames.TV),
            PgcType.Movie => ResourceToolkit.GetLocalizedString(StringNames.Movie),
            PgcType.Documentary => ResourceToolkit.GetLocalizedString(StringNames.Documentary),
            _ => string.Empty,
        };
        Filters = new ObservableCollection<IndexFilterItemViewModel>();
    }

    /// <inheritdoc/>
    protected override void BeforeReload()
    {
        _isEnd = false;
        IsEmpty = false;
        if (_type is PgcType.Bangumi or PgcType.Domestic)
        {
            PgcProvider.Instance.ResetIndexStatus(_type);
        }
        else
        {
            PgcProvider.Instance.ResetPageStatus(_type);
        }
    }

    /// <inheritdoc/>
    protected override async Task GetDataAsync()
    {
        if (_isEnd)
        {
            return;
        }

        if (Filters.Count == 0)
        {
            var isAnime = _type == PgcType.Bangumi || _type == PgcType.Domestic;
            var filters = await PgcProvider.GetPgcIndexFiltersAsync(_type);
            foreach (var item in filters)
            {
                if (isAnime && item.Id == "area")
                {
                    var areaId = _type == PgcType.Bangumi ? "2" : "1,";
                    var selectedItem = item.Conditions.FirstOrDefault(p => p.Id.Contains(areaId));
                    Filters.Add(new IndexFilterItemViewModel(item, selectedItem));
                }
                else
                {
                    Filters.Add(new IndexFilterItemViewModel(item));
                }
            }
        }

        var seasons = new List<SeasonInformation>();
        if (_type is PgcType.Bangumi or PgcType.Domestic)
        {
            var (isFinished, items) = await PgcProvider.Instance.GetPgcIndexResultAsync(_type, GetAnimeParameters());
            seasons = items?.ToList();
            _isEnd = isFinished;
        }
        else
        {
            var view = await PgcProvider.Instance.GetPageDetailAsync(_type);
            if (view.Seasons?.Any() ?? false)
            {
                seasons = view.Seasons.ToList();
            }
        }

        seasons.ForEach(p =>
        {
            Items.Add(new SeasonItemViewModel(p));
        });

        IsEmpty = Items.Count == 0;
    }

    /// <inheritdoc/>
    protected override string FormatException(string errorMsg)
        => $"{ResourceToolkit.GetLocalizedString(StringNames.RequestFeedDetailFailed)}\n{errorMsg}";

    private Dictionary<string, string> GetAnimeParameters()
    {
        var queryPrameters = new Dictionary<string, string>();
        foreach (var item in Filters)
        {
            if (item.SelectedIndex >= 0)
            {
                var id = item.Data.Conditions.ToList()[item.SelectedIndex].Id;
                if (item.Data.Id == "year")
                {
                    id = Uri.EscapeDataString(id);
                }

                queryPrameters.Add(item.Data.Id, id);
            }
        }

        return queryPrameters;
    }
}
