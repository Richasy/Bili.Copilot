// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Pgc;
using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;

namespace Bili.Copilot.ViewModels.Components;

/// <summary>
/// 通知视图模型.
/// </summary>
public sealed partial class NotificationViewModel : ViewModelBase
{
    private NotificationViewModel()
    {
        _timer = new Timer(TimeSpan.FromMinutes(5).TotalMilliseconds);
        _timer.Elapsed += OnTimerElapsedAsync;
    }

    private static async Task CheckVideoDynamicNotificationsAsync()
    {
        var lastSeen = Convert.ToInt64(SettingsToolkit.ReadLocalSetting(SettingNames.LastReadVideoDynamicId, string.Empty));
        var latestDynamics = await CommunityProvider.Instance.GetDynamicVideoListAsync(true);
        if (latestDynamics != null)
        {
            var dynamics = latestDynamics.Dynamics.Where(p => p.DynamicType != Models.Constants.Community.DynamicItemType.Forward).ToList();
            var currentSeen = Convert.ToInt64(SettingsToolkit.ReadLocalSetting(SettingNames.LastReadVideoDynamicId, string.Empty));
            if (currentSeen > lastSeen)
            {
                // 有更新，发送通知.
                for (var i = 0; i < 3; i++)
                {
                    if (i >= dynamics.Count() - 1)
                    {
                        break;
                    }

                    var d = dynamics.ElementAt(i);

                    if (Convert.ToInt64(d.Id) <= lastSeen)
                    {
                        break;
                    }

                    var title = string.Empty;
                    var cover = string.Empty;
                    var avatar = string.Empty;
                    var publisher = string.Empty;
                    if (d.Data is VideoInformation videoInfo)
                    {
                        title = videoInfo.Identifier.Title;
                        cover = videoInfo.Identifier.Cover.Uri;
                    }
                    else if (d.Data is EpisodeInformation episodeInfo)
                    {
                        title = episodeInfo.Identifier.Title;
                        cover = episodeInfo.Identifier.Cover.Uri;
                    }

                    avatar = d.User.Avatar.Uri;
                    publisher = d.User.Name;

                    var snapshot = DynamicItemViewModel.GetSnapshot(d.Data);
                    var notification = new AppNotificationBuilder()
                        .AddArgument("type", "dynamic")
                        .AddArgument("payload", JsonSerializer.Serialize(snapshot))
                        .AddText(title)
                        .AddText(publisher)
                        .SetAppLogoOverride(new Uri(avatar), AppNotificationImageCrop.Circle)
                        .SetHeroImage(new Uri(cover))
                        .BuildNotification();

                    AppNotificationManager.Default.Show(notification);
                }
            }
        }
    }

    [RelayCommand]
    private void TryStart()
    {
        var isNotifyEnabled = SettingsToolkit.ReadLocalSetting(SettingNames.IsNotifyEnabled, true);
        if (isNotifyEnabled && !_timer.Enabled)
        {
            _timer.Start();
        }
    }

    [RelayCommand]
    private void TryStop()
    {
        if (_timer.Enabled)
        {
            _timer.Stop();
        }
    }

    private async void OnTimerElapsedAsync(object sender, ElapsedEventArgs e)
    {
        var isNotifyEnabled = SettingsToolkit.ReadLocalSetting(SettingNames.IsNotifyEnabled, true);
        if (!isNotifyEnabled)
        {
            return;
        }

        try
        {
            var isDynamicNotifyEnabled = SettingsToolkit.ReadLocalSetting(SettingNames.DynamicNotificationEnabled, true);
            if (isDynamicNotifyEnabled)
            {
                await CheckVideoDynamicNotificationsAsync();
            }

            var isMessageNotifyEnabled = SettingsToolkit.ReadLocalSetting(SettingNames.MessageNotificationEnabled, true);
            if (isMessageNotifyEnabled)
            {
                // TODO: 发送消息通知
            }
        }
        catch (Exception ex)
        {
            LogException(ex);
        }
    }
}
