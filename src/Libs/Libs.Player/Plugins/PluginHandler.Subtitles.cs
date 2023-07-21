// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Bili.Copilot.Libs.Player.Core;
using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;

namespace Bili.Copilot.Libs.Player.Plugins;

/// <summary>
/// 插件处理器.
/// </summary>
public partial class PluginHandler
{
    /// <summary>
    /// 打开字幕.
    /// </summary>
    /// <param name="url">字幕地址.</param>
    /// <returns>打开的字幕结果.</returns>
    public OpenSubtitleResult OpenSubtitle(string url)
    {
        var plugins = PluginsOpenSubtitles.Values.OrderBy(x => x.Priority);
        foreach (var plugin in plugins)
        {
            var res = plugin.Open(url);
            if (res == null)
            {
                continue;
            }

            if (res.Error != null)
            {
                return res;
            }

            OpenedSubtitlePlugin = plugin;
            Log.Info($"[{plugin.Name}] Open Subtitles Success");

            return res;
        }

        return null;
    }

    /// <summary>
    /// 搜索本地字幕.
    /// </summary>
    public void SearchLocalSubtitle()
    {
        var plugins = PluginsSearchLocalSubtitles.Values.OrderBy(x => x.Priority);
        foreach (var plugin in plugins)
        {
            if (Interrupt)
            {
                return;
            }

            plugin.SearchLocalSubtitle();
        }

        Playlist.Selected.SearchedLocal = true;
    }

    /// <summary>
    /// 搜索在线字幕.
    /// </summary>
    public void SearchOnlineSubtitle()
    {
        var plugins = PluginsSearchOnlineSubtitles.Values.OrderBy(x => x.Priority);
        foreach (var plugin in plugins)
        {
            if (Interrupt)
            {
                return;
            }

            plugin.SearchOnlineSubtitle();
        }

        Playlist.Selected.SearchedOnline = true;
    }

    /// <summary>
    /// 下载字幕.
    /// </summary>
    /// <param name="extStream">外部字幕流.</param>
    /// <returns>是否下载成功.</returns>
    public bool DownloadSubtitle(ExternalSubtitleStream extStream)
    {
        var res = false;

        var plugins = PluginsDownloadSubtitles.Values.OrderBy(x => x.Priority);
        foreach (var plugin in plugins)
        {
            if (res = plugin.DownloadSubtitle(extStream))
            {
                extStream.Downloaded = true;
                return res;
            }
        }

        return res;
    }

    /// <summary>
    /// 建议最佳外部字幕.
    /// </summary>
    /// <returns>最佳外部字幕流.</returns>
    public ExternalSubtitleStream SuggestBestExternalSubtitle()
    {
        var plugins = PluginsSuggestBestExternalSubtitles.Values.OrderBy(x => x.Priority);
        foreach (var plugin in plugins)
        {
            if (Interrupt)
            {
                return null;
            }

            var extStream = plugin.SuggestBestExternalSubtitle();
            if (extStream != null)
            {
                return extStream;
            }
        }

        return null;
    }

    /// <summary>
    /// 建议字幕.
    /// </summary>
    /// <param name="stream">字幕流.</param>
    /// <param name="extStream">外部字幕流.</param>
    public void SuggestSubtitle(out SubtitlesStream stream, out ExternalSubtitleStream extStream)
    {
        stream = null;
        extStream = null;

        var plugins = PluginsSuggestSubtitles.Values.OrderBy(x => x.Priority);
        foreach (var plugin in plugins)
        {
            if (Interrupt)
            {
                return;
            }

            plugin.SuggestSubtitle(out stream, out extStream);
            if (stream != null || extStream != null)
            {
                return;
            }
        }
    }

    /// <summary>
    /// 建议字幕.
    /// </summary>
    /// <param name="streams">字幕流集合.</param>
    /// <param name="langs">语言列表.</param>
    /// <returns>建议的字幕流.</returns>
    public SubtitlesStream SuggestSubtitles(ObservableCollection<SubtitlesStream> streams, List<Language> langs)
    {
        if (streams == null || streams.Count == 0)
        {
            return null;
        }

        var plugins = PluginsSuggestSubtitlesStream.Values.OrderBy(x => x.Priority);
        foreach (var plugin in plugins)
        {
            if (Interrupt)
            {
                return null;
            }

            var stream = plugin.SuggestSubtitle(streams, langs);
            if (stream != null)
            {
                return stream;
            }
        }

        return null;
    }
}
