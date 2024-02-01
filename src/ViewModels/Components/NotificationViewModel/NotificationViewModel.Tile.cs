// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Security;
using Bili.Copilot.Models.Data.Dynamic;
using Bili.Copilot.Models.Data.Pgc;
using Bili.Copilot.Models.Data.Video;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Bili.Copilot.ViewModels.Components;

/// <summary>
/// 通知视图模型.
/// </summary>
public sealed partial class NotificationViewModel
{
    /// <summary>
    /// 更新磁贴.
    /// </summary>
    public static void UpdateTile(List<DynamicInformation> dynamics)
    {
        foreach (var d in dynamics)
        {
            var title = string.Empty;
            var cover = string.Empty;
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

            var avatar = d.User.Avatar.Uri;
            var publisher = d.User.Name;

            var xmlContent = $"""
                <?xml version="1.0" encoding="utf-8"?>
                <tile>
                    <visual>
                        <binding template="TileMedium" branding="name">
                            <image src="{cover}" placement="background" hint-overlay="60" />
                            <image src="{avatar}" placement="peek" hint-crop="circle" />
                            <text hint-style="caption" hint-wrap="true">{SecurityElement.Escape(title)}</text>
                        </binding>
                        <binding template="TileWide" branding="nameAndLogo">
                            <image src="{cover}" placement="background" hint-overlay="60" />
                            <text hint-maxLines="2" hint-style="base" hint-wrap="true">{SecurityElement.Escape(title)}</text>
                            <text hint-style="caption">{SecurityElement.Escape(publisher)}</text>
                        </binding>
                        <binding template="TileLarge" branding="nameAndLogo">
                            <image src="{cover}" placement="background" hint-overlay="60" />
                            <text hint-style="subtitle" hint-wrap="true">{SecurityElement.Escape(title)}</text>
                            <text hint-style="base"></text>
                            <text hint-style="base">{SecurityElement.Escape(publisher)}</text>
                        </binding>
                    </visual>
                </tile>
                """;
            var xml = new XmlDocument();
            xml.LoadXml(xmlContent);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(new TileNotification(xml) { Tag = d.Id });
        }
    }
}
