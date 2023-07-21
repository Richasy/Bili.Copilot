// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.Linq;
using Bili.Copilot.Libs.Player.MediaFramework.MediaPlaylist;
using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;

namespace Bili.Copilot.Libs.Player.Plugins;

/// <summary>
/// 插件处理器.
/// </summary>
public partial class PluginHandler
{
    /// <summary>
    /// 打开插件.
    /// </summary>
    /// <returns>打开结果.</returns>
    public OpenResult Open()
    {
        var sessionId = OpenCounter;
        var plugins = PluginsOpen.Values.OrderBy(x => x.Priority);
        foreach (var plugin in plugins)
        {
            if (Interrupt || sessionId != OpenCounter)
            {
                return new OpenResult("Cancelled");
            }

            if (!plugin.CanOpen())
            {
                continue;
            }

            var res = plugin.Open();
            if (res == null)
            {
                continue;
            }

            // 当前不允许出现错误时进行回退
            if (res.Error != null)
            {
                return res;
            }

            OpenedPlugin = plugin;
            Log.Info($"[{plugin.Name}] Open Success");

            return res;
        }

        return new OpenResult("No plugin found for the provided input");
    }

    /// <summary>
    /// 打开项目.
    /// </summary>
    /// <returns>打开结果.</returns>
    public OpenResult OpenItem()
    {
        var sessionId = OpenItemCounter;
        var res = OpenedPlugin.OpenItem();

        res ??= new OpenResult("Cancelled");

        if (sessionId != OpenItemCounter)
        {
            res.Error = "Cancelled";
        }

        if (res.Error == null)
        {
            Log.Info($"[{OpenedPlugin.Name}] Open Item ({Playlist.Selected.Index}) Success");
        }

        return res;
    }

    /// <summary>
    /// 仅允许从已打开的插件中调用.
    /// </summary>
    public void OnPlaylistCompleted()
    {
        Playlist.Completed = true;
        if (Playlist.ExpectingItems == 0)
        {
            Playlist.ExpectingItems = Playlist.Items.Count;
        }

        if (Playlist.Items.Count > 1)
        {
            Log.Debug("Playlist Completed");
        }
    }

    /// <summary>
    /// 抓取播放项目.
    /// </summary>
    /// <param name="item">项目.</param>
    public void ScrapeItem(PlaylistItem item)
    {
        var plugins = PluginsScrapeItem.Values.OrderBy(x => x.Priority);
        foreach (var plugin in plugins)
        {
            if (Interrupt)
            {
                return;
            }

            plugin.ScrapeItem(item);
        }
    }

    /// <summary>
    /// 建议播放条目.
    /// </summary>
    /// <returns>建议的条目.</returns>
    public PlaylistItem SuggestItem()
    {
        var plugins = PluginsSuggestItem.Values.OrderBy(x => x.Priority);
        foreach (var plugin in plugins)
        {
            if (Interrupt)
            {
                return null;
            }

            var item = plugin.SuggestItem();
            if (item != null)
            {
                Log.Info($"SuggestItem #{item.Index} - {item.Title}");
                return item;
            }
        }

        return null;
    }

    /// <summary>
    /// 建议外部视频流.
    /// </summary>
    /// <returns>建议的外部视频流.</returns>
    public ExternalVideoStream SuggestExternalVideo()
    {
        var plugins = PluginsSuggestExternalVideo.Values.OrderBy(x => x.Priority);
        foreach (var plugin in plugins)
        {
            if (Interrupt)
            {
                return null;
            }

            var extStream = plugin.SuggestExternalVideo();
            if (extStream != null)
            {
                Log.Info($"SuggestVideo (External) {extStream.Width} x {extStream.Height} @ {extStream.FPS}");
                return extStream;
            }
        }

        return null;
    }

    /// <summary>
    /// 建议外部音频流.
    /// </summary>
    /// <returns>建议的外部音频流.</returns>
    public ExternalAudioStream SuggestExternalAudio()
    {
        var plugins = PluginsSuggestExternalAudio.Values.OrderBy(x => x.Priority);
        foreach (var plugin in plugins)
        {
            if (Interrupt)
            {
                return null;
            }

            var extStream = plugin.SuggestExternalAudio();
            if (extStream != null)
            {
                Log.Info($"SuggestAudio (External) {extStream.SampleRate} Hz, {extStream.Codec}");
                return extStream;
            }
        }

        return null;
    }

    /// <summary>
    /// 建议视频流.
    /// </summary>
    /// <param name="streams">视频流集合.</param>
    /// <returns>建议的视频流.</returns>
    public VideoStream SuggestVideo(ObservableCollection<VideoStream> streams)
    {
        if (streams == null || streams.Count == 0)
        {
            return null;
        }

        var plugins = PluginsSuggestVideoStream.Values.OrderBy(x => x.Priority);
        foreach (var plugin in plugins)
        {
            if (Interrupt)
            {
                return null;
            }

            var stream = plugin.SuggestVideo(streams);
            if (stream != null)
            {
                return stream;
            }
        }

        return null;
    }

    /// <summary>
    /// 建议视频流和外部视频流.
    /// </summary>
    /// <param name="stream">视频流.</param>
    /// <param name="extStream">外部视频流.</param>
    /// <param name="streams">视频流集合.</param>
    public void SuggestVideo(out VideoStream stream, out ExternalVideoStream extStream, ObservableCollection<VideoStream> streams)
    {
        stream = null;
        extStream = null;

        if (Interrupt)
        {
            return;
        }

        stream = SuggestVideo(streams);
        if (stream != null)
        {
            return;
        }

        if (Interrupt)
        {
            return;
        }

        extStream = SuggestExternalVideo();
    }

    /// <summary>
    /// 建议音频流.
    /// </summary>
    /// <param name="streams">音频流集合.</param>
    /// <returns>建议的音频流.</returns>
    public AudioStream SuggestAudio(ObservableCollection<AudioStream> streams)
    {
        if (streams == null || streams.Count == 0)
        {
            return null;
        }

        var plugins = PluginsSuggestAudioStream.Values.OrderBy(x => x.Priority);
        foreach (var plugin in plugins)
        {
            if (Interrupt)
            {
                return null;
            }

            var stream = plugin.SuggestAudio(streams);
            if (stream != null)
            {
                return stream;
            }
        }

        return null;
    }

    /// <summary>
    /// 建议音频流和外部音频流.
    /// </summary>
    /// <param name="stream">音频流.</param>
    /// <param name="extStream">外部音频流.</param>
    /// <param name="streams">音频流集合.</param>
    public void SuggestAudio(out AudioStream stream, out ExternalAudioStream extStream, ObservableCollection<AudioStream> streams)
    {
        stream = null;
        extStream = null;

        if (Interrupt)
        {
            return;
        }

        stream = SuggestAudio(streams);
        if (stream != null)
        {
            return;
        }

        if (Interrupt)
        {
            return;
        }

        extStream = SuggestExternalAudio();
    }
}
