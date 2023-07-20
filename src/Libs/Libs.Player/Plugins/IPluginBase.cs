// Copyright (c) Bili Copilot. All rights reserved.

using System;

namespace Bili.Copilot.Libs.Player.Plugins;

/// <summary>
/// 插件基类接口.
/// </summary>
public interface IPluginBase
{
    /// <summary>
    /// 插件名称.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 插件版本.
    /// </summary>
    Version Version { get; }

    /// <summary>
    /// 插件处理程序.
    /// </summary>
    PluginHandler Handler { get; }

    /// <summary>
    /// 插件优先级.
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// 当插件加载时调用.
    /// </summary>
    void OnLoaded();

    /// <summary>
    /// 当插件正在初始化时调用.
    /// </summary>
    void OnInitializing();

    /// <summary>
    /// 当插件初始化完成时调用.
    /// </summary>
    void OnInitialized();

    /// <summary>
    /// 当插件正在初始化切换时调用.
    /// </summary>
    void OnInitializingSwitch();

    /// <summary>
    /// 当插件初始化切换完成时调用.
    /// </summary>
    void OnInitializedSwitch();

    /// <summary>
    /// 当缓冲时调用.
    /// </summary>
    void OnBuffering();

    /// <summary>
    /// 当缓冲完成时调用.
    /// </summary>
    void OnBufferingCompleted();

    /// <summary>
    /// 当打开外部音频时调用.
    /// </summary>
    void OnOpenExternalAudio();

    /// <summary>
    /// 当打开外部视频时调用.
    /// </summary>
    void OnOpenExternalVideo();

    /// <summary>
    /// 当打开外部字幕时调用.
    /// </summary>
    void OnOpenExternalSubtitles();
}
