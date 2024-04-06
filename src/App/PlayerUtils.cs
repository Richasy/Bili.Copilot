﻿// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Pages;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Player;
using Bili.Copilot.Models.Data.Local;
using Bili.Copilot.Models.Data.Video;
using Bili.Copilot.ViewModels.Items;

namespace Bili.Copilot.App;

internal class PlayerUtils
{
    internal static void InitializePlayer(PlaySnapshot snapshot, Frame frame, Window attachedWindow)
    {
        var navArgs = new PlayerPageNavigateEventArgs
        {
            Snapshot = snapshot,
            AttachedWindow = attachedWindow,
        };
        if (IsWebPlayer())
        {
            AppToolkit.ChangeWindowTheme(attachedWindow, ElementTheme.Light);
            _ = frame.Navigate(typeof(WebPlayerPage), navArgs);
            TraceLogger.LogWebPlayerOpen();
            attachedWindow.Activate();
            return;
        }

        if (snapshot.VideoType == Models.Constants.Bili.VideoType.Video)
        {
            var pageType = IsMpvPlayer() ? typeof(VideoMpvPlayerPage) : typeof(VideoPlayerPage);
            _ = frame.Navigate(pageType, navArgs);
        }
        else if (snapshot.VideoType == Models.Constants.Bili.VideoType.Live)
        {
            var pageType = IsMpvPlayer() ? typeof(LiveMpvPlayerPage) : typeof(LivePlayerPage);
            _ = frame.Navigate(pageType, navArgs);
        }
        else if (snapshot.VideoType == Models.Constants.Bili.VideoType.Pgc)
        {
            var pageType = IsMpvPlayer() ? typeof(PgcMpvPlayerPage) : typeof(PgcPlayerPage);
            _ = frame.Navigate(pageType, navArgs);
        }

        TraceLogger.LogPlayerOpen(
            snapshot.VideoType.ToString(),
            SettingsToolkit.ReadLocalSetting(SettingNames.PreferCodec, PreferCodec.H264).ToString(),
            SettingsToolkit.ReadLocalSetting(SettingNames.PreferQuality, PreferQuality.HDFirst).ToString(),
            SettingsToolkit.ReadLocalSetting(SettingNames.DecodeType, DecodeType.HardwareDecode).ToString(),
            IsMpvPlayer() ? "mpv" : SettingsToolkit.ReadLocalSetting(SettingNames.PlayerType, PlayerType.Native).ToString());
        attachedWindow.Activate();
    }

    internal static void InitializePlayer(List<VideoInformation> snapshots, Frame frame, Window attachedWindow)
    {
        var navArgs = new PlayerPageNavigateEventArgs
        {
            Playlist = snapshots,
            AttachedWindow = attachedWindow,
        };

        var pageType = IsMpvPlayer() ? typeof(VideoMpvPlayerPage) : typeof(VideoPlayerPage);
        _ = frame.Navigate(pageType, navArgs);
        attachedWindow.Activate();
    }

    internal static void InitializePlayer(List<WebDavStorageItemViewModel> items, Frame frame, Window attachedWindow)
    {
        frame.Tag = attachedWindow;
        _ = frame.Navigate(typeof(WebDavPlayerPage), items);
        attachedWindow.Activate();
    }

    private static bool IsWebPlayer()
        => SettingsToolkit.ReadLocalSetting(SettingNames.UseWebPlayer, false);

    private static bool IsMpvPlayer()
        => SettingsToolkit.ReadLocalSetting(SettingNames.UseMpvPlayer, false);
}
