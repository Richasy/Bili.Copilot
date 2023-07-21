// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.Plugins;

/// <summary>
/// 插件处理器.
/// </summary>
public partial class PluginHandler
{
    /// <summary>
    /// 当初始化时调用.
    /// </summary>
    public void OnInitializing()
    {
        OpenCounter++;
        OpenItemCounter++;
        foreach (var plugin in Plugins.Values)
        {
            plugin.OnInitializing();
        }
    }

    /// <summary>
    /// 当初始化完成时调用.
    /// </summary>
    public void OnInitialized()
    {
        OpenedPlugin = null;
        OpenedSubtitlePlugin = null;

        Playlist.Reset();

        foreach (var plugin in Plugins.Values)
        {
            plugin.OnInitialized();
        }
    }

    /// <summary>
    /// 当切换初始化时调用.
    /// </summary>
    public void OnInitializingSwitch()
    {
        OpenItemCounter++;
        foreach (var plugin in Plugins.Values)
        {
            plugin.OnInitializingSwitch();
        }
    }

    /// <summary>
    /// 当切换初始化完成时调用.
    /// </summary>
    public void OnInitializedSwitch()
    {
        foreach (var plugin in Plugins.Values)
        {
            plugin.OnInitializedSwitch();
        }
    }

    /// <summary>
    /// 释放资源.
    /// </summary>
    public void Dispose()
    {
        foreach (var plugin in Plugins.Values)
        {
            plugin.Dispose();
        }
    }
}
