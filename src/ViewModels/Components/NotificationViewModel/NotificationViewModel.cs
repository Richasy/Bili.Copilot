﻿// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Dynamic;
using Bili.Copilot.Models.Data.Pgc;
using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Windows.ApplicationModel;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace Bili.Copilot.ViewModels.Components;

/// <summary>
/// 通知视图模型.
/// </summary>
public sealed partial class NotificationViewModel : ViewModelBase
{
    private NotificationViewModel()
    {
        _timer = new Timer(TimeSpan.FromMinutes(15).TotalMilliseconds);
        _timer.Elapsed += OnTimerElapsedAsync;
    }

    [RelayCommand]
    private async Task CheckVideoDynamicNotificationsAsync()
    {
        var lastSeen = GetLastReadDynamicId();
        var latestDynamics = await CommunityProvider.Instance.GetDynamicVideoListAsync(true);
        if (latestDynamics != null)
        {
            var dynamics = latestDynamics.Dynamics.Where(p => p.DynamicType != Models.Constants.Community.DynamicItemType.Forward).ToList();
            var currentSeen = GetLastReadDynamicId();
            var notifiedItems = new List<DynamicInformation>();
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

                    notifiedItems.Add(d);
                    AppNotificationManager.Default.Show(notification);
                }
            }

            if (_isTileSupport)
            {
                if (notifiedItems.Count == 0)
                {
                    dynamics.Take(5).ToList().ForEach(notifiedItems.Add);
                }

                UpdateTile(notifiedItems);
            }
        }

        long GetLastReadDynamicId()
        {
            var lastReadId = SettingsToolkit.ReadLocalSetting(SettingNames.LastReadVideoDynamicId, string.Empty);
            return string.IsNullOrEmpty(lastReadId) ? 0 : Convert.ToInt64(lastReadId);
        }
    }

    [RelayCommand]
    private async Task TryStartAsync()
    {
        var isNotifyEnabled = SettingsToolkit.ReadLocalSetting(SettingNames.IsNotifyEnabled, true);
        if (isNotifyEnabled && !_timer.Enabled)
        {
            _timer.Start();
            await InitializeAsync();
            CheckVideoDynamicNotificationsCommand.Execute(default);
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

    private async Task InitializeAsync()
    {
        var entry = (await Package.Current.GetAppListEntriesAsync()).First();
        _isTileSupport = StartScreenManager.GetDefault().SupportsAppListEntry(entry);

        if (_isTileSupport)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
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
