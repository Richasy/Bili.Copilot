// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.Toolkits;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models.Media;
using Richasy.MpvKernel.Core.Enums;
using Richasy.MpvKernel.Player;

namespace BiliCopilot.UI.Resolvers;

internal sealed partial class PgcMediaHistoryResolver(IPlayerService playerService, ILogger<PgcMediaHistoryResolver> logger) : IMpvMediaHistoryResolver
{
    private PgcPlayerView _view;
    private EpisodeInformation _episode;
    private MediaSnapshot _snapshot;

    public void Initialize(PgcPlayerView view, EpisodeInformation episode, MediaSnapshot snapshot)
    {
        _view = view;
        _episode = episode;
        _snapshot = snapshot;
    }

    public Task<double> GetStartPositionAsync()
    {
        if (_view is null)
        {
            throw new InvalidOperationException("视频信息未初始化.");
        }

        var progress = _view.Progress?.Progress ?? 0d;
        if (_view.Progress?.Cid != _episode.Identifier.Id)
        {
            return Task.FromResult(0d);
        }

        return Task.FromResult(progress >= _episode.Duration - 5 ? 0d : progress);
    }

    public async Task SaveHistoryAsync(double position, double duration, MpvPlayerState state, bool isExiting = false)
    {
        var shouldReport = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.ShouldReportProgress, true);
        if (!shouldReport || _snapshot.IsPrivate || position == 0 || state == MpvPlayerState.Paused)
        {
            return;
        }

        try
        {
            var ct = new CancellationTokenSource(TimeSpan.FromSeconds(4));
            var aid = _episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Aid);
            var cid = _episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Cid);
            await playerService.ReportEpisodeProgressAsync(aid.ToString(), cid.ToString(), _episode.Identifier.Id, _view.Information.Identifier.Id, Convert.ToInt32(position), ct.Token);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"上报 {_view.Information.Identifier.Title} 进度时失败");
        }
    }
}
