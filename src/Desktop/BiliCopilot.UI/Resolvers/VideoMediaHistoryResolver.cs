// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.Toolkits;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models.Media;
using Richasy.MpvKernel.Core.Enums;
using Richasy.MpvKernel.Player;

namespace BiliCopilot.UI.Resolvers;

internal sealed partial class VideoMediaHistoryResolver(IPlayerService playerService, ILogger<VideoMediaHistoryResolver> logger) : IMpvMediaHistoryResolver
{
    private VideoPlayerView _view;
    private VideoPart _part;
    private MediaSnapshot _snapshot;

    public void Initialize(VideoPlayerView view, VideoPart part, MediaSnapshot snapshot)
    {
        _view = view;
        _part = part;
        _snapshot = snapshot;
    }

    public Task<double> GetStartPositionAsync()
    {
        if (_view is null)
        {
            throw new InvalidOperationException("视频信息未初始化.");
        }

        var progress = _view.Progress?.Progress ?? 0d;
        return Task.FromResult(progress >= _view.Information.Duration - 5 ? 0d : progress);
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
            await playerService.ReportVideoProgressAsync(_view.Information.Identifier.Id, _part.Identifier.Id, Convert.ToInt32(position), ct.Token);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"上报 {_view.Information.Identifier.Id} 进度时失败");
        }
    }
}
