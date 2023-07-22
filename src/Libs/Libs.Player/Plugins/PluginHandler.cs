// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using Bili.Copilot.Libs.Player.Core;
using Bili.Copilot.Libs.Player.MediaFramework.MediaPlaylist;
using Bili.Copilot.Libs.Player.Misc;

namespace Bili.Copilot.Libs.Player.Plugins;

/// <summary>
/// 插件处理器.
/// </summary>
public partial class PluginHandler
{
    /// <summary>
    /// 构造函数.
    /// </summary>
    /// <param name="config">配置.</param>
    /// <param name="uniqueId">唯一标识.</param>
    public PluginHandler(Config config, int uniqueId = -1)
    {
        Config = config;
        UniqueId = uniqueId == -1 ? Utils.GetUniqueId() : uniqueId;
        Playlist = new Playlist(UniqueId);
        Log = new LogHandler(("[#" + UniqueId + "]").PadRight(8, ' ') + " [PluginHandler ] ");
        LoadPlugins();
    }

    /// <summary>
    /// 创建插件实例.
    /// </summary>
    /// <param name="type">插件类型.</param>
    /// <param name="handler">插件处理器.</param>
    /// <returns>插件实例.</returns>
    public static PluginBase CreatePluginInstance(PluginType type, PluginHandler handler = null)
    {
        var plugin = (PluginBase)Activator.CreateInstance(type.Type, true);
        plugin.Handler = handler;
        plugin.Name = type.Name;
        plugin.Type = type.Type;
        plugin.Version = type.Version;

        if (handler != null)
        {
            plugin.OnLoaded();
        }

        return plugin;
    }

    /// <summary>
    /// 加载插件.
    /// </summary>
    private void LoadPlugins()
    {
        Plugins = new Dictionary<string, PluginBase>();

        foreach (var type in Engine.Plugins.Types.Values)
        {
            try
            {
                var plugin = CreatePluginInstance(type, this);
                plugin.Log = new LogHandler(("[#" + UniqueId + "]").PadRight(8, ' ') + $" [{plugin.Name,-14}] ");
                Plugins.Add(plugin.Name, plugin);
            }
            catch (Exception e)
            {
                Log.Error($"[Plugins] [Error] Failed to load plugin ... ({e.Message} {Utils.GetRecInnerException(e)}");
            }
        }

        PluginsOpen = new Dictionary<string, IOpenPlugin>();
        PluginsOpenSubtitles = new Dictionary<string, IOpenSubtitlePlugin>();
        PluginsScrapeItem = new Dictionary<string, IScrapeItemPlugin>();

        PluginsSuggestItem = new Dictionary<string, ISuggestPlaylistItemPlugin>();

        PluginsSuggestAudioStream = new Dictionary<string, ISuggestAudioStreamPlugin>();
        PluginsSuggestVideoStream = new Dictionary<string, ISuggestVideoStreamPlugin>();
        PluginsSuggestSubtitlesStream = new Dictionary<string, ISuggestSubtitleStreamPlugin>();
        PluginsSuggestSubtitles = new Dictionary<string, ISuggestSubtitlePlugin>();

        PluginsSuggestExternalAudio = new Dictionary<string, ISuggestExternalAudioPlugin>();
        PluginsSuggestExternalVideo = new Dictionary<string, ISuggestExternalVideoPlugin>();
        PluginsSuggestBestExternalSubtitles
                                        = new Dictionary<string, ISuggestBestExternalSubtitlePlugin>();

        PluginsSearchLocalSubtitles = new Dictionary<string, ISearchLocalSubtitlePlugin>();
        PluginsSearchOnlineSubtitles = new Dictionary<string, ISearchOnlineSubtitlePlugin>();
        PluginsDownloadSubtitles = new Dictionary<string, IDownloadSubtitlePlugin>();

        foreach (var plugin in Plugins.Values)
        {
            LoadPluginInterfaces(plugin);
        }
    }

    /// <summary>
    /// 加载插件接口.
    /// </summary>
    /// <param name="plugin">插件实例.</param>
    private void LoadPluginInterfaces(PluginBase plugin)
    {
        if (plugin is IOpenPlugin open)
        {
            PluginsOpen.Add(plugin.Name, open);
        }
        else if (plugin is IOpenSubtitlePlugin subtitles)
        {
            PluginsOpenSubtitles.Add(plugin.Name, subtitles);
        }

        if (plugin is IScrapeItemPlugin scrapeItem)
        {
            PluginsScrapeItem.Add(plugin.Name, scrapeItem);
        }

        if (plugin is ISuggestPlaylistItemPlugin suggestPlaylistItem)
        {
            PluginsSuggestItem.Add(plugin.Name, suggestPlaylistItem);
        }

        if (plugin is ISuggestAudioStreamPlugin suggestAudioStream)
        {
            PluginsSuggestAudioStream.Add(plugin.Name, suggestAudioStream);
        }

        if (plugin is ISuggestVideoStreamPlugin suggestVideoStream)
        {
            PluginsSuggestVideoStream.Add(plugin.Name, suggestVideoStream);
        }

        if (plugin is ISuggestSubtitleStreamPlugin suggestSubtitleStream)
        {
            PluginsSuggestSubtitlesStream.Add(plugin.Name, suggestSubtitleStream);
        }

        if (plugin is ISuggestSubtitlePlugin suggestSubtitle)
        {
            PluginsSuggestSubtitles.Add(plugin.Name, suggestSubtitle);
        }

        if (plugin is ISuggestExternalAudioPlugin extAudio)
        {
            PluginsSuggestExternalAudio.Add(plugin.Name, extAudio);
        }

        if (plugin is ISuggestExternalVideoPlugin extVideo)
        {
            PluginsSuggestExternalVideo.Add(plugin.Name, extVideo);
        }

        if (plugin is ISuggestBestExternalSubtitlePlugin extSubtitle)
        {
            PluginsSuggestBestExternalSubtitles.Add(plugin.Name, extSubtitle);
        }

        if (plugin is ISearchLocalSubtitlePlugin localSubtitle)
        {
            PluginsSearchLocalSubtitles.Add(plugin.Name, localSubtitle);
        }

        if (plugin is ISearchOnlineSubtitlePlugin onlineSubtitle)
        {
            PluginsSearchOnlineSubtitles.Add(plugin.Name, onlineSubtitle);
        }

        if (plugin is IDownloadSubtitlePlugin downloadSubtitle)
        {
            PluginsDownloadSubtitles.Add(plugin.Name, downloadSubtitle);
        }
    }
}
