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
/// 动漫页面的视图模型.
/// </summary>
public sealed partial class AnimePageViewModel : InformationFlowViewModel<SeasonItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnimePageViewModel"/> class.
    /// </summary>
    private AnimePageViewModel()
    {
        _caches = new Dictionary<AnimeDisplayType, IEnumerable<SeasonInformation>>();
        CurrentType = SettingsToolkit.ReadLocalSetting(SettingNames.LastAnimeType, AnimeDisplayType.Timeline);
        CheckModuleState();
    }

    /// <inheritdoc/>
    protected override void BeforeReload()
        => PgcProvider.Instance.ResetIndexStatus();

    /// <inheritdoc/>
    protected override async Task GetDataAsync()
    {
        if ((IsBangumiShown && _isBangumiFinished)
             || (IsDomesticShown && _isDomesticFinished))
        {
            return;
        }

        var pgcType = GetPgcType();
        var parameters = GetDefaultIndexParameters();
        var (isFinished, items) = await PgcProvider.Instance.GetPgcIndexResultAsync(pgcType, parameters);
        if (IsBangumiShown)
        {
            _isBangumiFinished = isFinished;
        }
        else
        {
            _isDomesticFinished = isFinished;
        }

        if (items != null && items.Count() > 0)
        {
            items.ToList().ForEach(p =>
            {
                Items.Add(new SeasonItemViewModel(p));
            });

            _caches[CurrentType] = Items.Select(p => p.Data).ToList();
        }

        IsEmpty = Items.Count == 0;
    }

    /// <inheritdoc/>
    protected override string FormatException(string errorMsg)
        => $"{ResourceToolkit.GetLocalizedString(StringNames.RequestFeedDetailFailed)}\n{errorMsg}";

    private void CheckModuleState()
    {
        IsTimelineShown = CurrentType == AnimeDisplayType.Timeline;
        IsBangumiShown = CurrentType == AnimeDisplayType.Bangumi;
        IsDomesticShown = CurrentType == AnimeDisplayType.Domestic;

        Title = CurrentType switch
        {
            AnimeDisplayType.Timeline => ResourceToolkit.GetLocalizedString(StringNames.TimeChart),
            AnimeDisplayType.Bangumi => ResourceToolkit.GetLocalizedString(StringNames.Bangumi),
            AnimeDisplayType.Domestic => ResourceToolkit.GetLocalizedString(StringNames.DomesticAnime),
            _ => string.Empty,
        };
    }

    private PgcType GetPgcType()
    {
        return CurrentType switch
        {
            AnimeDisplayType.Bangumi => PgcType.Bangumi,
            AnimeDisplayType.Domestic => PgcType.Domestic,
            _ => PgcType.Bangumi,
        };
    }

    private Dictionary<string, string> GetDefaultIndexParameters()
    {
        var area = IsBangumiShown ? "2" : "1,6,7";
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

    partial void OnCurrentTypeChanged(AnimeDisplayType value)
    {
        CheckModuleState();
        SettingsToolkit.WriteLocalSetting(SettingNames.LastAnimeType, value);

        if (IsTimelineShown)
        {
            TimelineViewModel.Instance.InitializeCommand.Execute(default);
        }
        else
        {
            TryClear(Items);
            if (_caches.ContainsKey(value))
            {
                var data = _caches[value];
                foreach (var season in data)
                {
                    var seasonVM = new SeasonItemViewModel(season);
                    Items.Add(seasonVM);
                }

                IsEmpty = Items.Count == 0;
            }
            else
            {
                InitializeCommand.ExecuteAsync(default);
            }
        }
    }
}
