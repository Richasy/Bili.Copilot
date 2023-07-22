// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Bili.Copilot.Libs.Player.Core;
using Bili.Copilot.Libs.Player.MediaFramework.MediaPlaylist;
using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;

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

/// <summary>
/// 打开插件接口.
/// </summary>
public interface IOpenPlugin : IPluginBase
{
    /// <summary>
    /// 是否可以打开.
    /// </summary>
    /// <returns>结果.</returns>
    bool CanOpen();

    /// <summary>
    /// 打开.
    /// </summary>
    /// <returns>打开的结果.</returns>
    OpenResult Open();

    /// <summary>
    /// 打开单项.
    /// </summary>
    /// <returns>打开结果.</returns>
    OpenResult OpenItem();
}

/// <summary>
/// 打开字幕插件的接口.
/// </summary>
public interface IOpenSubtitlePlugin : IPluginBase
{
    /// <summary>
    /// 打开字幕.
    /// </summary>
    /// <param name="url">字幕的URL.</param>
    /// <returns>打开字幕的结果.</returns>
    OpenSubtitleResult Open(string url);

    /// <summary>
    /// 打开字幕.
    /// </summary>
    /// <param name="ioStream">字幕的输入流.</param>
    /// <returns>打开字幕的结果.</returns>
    OpenSubtitleResult Open(Stream ioStream);
}

/// <summary>
/// 描述一个用于抓取项目的插件接口.
/// </summary>
public interface IScrapeItemPlugin : IPluginBase
{
    /// <summary>
    /// 抓取项目.
    /// </summary>
    /// <param name="item">要抓取的项目.</param>
    void ScrapeItem(PlaylistItem item);
}

/// <summary>
/// 描述一个用于建议播放列表项目的插件接口.
/// </summary>
public interface ISuggestPlaylistItemPlugin : IPluginBase
{
    /// <summary>
    /// 建议一个项目.
    /// </summary>
    /// <returns>建议的项目.</returns>
    PlaylistItem SuggestItem();
}

/// <summary>
/// 描述一个用于建议外部音频流的插件接口.
/// </summary>
public interface ISuggestExternalAudioPlugin : IPluginBase
{
    /// <summary>
    /// 建议一个外部音频流.
    /// </summary>
    /// <returns>建议的外部音频流.</returns>
    ExternalAudioStream SuggestExternalAudio();
}

/// <summary>
/// 描述一个用于建议外部视频流的插件接口.
/// </summary>
public interface ISuggestExternalVideoPlugin : IPluginBase
{
    /// <summary>
    /// 建议一个外部视频流.
    /// </summary>
    /// <returns>建议的外部视频流.</returns>
    ExternalVideoStream SuggestExternalVideo();
}

/// <summary>
/// 描述一个用于建议音频流的插件接口.
/// </summary>
public interface ISuggestAudioStreamPlugin : IPluginBase
{
    /// <summary>
    /// 建议一个音频流.
    /// </summary>
    /// <param name="streams">可用的音频流集合.</param>
    /// <returns>建议的音频流.</returns>
    AudioStream SuggestAudio(ObservableCollection<AudioStream> streams);
}

/// <summary>
/// 描述一个用于建议视频流的插件接口.
/// </summary>
public interface ISuggestVideoStreamPlugin : IPluginBase
{
    /// <summary>
    /// 建议一个视频流.
    /// </summary>
    /// <param name="streams">可用的视频流集合.</param>
    /// <returns>建议的视频流.</returns>
    VideoStream SuggestVideo(ObservableCollection<VideoStream> streams);
}

/// <summary>
/// 描述一个用于建议字幕流的插件接口.
/// </summary>
public interface ISuggestSubtitleStreamPlugin : IPluginBase
{
    /// <summary>
    /// 建议一个字幕流.
    /// </summary>
    /// <param name="streams">可用的字幕流集合.</param>
    /// <param name="langs">语言列表.</param>
    /// <returns>建议的字幕流.</returns>
    SubtitlesStream SuggestSubtitle(ObservableCollection<SubtitlesStream> streams, List<Language> langs);
}

/// <summary>
/// 描述一个用于建议字幕的插件接口.
/// </summary>
public interface ISuggestSubtitlePlugin : IPluginBase
{
    /// <summary>
    /// 从所有可用的字幕中进行建议.
    /// </summary>
    /// <param name="stream">内嵌字幕流.</param>
    /// <param name="extStream">外部字幕流.</param>
    void SuggestSubtitle(out SubtitlesStream stream, out ExternalSubtitleStream extStream);
}

/// <summary>
/// 描述一个用于建议最佳外部字幕的插件接口.
/// </summary>
public interface ISuggestBestExternalSubtitlePlugin : IPluginBase
{
    /// <summary>
    /// 只有当最佳匹配存在时才进行建议（以避免本地/在线搜索）.
    /// </summary>
    /// <returns>最佳外部字幕流.</returns>
    ExternalSubtitleStream SuggestBestExternalSubtitle();
}

/// <summary>
/// 描述一个用于搜索本地字幕的插件接口.
/// </summary>
public interface ISearchLocalSubtitlePlugin : IPluginBase
{
    /// <summary>
    /// 搜索本地字幕.
    /// </summary>
    void SearchLocalSubtitle();
}

/// <summary>
/// 描述一个用于搜索在线字幕的插件接口.
/// </summary>
public interface ISearchOnlineSubtitlePlugin : IPluginBase
{
    /// <summary>
    /// 搜索在线字幕.
    /// </summary>
    void SearchOnlineSubtitle();
}

/// <summary>
/// 描述一个用于下载字幕的插件接口.
/// </summary>
public interface IDownloadSubtitlePlugin : IPluginBase
{
    /// <summary>
    /// 下载字幕.
    /// </summary>
    /// <param name="extStream">外部字幕流.</param>
    /// <returns>是否下载成功.</returns>
    bool DownloadSubtitle(ExternalSubtitleStream extStream);
}
