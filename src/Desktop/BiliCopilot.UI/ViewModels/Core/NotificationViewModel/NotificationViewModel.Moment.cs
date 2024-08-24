// Copyright (c) Bili Copilot. All rights reserved.

using System.Text.Json;
using System.Threading;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 通知视图模型.
/// </summary>
public sealed partial class NotificationViewModel
{
    [RelayCommand]
    private async Task TrySendVideoMomentNotificationsAsync()
    {
        var isVideoNotifyEnabled = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsVideoMomentNotificationEnabled, true);
        if (!isVideoNotifyEnabled)
        {
            return;
        }

        try
        {
            var lastMoments = await _momentService.GetVideoMomentsAsync(default, default, new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);
            if (lastMoments is null || lastMoments.Moments.Count == 0)
            {
                return;
            }

            LoadLastTenMomentIds();

            // 只取最新的三个动态.
            var newMoments = lastMoments.Moments.Take(3).ToList();
            foreach (var moment in newMoments)
            {
                if (_momentIds.Contains(moment.Id))
                {
                    continue;
                }

                var title = string.Empty;
                var cover = string.Empty;
                var avatar = string.Empty;
                var publisher = string.Empty;
                var pageType = string.Empty;
                var arguments = string.Empty;
                if (moment.Data is VideoInformation vinfo)
                {
                    title = vinfo.Identifier.Title;
                    cover = vinfo.Identifier.Cover.Uri.ToString();
                    pageType = typeof(VideoPlayerPage).FullName;
                    arguments = JsonSerializer.Serialize(vinfo.Identifier);
                }
                else if (moment.Data is EpisodeInformation einfo)
                {
                    title = einfo.Identifier.Title;
                    cover = einfo.Identifier.Cover.Uri.ToString();
                    pageType = typeof(PgcPlayerPage).FullName;
                    var hasEpid = einfo.Identifier.Id != "0";
                    if (hasEpid)
                    {
                        var identifier = new MediaIdentifier("ep_" + einfo.Identifier.Id, default, default);
                        arguments = JsonSerializer.Serialize(identifier);
                    }
                    else
                    {
                        arguments = string.Empty;
                    }
                }

                avatar = moment.User.Avatar.Uri.ToString();
                publisher = moment.User.Name;
                var notification = new AppNotificationBuilder()
                    .AddArgument("page", pageType)
                    .AddArgument("args", arguments)
                    .AddText(title)
                    .AddText(publisher)
                    .SetAppLogoOverride(new Uri(avatar), AppNotificationImageCrop.Circle)
                    .SetHeroImage(new Uri(cover))
                    .BuildNotification();

                AppNotificationManager.Default.Show(notification);
                await Task.Delay(1000);
            }

            // 尝试提取最新的10个动态id，如果不足10个，则从 _momentIds 中补充.
            var newMomentIds = newMoments.Select(p => p.Id).ToList();
            var momentIds = new List<string>();
            momentIds.AddRange(newMomentIds);
            momentIds.AddRange(_momentIds.Where(p => !newMomentIds.Contains(p)).Take(10 - newMomentIds.Count));

            SaveLastTenMomentIds(momentIds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试发送视频动态通知时发生错误.");
        }
    }

    /// <summary>
    /// 这里会读取保存在本地的最近十个动态Id.
    /// 如果新的动态Id不在这十个Id中,则会触发通知.
    /// </summary>
    private void LoadLastTenMomentIds()
    {
        var lastTenMomentIds = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.LastTenMomentIds, string.Empty);
        if (string.IsNullOrEmpty(lastTenMomentIds))
        {
            _momentIds = new List<string>();
            return;
        }

        _momentIds = lastTenMomentIds.Split(',').ToList();
    }

    private void SaveLastTenMomentIds(List<string> momentIds)
    {
        _momentIds = momentIds;
        var lastTenMomentIds = string.Join(',', momentIds);
        SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.LastTenMomentIds, lastTenMomentIds);
    }
}
