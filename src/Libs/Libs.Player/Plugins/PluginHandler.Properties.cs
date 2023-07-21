// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using Bili.Copilot.Libs.Player.Engine;

namespace Bili.Copilot.Libs.Player.Plugins;

/// <summary>
/// 插件处理器.
/// </summary>
public partial class PluginHandler
{
    /// <summary>
    /// 插件处理器的配置.
    /// </summary>
    public Config Config { get; private set; }

    /// <summary>
    /// 是否中断.
    /// </summary>
    public bool Interrupt { get; set; }

    /// <summary>
    /// 当前打开的插件.
    /// </summary>
    public IOpenPlugin OpenedPlugin { get; private set; }

    /// <summary>
    /// 当前打开的字幕插件.
    /// </summary>
    public IOpenSubtitles OpenedSubtitlesPlugin { get; private set; }

    /// <summary>
    /// 打开计数器.
    /// </summary>
    public long OpenCounter { get; internal set; }

    /// <summary>
    /// 打开项计数器.
    /// </summary>
    public long OpenItemCounter { get; internal set; }

    /// <summary>
    /// 播放列表.
    /// </summary>
    public Playlist Playlist { get; set; }

    /// <summary>
    /// 唯一标识符.
    /// </summary>
    public int UniqueId { get; set; }

    /// <summary>
    /// 插件集合.
    /// </summary>
    public Dictionary<string, PluginBase> Plugins { get; private set; }

    /// <summary>
    /// 打开插件集合.
    /// </summary>
    public Dictionary<string, IOpen> PluginsOpen { get; private set; }

    /// <summary>
    /// 打开字幕插件集合.
    /// </summary>
    public Dictionary<string, IOpenSubtitles> PluginsOpenSubtitles { get; private set; }

    /// <summary>
    /// 爬取项插件集合.
    /// </summary>
    public Dictionary<string, IScrapeItem> PluginsScrapeItem { get; private set; }

    /// <summary>
    /// 建议播放项插件集合.
    /// </summary>
    public Dictionary<string, ISuggestPlaylistItem> PluginsSuggestItem { get; private set; }

    /// <summary>
    /// 建议音频流插件集合.
    /// </summary>
    public Dictionary<string, ISuggestAudioStream> PluginsSuggestAudioStream { get; private set; }

    /// <summary>
    /// 建议视频流插件集合.
    /// </summary>
    public Dictionary<string, ISuggestVideoStream> PluginsSuggestVideoStream { get; private set; }

    /// <summary>
    /// 建议外部音频插件集合.
    /// </summary>
    public Dictionary<string, ISuggestExternalAudio> PluginsSuggestExternalAudio { get; private set; }

    /// <summary>
    /// 建议外部视频插件集合.
    /// </summary>
    public Dictionary<string, ISuggestExternalVideo> PluginsSuggestExternalVideo { get; private set; }

    /// <summary>
    /// 建议字幕流插件集合.
    /// </summary>
    public Dictionary<string, ISuggestSubtitlesStream> PluginsSuggestSubtitlesStream { get; private set; }

    /// <summary>
    /// 建议字幕插件集合.
    /// </summary>
    public Dictionary<string, ISuggestSubtitles> PluginsSuggestSubtitles { get; private set; }

    /// <summary>
    /// 建议最佳外部字幕插件集合.
    /// </summary>
    public Dictionary<string, ISuggestBestExternalSubtitles> PluginsSuggestBestExternalSubtitles { get; private set; }

    /// <summary>
    /// 下载字幕插件集合.
    /// </summary>
    public Dictionary<string, IDownloadSubtitles> PluginsDownloadSubtitles { get; private set; }

    /// <summary>
    /// 搜索本地字幕插件集合.
    /// </summary>
    public Dictionary<string, ISearchLocalSubtitles> PluginsSearchLocalSubtitles { get; private set; }

    /// <summary>
    /// 搜索在线字幕插件集合.
    /// </summary>
    public Dictionary<string, ISearchOnlineSubtitles> PluginsSearchOnlineSubtitles { get; private set; }
}
