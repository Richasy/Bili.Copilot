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
/// 影视圈页面视图模型.
/// </summary>
public sealed partial class FilmPageViewModel : InformationFlowViewModel<SeasonItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FilmPageViewModel"/> class.
    /// </summary>
    private FilmPageViewModel()
    {
        _seasonCaches = new Dictionary<FilmType, IEnumerable<SeasonInformation>>();
        CurrentType = SettingsToolkit.ReadLocalSetting(SettingNames.LastFilmType, FilmType.Movie);
        CheckModuleState();
    }

    /// <inheritdoc/>
    protected override void BeforeReload()
    {
        var pgcType = GetPgcType();
        PgcProvider.Instance.ResetPageStatus(pgcType);
    }

    /// <inheritdoc/>
    protected override async Task GetDataAsync()
    {
        var pgcType = GetPgcType();
        var data = await PgcProvider.Instance.GetPageDetailAsync(pgcType);

        if (data.Seasons != null && data.Seasons.Count() > 0)
        {
            data.Seasons.ToList().ForEach(p =>
            {
                Items.Add(new SeasonItemViewModel(p));
            });

            _seasonCaches[CurrentType] = Items.Select(p => p.Data).ToList();
        }

        IsEmpty = Items.Count == 0;
    }

    /// <inheritdoc/>
    protected override string FormatException(string errorMsg)
        => $"{ResourceToolkit.GetLocalizedString(StringNames.RequestFeedDetailFailed)}\n{errorMsg}";

    private void CheckModuleState()
    {
        IsMovieShown = CurrentType == FilmType.Movie;
        IsTvShown = CurrentType == FilmType.Tv;
        IsDocumentaryShown = CurrentType == FilmType.Documentary;

        Title = CurrentType switch
        {
            FilmType.Movie => ResourceToolkit.GetLocalizedString(StringNames.Movie),
            FilmType.Tv => ResourceToolkit.GetLocalizedString(StringNames.TV),
            FilmType.Documentary => ResourceToolkit.GetLocalizedString(StringNames.Documentary),
            _ => string.Empty,
        };
    }

    private PgcType GetPgcType()
    {
        return CurrentType switch
        {
            FilmType.Movie => PgcType.Movie,
            FilmType.Tv => PgcType.TV,
            FilmType.Documentary => PgcType.Documentary,
            _ => PgcType.Movie,
        };
    }

    partial void OnCurrentTypeChanged(FilmType value)
    {
        CheckModuleState();
        SettingsToolkit.WriteLocalSetting(SettingNames.LastFilmType, value);

        if (!IsInitialized)
        {
            return;
        }

        TryClear(Items);
        if (_seasonCaches.ContainsKey(value))
        {
            var data = _seasonCaches[value];
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
