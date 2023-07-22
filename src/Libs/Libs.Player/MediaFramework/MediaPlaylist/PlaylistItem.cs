// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;

using Bili.Copilot.Libs.Player.MediaFramework.MediaDemuxer;
using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaPlaylist;

/// <summary>
/// 播放列表项.
/// </summary>
public class PlaylistItem : DemuxerInput
{
    private readonly object _lockExternalStreams = new();
    private string _title = string.Empty;
    private string _originalTitle = string.Empty;
    private bool _enabled;

    /// <summary>
    /// 索引，如果需要，在删除项时需要确保修复它.
    /// </summary>
    public int Index { get; set; } = -1;

    /// <summary>
    /// 当URL可能过期或为空时，可以使用DirectUrl作为重新打开的新输入.
    /// </summary>
    public string DirectUrl { get; set; }

    /// <summary>
    /// 相对于播放列表文件夹基础的文件夹（可以为空，但不能为null）.
    /// 使用Path.Combine(Playlist.FolderBase, Folder)获取与当前选择项相关文件（如字幕）的绝对路径.
    /// </summary>
    public string Folder { get; set; } = string.Empty;

    /// <summary>
    /// 项的文件大小.
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 项的标题（可以从爬虫更新）.
    /// </summary>
    public string Title
    {
        get => _title;
        set
        {
            if (_title == string.Empty)
            {
                OriginalTitle = value;
            }

            SetProperty(ref _title, value ?? string.Empty);
        }
    }

    /// <summary>
    /// 项的原始标题（由打开的插件设置）.
    /// </summary>
    public string OriginalTitle
    {
        get => _originalTitle;
        set => SetProperty(ref _originalTitle, value ?? string.Empty);
    }

    /// <summary>
    /// 季数.
    /// </summary>
    public int Season { get; set; }

    /// <summary>
    /// 集数.
    /// </summary>
    public int Episode { get; set; }

    /// <summary>
    /// 是否已在本地搜索.
    /// </summary>
    public bool SearchedLocal { get; set; }

    /// <summary>
    /// 是否已在线搜索.
    /// </summary>
    public bool SearchedOnline { get; set; }

    /// <summary>
    /// 标签字典.
    /// </summary>
    public Dictionary<string, object> Tag { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// 项当前是否启用.
    /// </summary>
    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (SetProperty(ref _enabled, value) && value == true)
            {
                OpenedCounter++;
            }
        }
    }

    /// <summary>
    /// 打开计数器.
    /// </summary>
    public int OpenedCounter { get; set; }

    /// <summary>
    /// 外部视频流.
    /// </summary>
    public ExternalVideoStream ExternalVideoStream { get; set; }

    /// <summary>
    /// 外部音频流.
    /// </summary>
    public ExternalAudioStream ExternalAudioStream { get; set; }

    /// <summary>
    /// 外部字幕流.
    /// </summary>
    public ExternalSubtitleStream ExternalSubtitlesStream { get; set; }

    /// <summary>
    /// 外部视频流集合.
    /// </summary>
    public ObservableCollection<ExternalVideoStream> ExternalVideoStreams { get; set; } = new ObservableCollection<ExternalVideoStream>();

    /// <summary>
    /// 外部音频流集合.
    /// </summary>
    public ObservableCollection<ExternalAudioStream> ExternalAudioStreams { get; set; } = new ObservableCollection<ExternalAudioStream>();

    /// <summary>
    /// 外部字幕流集合.
    /// </summary>
    public ObservableCollection<ExternalSubtitleStream> ExternalSubtitlesStreams { get; set; } = new ObservableCollection<ExternalSubtitleStream>();

    /// <summary>
    /// 添加外部流.
    /// </summary>
    /// <param name="extStream">外部流.</param>
    /// <param name="item">播放列表项.</param>
    /// <param name="pluginName">插件名称.</param>
    /// <param name="tag">标签.</param>
    public static void AddExternalStream(ExternalStream extStream, PlaylistItem item, string pluginName, object tag = null)
    {
        lock (item._lockExternalStreams)
        {
            extStream.PlaylistItem = item;
            extStream.PluginName = pluginName;

            if (extStream is ExternalAudioStream audioStream)
            {
                item.ExternalAudioStreams.Add(audioStream);
                extStream.Index = item.ExternalAudioStreams.Count - 1;
            }
            else if (extStream is ExternalVideoStream videoStream)
            {
                item.ExternalVideoStreams.Add(videoStream);
                extStream.Index = item.ExternalVideoStreams.Count - 1;
            }
            else if (extStream is ExternalSubtitleStream subtitleStream)
            {
                item.ExternalSubtitlesStreams.Add(subtitleStream);
                extStream.Index = item.ExternalSubtitlesStreams.Count - 1;
            }

            if (tag != null)
            {
                extStream.AddTag(tag, pluginName);
            }
        }
    }

    /// <summary>
    /// 添加标签.
    /// </summary>
    /// <param name="tag">标签.</param>
    /// <param name="pluginName">插件名称.</param>
    public void AddTag(object tag, string pluginName)
    {
        if (Tag.ContainsKey(pluginName))
        {
            Tag[pluginName] = tag;
        }
        else
        {
            Tag.Add(pluginName, tag);
        }
    }

    /// <summary>
    /// 获取标签.
    /// </summary>
    /// <param name="pluginName">插件名称.</param>
    /// <returns>标签.</returns>
    public object GetTag(string pluginName)
        => Tag.TryGetValue(pluginName, out var value) ? value : null;
}
