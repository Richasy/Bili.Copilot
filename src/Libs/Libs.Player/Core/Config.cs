// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using Bili.Copilot.Libs.Player.Core.Configs;
using Bili.Copilot.Libs.Player.Models;
using Bili.Copilot.Libs.Player.Plugins;

namespace Bili.Copilot.Libs.Player.Core;

/// <summary>
/// 播放器配置.
/// </summary>
public sealed class Config : ObservableObject
{
    /// <summary>
    /// 初始化 <see cref="Config"/> 类的新实例.
    /// </summary>
    public Config()
    {
        // 将默认插件选项解析为 Config.Plugins（在接口中使用静态成员之前创建实例）
        foreach (var plugin in Engine.Plugins.Types.Values)
        {
            var tmpPlugin = PluginHandler.CreatePluginInstance(plugin);
            var defaultOptions = tmpPlugin.GetDefaultOptions();
            tmpPlugin.Dispose();

            if (defaultOptions == null || defaultOptions.Count == 0)
            {
                continue;
            }

            Plugins.Add(plugin.Name, new Dictionary<string, string>());
            foreach (var opt in defaultOptions)
            {
                Plugins[plugin.Name].Add(opt.Key, opt.Value);
            }
        }

        Player.SetConfig(this);
        Demuxer.SetConfig(this);
    }

    /// <summary>
    /// 播放器配置.
    /// </summary>
    public PlayerConfig Player { get; set; } = new PlayerConfig();

    /// <summary>
    /// 解复用器配置.
    /// </summary>
    public DemuxerConfig Demuxer { get; set; } = new DemuxerConfig();

    /// <summary>
    /// 解码器配置.
    /// </summary>
    public DecoderConfig Decoder { get; set; } = new DecoderConfig();

    /// <summary>
    /// 视频配置.
    /// </summary>
    public VideoConfig Video { get; set; } = new VideoConfig();

    /// <summary>
    /// 音频配置.
    /// </summary>
    public AudioConfig Audio { get; set; } = new AudioConfig();

    /// <summary>
    /// 字幕配置.
    /// </summary>
    public SubtitleConfig Subtitles { get; set; } = new SubtitleConfig();

    /// <summary>
    /// 插件配置.
    /// </summary>
    public Dictionary<string, Dictionary<string, string>> Plugins = new();

    /// <summary>
    /// 克隆配置.
    /// </summary>
    /// <returns>克隆后的配置.</returns>
    public Config Clone()
    {
        Config config = new()
        {
            Audio = Audio.Clone(),
            Video = Video.Clone(),
            Subtitles = Subtitles.Clone(),
            Demuxer = Demuxer.Clone(),
            Decoder = Decoder.Clone(),
            Player = Player.Clone(),
        };

        config.Player.SetConfig(config);
        config.Demuxer.SetConfig(config);

        return config;
    }

    internal void SetPlayer(MediaPlayer.Player player)
    {
        Player.SetPlayer(player);
        Demuxer.SetPlayer(player);
        Decoder.SetPlayer(player);
        Audio.SetPlayer(player);
        Video.SetPlayer(player);
        Subtitles.SetPlayer(player);
    }
}
