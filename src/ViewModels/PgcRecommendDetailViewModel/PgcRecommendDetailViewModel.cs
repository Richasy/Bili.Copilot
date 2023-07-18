// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Pgc;

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
        => _type = type;

    /// <inheritdoc/>
    protected override void BeforeReload()
    {
        _isEnd = false;
        IsEmpty = false;
        if (_type == PgcType.Bangumi || _type == PgcType.Domestic)
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

        var seasons = new List<SeasonInformation>();
        if (_type == PgcType.Bangumi || _type == PgcType.Domestic)
        {
            var (isFinished, items) = await PgcProvider.Instance.GetPgcIndexResultAsync(_type, GetDefaultAnimeParameters());
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

    private Dictionary<string, string> GetDefaultAnimeParameters()
    {
        var area = _type == PgcType.Bangumi ? "2" : "1,6,7";
        return new Dictionary<string, string>
        {
            { "order", "8" },
            { "style_id", "-1" },
            { "area", area },
            { "is_finish", "-1" },
            { "year", "-1" },
            { "season_month", "-1" },
            { "copyright", "-1" },
            { "spoken_language_type", "-1" },
            { "season_status", "-1" },
            { "season_version", "-1" },
        };
    }
}
