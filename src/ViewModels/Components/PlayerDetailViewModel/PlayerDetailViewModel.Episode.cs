// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Player;
using Bili.Copilot.Models.Data.Pgc;
using Bili.Copilot.Models.Data.Video;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 媒体播放器视图模型.
/// </summary>
public sealed partial class PlayerDetailViewModel
{
    private async Task ChangeEpisodeAsync(VideoIdentifier identifier)
    {
        var view = _viewData as PgcPlayerView;
        Cover = view.Information.Identifier.Cover.GetSourceUri().ToString();
        if (string.IsNullOrEmpty(identifier.Id))
        {
            return;
        }

        if (view.Episodes != null && view.Episodes.Any(p => p.Identifier.Id == identifier.Id))
        {
            _currentEpisode = view.Episodes.First(p => p.Identifier.Id == identifier.Id);
        }
        else if (view.Extras != null)
        {
            var episodes = view.Extras.SelectMany(p => p.Value);
            _currentEpisode = episodes.FirstOrDefault(p => p.Identifier.Id == identifier.Id);
        }

        if (_currentEpisode == null)
        {
            IsError = true;
            ErrorText = view.Warning ?? ResourceToolkit.GetLocalizedString(StringNames.RequestPgcFailed);
        }

        await LoadEpisodeAsync();
    }

    private async Task LoadEpisodeAsync()
    {
        var view = _viewData as PgcPlayerView;
        if (_currentEpisode == null)
        {
            IsError = true;
            ErrorText = view.Warning ?? ResourceToolkit.GetLocalizedString(StringNames.RequestPgcFailed);
            return;
        }

        SubtitleViewModel.SetData(_currentEpisode.VideoId, _currentEpisode.PartId);
        DanmakuViewModel.SetData(_currentEpisode.VideoId, _currentEpisode.PartId, _videoType);
        DownloadViewModel.SetData("ss" + view.Information.Identifier.Id, view.Episodes.Select(p => p.Identifier));
        await InitializeEpisodeMediaInformationAsync();
        CheckEpisodeHistory();
        InitializeOriginalVideoSource();
    }

    private void CheckEpisodeHistory()
    {
        var view = _viewData as PgcPlayerView;
        if (view.Progress != null && view.Progress.Status == PlayedProgressStatus.Playing)
        {
            var history = view.Progress.Identifier;
            if (_currentEpisode != null && history.Id == _currentEpisode.Identifier.Id)
            {
                _initializeProgress = TimeSpan.FromSeconds(view.Progress.Progress);
            }
            else
            {
                var ts = TimeSpan.FromSeconds(view.Progress.Progress);
                IsShowProgressTip = true;
                ProgressTip = $"{ResourceToolkit.GetLocalizedString(StringNames.PreviousView)}{history.Title} {ts}";
            }
        }
    }

    private async Task InitializeEpisodeMediaInformationAsync()
    {
        var view = _viewData as PgcPlayerView;
        var proxy = AppToolkit.GetProxyAndArea(view.Information.Identifier.Title, true);
        _mediaInformation = await PlayerProvider.GetPgcMediaInformationAsync(
            _currentEpisode.PartId,
            _currentEpisode.Identifier.Id,
            _currentEpisode.SeasonType,
            proxy.Item1,
            proxy.Item2);
    }
}
