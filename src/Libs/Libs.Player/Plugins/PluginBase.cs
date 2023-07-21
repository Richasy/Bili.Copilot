// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using Bili.Copilot.Libs.Player.Core;
using Bili.Copilot.Libs.Player.MediaFramework.MediaContext;
using Bili.Copilot.Libs.Player.MediaFramework.MediaPlaylist;
using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;
using Bili.Copilot.Libs.Player.Misc;

namespace Bili.Copilot.Libs.Player.Plugins;

/// <summary>
/// 插件基类，继承自 PluginType，实现了 IPluginBase 接口.
/// </summary>
public abstract class PluginBase : PluginType, IPluginBase
{
    /// <summary>
    /// 获取或设置插件的配置选项.
    /// </summary>
    public Dictionary<string, string> Options => Config?.Plugins[Name];

    /// <summary>
    /// 获取插件的配置信息.
    /// </summary>
    public Config Config => Handler.Config;

    /// <summary>
    /// 获取插件的播放列表.
    /// </summary>
    public Playlist Playlist => Handler.Playlist;

    /// <summary>
    /// 获取当前选中的播放项.
    /// </summary>
    public PlaylistItem Selected => Handler.Playlist.Selected;

    /// <summary>
    /// 获取插件的解码器上下文.
    /// </summary>
    public DecoderContext Decoder => (DecoderContext)Handler;

    /// <summary>
    /// 获取或设置插件的处理器.
    /// </summary>
    public PluginHandler Handler { get; internal set; }

    /// <summary>
    /// 获取或设置插件的日志处理器.
    /// </summary>
    public LogHandler Log { get; internal set; }

    /// <summary>
    /// 获取或设置插件是否已释放.
    /// </summary>
    public bool Disposed { get; protected set; }

    /// <summary>
    /// 获取或设置插件的优先级，默认为 1000.
    /// </summary>
    public int Priority { get; set; } = 1000;

    /// <inheritdoc/>
    public virtual void OnLoaded()
    {
    }

    /// <inheritdoc/>
    public virtual void OnInitializing()
    {
    }

    /// <inheritdoc/>
    public virtual void OnInitialized()
    {
    }

    /// <inheritdoc/>
    public virtual void OnInitializingSwitch()
    {
    }

    /// <inheritdoc/>
    public virtual void OnInitializedSwitch()
    {
    }

    /// <inheritdoc/>
    public virtual void OnBuffering()
    {
    }

    /// <inheritdoc/>
    public virtual void OnBufferingCompleted()
    {
    }

    /// <summary>
    /// 当打开播放项时调用.
    /// </summary>
    public virtual void OnOpen()
    {
    }

    /// <inheritdoc/>
    public virtual void OnOpenExternalAudio()
    {
    }

    /// <inheritdoc/>
    public virtual void OnOpenExternalVideo()
    {
    }

    /// <inheritdoc/>
    public virtual void OnOpenExternalSubtitles()
    {
    }

    /// <summary>
    /// 释放插件资源.
    /// </summary>
    public virtual void Dispose()
    {
    }

    /// <summary>
    /// 添加外部流到播放项.
    /// </summary>
    /// <param name="extStream">外部流对象.</param>
    /// <param name="tag">附加的标签对象.</param>
    /// <param name="item">要添加到的播放项，默认为当前选中的播放项.</param>
    public void AddExternalStream(ExternalStream extStream, object tag = null, PlaylistItem item = null)
    {
        item ??= Playlist.Selected;
        item?.AddExternalStream(extStream, item, Name, tag);
    }

    /// <summary>
    /// 添加播放项到播放列表.
    /// </summary>
    /// <param name="item">要添加的播放项.</param>
    /// <param name="tag">附加的标签对象.</param>
    public void AddPlaylistItem(PlaylistItem item, object tag = null)
        => Playlist.AddItem(item, Name, tag);

    /// <summary>
    /// 添加标签到播放项.
    /// </summary>
    /// <param name="tag">要添加的标签对象.</param>
    /// <param name="item">要添加到的播放项，默认为当前选中的播放项.</param>
    public void AddTag(object tag, PlaylistItem item = null)
    {
        item ??= Playlist.Selected;
        item?.AddTag(tag, Name);
    }

    /// <summary>
    /// 获取外部流的标签对象.
    /// </summary>
    /// <param name="extStream">外部流对象.</param>
    /// <returns>标签对象.</returns>
    public object GetTag(ExternalStream extStream)
        => extStream?.GetTag(Name);

    /// <summary>
    /// 获取播放项的标签对象.
    /// </summary>
    /// <param name="item">播放项对象.</param>
    /// <returns>标签对象.</returns>
    public object GetTag(PlaylistItem item)
        => item?.GetTag(Name);

    /// <summary>
    /// 获取默认的配置选项.
    /// </summary>
    /// <returns>默认的配置选项.</returns>
    public virtual Dictionary<string, string> GetDefaultOptions()
        => new Dictionary<string, string>();
}
