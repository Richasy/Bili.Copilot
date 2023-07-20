// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.IO;
using System.Threading;
using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.Models;

namespace Bili.Copilot.Libs.Player.Core.Configs;

/// <summary>
/// 播放器配置类.
/// </summary>
public class PlayerConfig : ObservableObject
{
    private MediaPlayer.Player _player;
    private Config _config;
    private long _minBufferDuration = 500 * 10000;
    private long _maxLatency = 0;
    private long _minLatency = 0;
    private bool _seekAccurate;
    private bool _stats = false;
    private int _volumeMax = 150;
    private int _zoomOffset = 10;

    /// <summary>
    /// 打开或者结束后自动播放.
    /// </summary>
    public bool AutoPlay { get; set; } = true;

    /// <summary>
    /// 播放前需要缓冲的时长.
    /// </summary>
    public long MinBufferDuration
    {
        get => _minBufferDuration;
        set
        {
            if (!Set(ref _minBufferDuration, value))
            {
                return;
            }

            if (_config != null && value > _config.Demuxer.BufferDuration)
            {
                _config.Demuxer.BufferDuration = value;
            }
        }
    }

    /// <summary>
    /// 播放器未播放时的帧率.
    /// </summary>
    public double IdleFps { get; set; } = 60.0;

    /// <summary>
    /// 最大延迟（以 ticks 为单位），强制播放器（速度为x1+）停留在直播网络流的末尾（默认值：0 - 禁用）.
    /// </summary>
    public long MaxLatency
    {
        get => _maxLatency;
        set
        {
            if (value < 0)
            {
                value = 0;
            }

            if (!Set(ref _maxLatency, value))
            {
                return;
            }

            if (value == 0)
            {
                if (_player != null)
                {
                    _player.Speed = 1;
                }

                return;
            }

            // 设置较大的最大缓冲区以确保实际延迟距离
            if (_config != null && _config.Demuxer.BufferDuration < value * 2)
            {
                _config.Demuxer.BufferDuration = value * 2;
            }

            // 设置较小的最小缓冲区以避免直接启用延迟速度
            if (_minBufferDuration > value / 10)
            {
                MinBufferDuration = value / 10;
            }
        }
    }

    /// <summary>
    /// 最小延迟（以 ticks 为单位），防止MaxLatency（速度为x1）低于此限制（默认值：0 - 尽可能低）.
    /// </summary>
    public long MinLatency
    {
        get => _minLatency;
        set => Set(ref _minLatency, value);
    }

    /// <summary>
    /// 当启用MaxLatency时，防止频繁的速度更改（以避免音频/视频间隙和不同步）.
    /// </summary>
    public long LatencySpeedChangeInterval { get; set; } = TimeSpan.FromMilliseconds(700).Ticks;

    /// <summary>
    /// 保存录制的文件夹路径（当未指定文件名时）.
    /// </summary>
    public string FolderRecordings { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recordings");

    /// <summary>
    /// 保存快照的文件夹路径（当未指定文件名时）.
    /// </summary>
    public string FolderSnapshots { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Snapshots");

    /// <summary>
    /// 强制在视频上准确地进行 CurTime/SeekBackward/SeekForward.
    /// </summary>
    public bool SeekAccurate
    {
        get => _seekAccurate;
        set => Set(ref _seekAccurate, value);
    }

    /// <summary>
    /// 快照的编码格式（有效格式为 bmp、png、jpg/jpeg）.
    /// </summary>
    public string SnapshotFormat { get; set; } = "bmp";

    /// <summary>
    /// 是否刷新有关比特率/帧率/丢帧等的统计信息.
    /// </summary>
    public bool Stats
    {
        get => _stats;
        set => Set(ref _stats, value);
    }

    /// <summary>
    /// 设置播放线程的优先级.
    /// </summary>
    public ThreadPriority ThreadPriority { get; set; } = ThreadPriority.AboveNormal;

    /// <summary>
    /// 在每一帧上刷新UI中的CurTime（可能会导致性能问题）.
    /// </summary>
    public bool UICurTimePerFrame { get; set; } = false;

    /// <summary>
    /// 音量放大器的上限.
    /// </summary>
    public int VolumeMax
    {
        get => _volumeMax;
        set
        {
            Set(ref _volumeMax, value);
            if (_player != null && _player.Audio.masteringVoice != null)
            {
                _player.Audio.masteringVoice.Volume = value / 100f;
            }
        }
    }

    /// <summary>
    /// 播放器的用途.
    /// </summary>
    public PlayerUsage Usage { get; set; } = PlayerUsage.AVS;

    /// <summary>
    /// 音频延迟偏移量.
    /// </summary>
    public long AudioDelayOffset { get; set; } = 100 * 10000;

    /// <summary>
    /// 音频延迟偏移量2.
    /// </summary>
    public long AudioDelayOffset2 { get; set; } = 1000 * 10000;

    /// <summary>
    /// 字幕延迟偏移量.
    /// </summary>
    public long SubtitlesDelayOffset { get; set; } = 100 * 10000;

    /// <summary>
    /// 字幕延迟偏移量2.
    /// </summary>
    public long SubtitlesDelayOffset2 { get; set; } = 1000 * 10000;

    /// <summary>
    /// 搜索偏移量.
    /// </summary>
    public long SeekOffset { get; set; } = 5 * 1000L * 10000;

    /// <summary>
    /// 搜索偏移量2.
    /// </summary>
    public long SeekOffset2 { get; set; } = 15 * 1000L * 10000;

    /// <summary>
    /// 搜索偏移量3.
    /// </summary>
    public long SeekOffset3 { get; set; } = 30 * 1000L * 10000;

    /// <summary>
    /// 速度偏移量.
    /// </summary>
    public double SpeedOffset { get; set; } = 0.10;

    /// <summary>
    /// 速度偏移量2.
    /// </summary>
    public double SpeedOffset2 { get; set; } = 0.25;

    /// <summary>
    /// 缩放偏移量.
    /// </summary>
    public int ZoomOffset
    {
        get => _zoomOffset;
        set
        {
            if (Set(ref _zoomOffset, value))
            {
                _player?.ResetAll();
            }
        }
    }

    /// <summary>
    /// 音量偏移量.
    /// </summary>
    public int VolumeOffset { get; set; } = 5;

    /// <summary>
    /// 克隆当前配置.
    /// </summary>
    /// <returns><see cref="PlayerConfig"/>.</returns>
    public PlayerConfig Clone()
    {
        var player = (PlayerConfig)MemberwiseClone();
        player._player = null;
        player._config = null;
        return player;
    }

    internal void SetConfig(Config config)
        => _config = config;

    internal void SetPlayer(MediaPlayer.Player player)
        => _player = player;
}
