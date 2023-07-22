// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.MediaFramework.MediaDemuxer;
using Bili.Copilot.Libs.Player.MediaFramework.MediaPlaylist;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaStream;

/// <summary>
/// 外部流类，继承自DemuxerInput类.
/// </summary>
public class ExternalStream : DemuxerInput
{
    private bool _enabled;

    /// <summary>
    /// 插件名称.
    /// </summary>
    public string PluginName { get; set; }

    /// <summary>
    /// 播放列表项.
    /// </summary>
    public PlaylistItem PlaylistItem { get; set; }

    /// <summary>
    /// 索引.
    /// </summary>
    public int Index { get; set; } = -1; // 如果需要的话（已经用于比较相同类型的流），我们需要确保在删除项目时修复它.

    /// <summary>
    /// 协议.
    /// </summary>
    public string Protocol { get; set; }

    /// <summary>
    /// 编解码器.
    /// </summary>
    public string Codec { get; set; }

    /// <summary>
    /// 比特率.
    /// </summary>
    public long BitRate { get; set; }

    /// <summary>
    /// 标签字典.
    /// </summary>
    public Dictionary<string, object> Tag { get; set; } = new();

    /// <summary>
    /// 是否当前启用该项.
    /// </summary>
    public bool Enabled
    {
        get => _enabled; set
        {
            if (SetProperty(ref _enabled, value) && value == true)
            {
                OpenedCounter++;
            }
        }
    }

    /// <summary>
    /// 该项被使用/打开的次数.
    /// </summary>
    public int OpenedCounter { get; set; }

    /// <summary>
    /// 媒体类型.
    /// </summary>
    public MediaType Type => this is ExternalAudioStream ? MediaType.Audio : this is ExternalVideoStream ? MediaType.Video : MediaType.Subtitle;

    /// <summary>
    /// 添加标签.
    /// </summary>
    /// <param name="tag">标签对象.</param>
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
    /// <returns>标签对象.</returns>
    public object GetTag(string pluginName)
        => Tag.TryGetValue(pluginName, out var value) ? value : null;
}
