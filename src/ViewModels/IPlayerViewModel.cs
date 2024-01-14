// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Player;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 播放器视图模型的接口.
/// </summary>
public interface IPlayerViewModel : INotifyPropertyChanged, IDisposable
{
    /// <summary>
    /// 媒体打开时触发的事件.
    /// </summary>
    event EventHandler MediaOpened;

    /// <summary>
    /// 媒体结束时触发的事件.
    /// </summary>
    event EventHandler MediaEnded;

    /// <summary>
    /// 媒体状态改变时触发的事件.
    /// </summary>
    event EventHandler<MediaStateChangedEventArgs> StateChanged;

    /// <summary>
    /// 媒体位置改变时触发的事件.
    /// </summary>
    event EventHandler<MediaPositionChangedEventArgs> PositionChanged;

    /// <summary>
    /// WebDav 字幕列表改变时触发的事件.
    /// </summary>
    event EventHandler<WebDavSubtitleListChangedEventArgs> WebDavSubtitleListChanged;

    /// <summary>
    /// WebDav 字幕改变时触发的事件.
    /// </summary>
    event EventHandler<string> WebDavSubtitleChanged;

    /// <summary>
    /// 截屏命令.
    /// </summary>
    IAsyncRelayCommand TakeScreenshotCommand { get; }

    /// <summary>
    /// 停止录制命令.
    /// </summary>
    IRelayCommand StartRecordingCommand { get; }

    /// <summary>
    /// 停止录制命令.
    /// </summary>
    IAsyncRelayCommand StopRecordingCommand { get; }

    /// <summary>
    /// 清理资源命令.
    /// </summary>
    IRelayCommand ClearCommand { get; }

    /// <summary>
    /// 是否循环.
    /// </summary>
    bool IsLoop { get; set; }

    /// <summary>
    /// 是否正在录制.
    /// </summary>
    bool IsRecording { get; }

    /// <summary>
    /// 获取当前媒体的位置.
    /// </summary>
    TimeSpan Position { get; }

    /// <summary>
    /// 获取媒体的总时长.
    /// </summary>
    TimeSpan Duration { get; }

    /// <summary>
    /// 获取媒体的音量.
    /// </summary>
    double Volume { get; }

    /// <summary>
    /// 获取媒体的播放速率.
    /// </summary>
    double PlayRate { get; }

    /// <summary>
    /// 获取或设置媒体的状态.
    /// </summary>
    PlayerStatus Status { get; set; }

    /// <summary>
    /// 获取媒体播放器是否准备就绪.
    /// </summary>
    bool IsPlayerReady { get; }

    /// <summary>
    /// 播放器核心.
    /// </summary>
    object Player { get; }

    /// <summary>
    /// 获取最后一次错误信息.
    /// </summary>
    string LastError { get; }

    /// <summary>
    /// 是否支持录制.
    /// </summary>
    bool IsRecordingSupported { get; }

    /// <summary>
    /// 是否支持显示媒体信息.
    /// </summary>
    bool IsMediaStatsSupported { get; }

    /// <summary>
    /// 初始化.
    /// </summary>
    void Initialize();

    /// <summary>
    /// 设置视频、音频源.
    /// </summary>
    /// <param name="video">视频源.</param>
    /// <param name="audio">音频源.</param>
    /// <param name="audioOnly">是否仅音频.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task SetSourceAsync(SegmentInformation video, SegmentInformation audio, bool audioOnly);

    /// <summary>
    /// 设置直播源.
    /// </summary>
    /// <param name="url">直播地址.</param>
    void SetLiveSource(string url, bool audioOnly);

    /// <summary>
    /// 设置 WebDav 源.
    /// </summary>
    /// <param name="video">视频信息.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task SetWebDavAsync(WebDavVideoInformation video);

    /// <summary>
    /// 暂停.
    /// </summary>
    void Pause();

    /// <summary>
    /// 停止.
    /// </summary>
    void Stop();

    /// <summary>
    /// 播放.
    /// </summary>
    void Play();

    /// <summary>
    /// 跳转至.
    /// </summary>
    /// <param name="time">指定的时间.</param>
    void SeekTo(TimeSpan time);

    /// <summary>
    /// 设置播放速率.
    /// </summary>
    /// <param name="rate">播放速率.</param>
    void SetPlayRate(double rate);

    /// <summary>
    /// 设置音量.
    /// </summary>
    /// <param name="volume">音量.</param>
    void SetVolume(int volume);

    /// <summary>
    /// 获取当前正在播放的媒体信息.
    /// </summary>
    /// <returns><see cref="MediaStats"/>.</returns>
    MediaStats GetMediaInformation();

    /// <summary>
    /// 改变本地字幕.
    /// </summary>
    /// <param name="meta">字幕信息.</param>
    void ChangeLocalSubtitle(SubtitleMeta meta);
}
