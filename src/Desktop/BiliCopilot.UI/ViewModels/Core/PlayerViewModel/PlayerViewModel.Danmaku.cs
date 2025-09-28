// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models.Danmaku;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.ViewModels.Core;

public sealed partial class PlayerViewModel
{
    [RelayCommand]
    private async Task InitializeDanmakuAsync()
    {
        SettingsToolkit.DeleteLocalSetting(Models.Constants.SettingNames.IsDanmakuEnabled);
        IsDanmakuEnabled = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsDanmakuEnabled, true);
        IsDanmakuControlVisible = IsDanmakuEnabled;
        if (!IsDanmakuEnabled)
        {
            return;
        }

        IsDanmakuLoading = true;
        // 使用限流器，最多同时请求 6 个分段弹幕.
        var semaphore = new SemaphoreSlim(6, 6);
        var danmakus = new List<DanmakuInformation>();
        try
        {
            if (_snapshot.Type == Models.Constants.BiliMediaType.Video)
            {
                var connector = Connector as VideoConnectorViewModel;
                var aid = connector._view.Information.Identifier.Id;
                var cid = connector._part.Identifier.Id;
                Danmaku.Title = connector._view.Information.Identifier.Title;
                var totalMinutes = Math.Ceiling(TimeSpan.FromSeconds(connector._view.Information.Duration ?? 0).TotalMinutes);
                var tasks = new List<Task>();
                var segmentCount = Convert.ToInt32(Math.Ceiling(totalMinutes / 6.0)); // 6 minutes per segment
                for (var i = 0; i < segmentCount; i++)
                {
                    var index = i + 1;
                    tasks.Add(Task.Run(async () => await GetDanmakuAsync(index, aid, cid)));
                }

                await Task.WhenAll(tasks);
            }
            else if (_snapshot.Type == Models.Constants.BiliMediaType.Pgc)
            {
                var connector = Connector as PgcConnectorViewModel;
                var aid = connector._episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Aid).ToString();
                var cid = connector._episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Cid).ToString();
                var totalMinutes = Math.Ceiling(TimeSpan.FromSeconds(connector._episode.Duration ?? 0).TotalMinutes);
                var tasks = new List<Task>();
                var segmentCount = Convert.ToInt32(Math.Ceiling(totalMinutes / 6.0)); // 6 minutes per segment
                for (var i = 0; i < segmentCount; i++)
                {
                    var index = i + 1;
                    tasks.Add(Task.Run(async () => await GetDanmakuAsync(index, aid, cid)));
                }

                Danmaku.Title = $"{connector.SeasonTitle} - {connector.EpisodeTitle}";
                await Task.WhenAll(tasks);
            }

            Danmaku.DanmakuCountText = danmakus.Count > 0
                ? string.Format(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.DanmakuCountTemplate), danmakus.Count.ToString("N0"))
                : ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.DanmakuEmpty);
            Danmaku.Initialize(danmakus);
            IsDanmakuLoading = false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize danmaku.");
            IsDanmakuLoading = false;
            IsDanmakuControlVisible = false;
        }

        async Task GetDanmakuAsync(int index, string aid, string cid)
        {
            try
            {
                await semaphore.WaitAsync();
                var danmaku = await this.Get<IDanmakuService>().GetSegmentDanmakusWithGrpcAsync(aid, cid, index);
                lock (danmakus)
                {
                    if (danmaku?.Count > 0)
                    {
                        danmakus.AddRange(danmaku);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"获取弹幕分片失败： {index}");
            }

            semaphore.Release();
        }
    }

    [RelayCommand]
    private async Task ReloadDanmakuAsync()
    {
        if (Player is null || Window is null || Window.IsClosed || !Player.IsPlaybackInitialized || Danmaku is null)
        {
            return;
        }

        Danmaku.ClearAll();
        await InitializeDanmakuAsync();
    }
}
