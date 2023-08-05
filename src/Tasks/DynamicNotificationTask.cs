// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Dynamic;
using Bili.Copilot.Models.Data.Pgc;
using Bili.Copilot.Models.Data.Video;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Windows.ApplicationModel.Background;

namespace Bili.Copilot.Tasks;

/// <summary>
/// 动态通知后台任务.
/// </summary>
public sealed class DynamicNotificationTask : IBackgroundTask
{
    /// <inheritdoc/>
    public async void Run(IBackgroundTaskInstance taskInstance)
    {
        var deferral = taskInstance.GetDeferral();
        try
        {
            var isSigned = await AuthorizeProvider.Instance.IsTokenValidAsync();
            if (!isSigned)
            {
                deferral.Complete();
                return;
            }

            var dynamics = await CommunityProvider.Instance.GetDynamicVideoListAsync();
            if (dynamics is null || !dynamics.Dynamics.Any())
            {
                deferral.Complete();
                return;
            }

            var isFirstCheck = SettingsToolkit.ReadLocalSetting(SettingNames.IsFirstRunDynamicNotifyTask, true);
            var firstCard = dynamics.Dynamics.First();
            var cardList = dynamics.Dynamics.ToList();

            var lastReadId = SettingsToolkit.ReadLocalSetting(SettingNames.LastReadVideoDynamicId, string.Empty);

            // 初次检查或者未更新时不会进行通知
            if (isFirstCheck || lastReadId == firstCard.Id)
            {
                SettingsToolkit.WriteLocalSetting(SettingNames.IsFirstRunDynamicNotifyTask, false);
                SettingsToolkit.WriteLocalSetting(SettingNames.LastReadVideoDynamicId, firstCard.Id);
                deferral.Complete();
                return;
            }

            var lastReadCard = cardList.FirstOrDefault(p => p.Id == lastReadId);
            var lastReadIndex = cardList.IndexOf(lastReadCard);
            var notifyCards = new List<DynamicInformation>();

            if (lastReadIndex != -1)
            {
                for (var i = 0; i < lastReadIndex; i++)
                {
                    notifyCards.Add(cardList[i]);
                }
            }
            else
            {
                notifyCards = cardList;
            }

            var sendCount = 0;

            if (notifyCards.Count > 0)
            {
                SettingsToolkit.WriteLocalSetting(SettingNames.LastReadVideoDynamicId, firstCard.Id);

                foreach (var item in notifyCards)
                {
                    if (sendCount > 2)
                    {
                        break;
                    }

                    var isPgc = false;
                    var title = string.Empty;
                    var coverUrl = string.Empty;
                    var id = string.Empty;
                    var avatar = item.User.Avatar.GetSourceUri().ToString();
                    var desc = item.Description?.Text;
                    var timeLabel = string.IsNullOrEmpty(item.Tip)
                        ? "ms-resource:AppName"
                        : item.Tip;

                    if (item.Data is VideoInformation videoInfo)
                    {
                        id = videoInfo.Identifier.Id;
                        title = videoInfo.Identifier.Title;
                        coverUrl = videoInfo.Identifier.Cover.GetSourceUri().ToString();
                        if (string.IsNullOrEmpty(desc))
                        {
                            desc = videoInfo.Publisher?.User?.Name ?? string.Empty;
                        }
                    }
                    else if (item.Data is EpisodeInformation episodeInfo)
                    {
                        id = episodeInfo.VideoId;
                        title = episodeInfo.Identifier.Title;
                        coverUrl = episodeInfo.Identifier.Cover.GetSourceUri().ToString();
                        isPgc = true;
                        if (string.IsNullOrEmpty(desc))
                        {
                            desc = episodeInfo.Subtitle;
                        }
                    }

                    coverUrl += "@400w_250h_1c_100q.jpg";
                    avatar += "@100w_100h_1c_100q.jpg";

                    var notification = new AppNotificationBuilder()
                        .SetGroup("Bili.Copilot")
                        .SetHeroImage(new Uri(coverUrl))
                        .AddText(title, new AppNotificationTextProperties { MaxLines = 2 })
                        .AddText(desc, new AppNotificationTextProperties { MaxLines = 3 })
                        .SetScenario(AppNotificationScenario.Reminder)
                        .SetAttributionText(timeLabel)
                        .SetAppLogoOverride(new Uri(avatar), AppNotificationImageCrop.Circle)
                        .AddArgument("action", "play")
                        .AddArgument("id", id)
                        .AddArgument("isPgc", isPgc.ToString())
                        .BuildNotification();
                    AppNotificationManager.Default.Show(notification);
                    sendCount++;
                }

                if (notifyCards.Count > sendCount)
                {
                    // 弹出省略提示
                    var notification = new AppNotificationBuilder()
                        .SetGroup("Bili.Copilot")
                        .AddText("ms-resource:MoreInDynamic")
                        .AddArgument("action", "navigate")
                        .AddArgument("id", "Dynamic")
                        .SetScenario(AppNotificationScenario.Default)
                        .BuildNotification();
                    AppNotificationManager.Default.Show(notification);
                }
            }

            deferral.Complete();
        }
        catch (Exception)
        {
            deferral.Complete();
        }
    }
}
