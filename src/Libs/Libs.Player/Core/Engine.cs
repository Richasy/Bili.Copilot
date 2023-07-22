// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Player.Core.Configs;
using Bili.Copilot.Libs.Player.Core.Engines;
using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.MediaFramework.MediaDevice;
using Bili.Copilot.Libs.Player.Misc;
using Microsoft.UI.Dispatching;

namespace Bili.Copilot.Libs.Player.Core;

/// <summary>
/// 播放引擎.
/// </summary>
public static class Engine
{
    private static readonly object _lockEngine = new();
    private static Thread _tMaster;
    private static bool _isLoading;
    private static int _timePeriod;

    /// <summary>
    /// 加载完成事件.
    /// </summary>
    public static event EventHandler Loaded;

    /// <summary>
    /// 引擎已经加载好并准备使用.
    /// </summary>
    public static bool IsLoaded { get; private set; }

    /// <summary>
    /// 引擎配置.
    /// </summary>
    public static EngineConfig Config { get; private set; }

    /// <summary>
    /// 音频引擎.
    /// </summary>
    public static AudioEngine Audio { get; private set; }

    /// <summary>
    /// 视频引擎.
    /// </summary>
    public static VideoEngine Video { get; private set; }

    /// <summary>
    /// 插件引擎.
    /// </summary>
    public static PluginsEngine Plugins { get; private set; }

    /// <summary>
    /// FFmpeg 引擎.
    /// </summary>
    public static FFmpegEngine FFmpeg { get; private set; }

    /// <summary>
    /// 活动播放器列表.
    /// </summary>
    public static List<MediaPlayer.Player> Players { get; private set; }

    internal static LogHandler Log { get; private set; }

    /// <summary>
    /// 初始化播放引擎（必须从 UI 线程调用）.
    /// </summary>
    /// <param name="config">引擎配置.</param>
    public static void Start(EngineConfig config = null)
        => StartInternal(config);

    /// <summary>
    /// 异步初始化播放引擎（必须从 UI 线程调用）.
    /// </summary>
    /// <param name="config">引擎配置.</param>
    public static void StartAsync(EngineConfig config = null)
        => StartInternal(config, true);

    /// <summary>
    /// 请求 timeBeginPeriod(1) - 当不再需要时应调用 TimeEndPeriod1.
    /// </summary>
    public static void TimeBeginPeriod1()
    {
        lock (_lockEngine)
        {
            _timePeriod++;

            if (_timePeriod == 1)
            {
                Log.Trace("timeBeginPeriod(1)");
                NativeMethods.TimeBeginPeriod(1);
            }
        }
    }

    /// <summary>
    /// 停止之前请求的 timeBeginPeriod(1).
    /// </summary>
    public static void TimeEndPeriod1()
    {
        lock (_lockEngine)
        {
            _timePeriod--;

            if (_timePeriod == 0)
            {
                Log.Trace("timeEndPeriod(1)");
                NativeMethods.TimeEndPeriod(1);
            }
        }
    }

    internal static void AddPlayer(MediaPlayer.Player player)
    {
        lock (Players)
        {
            Players.Add(player);
        }
    }

    internal static int GetPlayerPos(int playerId)
    {
        for (var i = 0; i < Players.Count; i++)
        {
            if (Players[i].PlayerId == playerId)
            {
                return i;
            }
        }

        return -1;
    }

    internal static void DisposePlayer(MediaPlayer.Player player)
    {
        if (player == null)
        {
            return;
        }

        DisposePlayer(player.PlayerId);
    }

    internal static void DisposePlayer(int playerId)
    {
        lock (Players)
        {
            Log.Trace($"Disposing {playerId}");
            var pos = GetPlayerPos(playerId);
            if (pos == -1)
            {
                return;
            }

            var player = Players[pos];
            player.DisposeInternal();
            Players.RemoveAt(pos);
            Log.Trace($"Disposed {playerId}");
        }
    }

    internal static void StartThread()
    {
        if (!EngineConfig.UIRefresh || (_tMaster != null && _tMaster.IsAlive))
        {
            return;
        }

        _tMaster = new Thread(() => { MasterThread(); })
        {
            Name = "PlayerEngine",
            IsBackground = true,
        };
        _tMaster.Start();
    }

    internal static void MasterThread()
    {
        Log.Info("Thread started");

        var curLoop = 0;
        var secondLoops = 1000 / Config.UIRefreshInterval;
        var prevTicks = DateTime.UtcNow.Ticks;
        double curSecond = 0;

        do
        {
            try
            {
                if (Players.Count == 0)
                {
                    Thread.Sleep(Config.UIRefreshInterval);
                    continue;
                }

                curLoop++;
                if (curLoop == secondLoops)
                {
                    var curTicks = DateTime.UtcNow.Ticks;
                    curSecond = (curTicks - prevTicks) / 10000000.0;
                    prevTicks = curTicks;
                }

                lock (Players)
                {
                    foreach (var player in Players)
                    {
                        /* 每个 UIRefreshInterval */
                        player.Activity.RefreshMode();

                        /* 每秒钟 */
                        if (curLoop == secondLoops)
                        {
                            if (player.Config.Player.Stats)
                            {
                                var curStats = player.Stats;
                                var curTotalBytes = player.VideoDemuxer.TotalBytes + player.AudioDemuxer.TotalBytes + player.SubtitlesDemuxer.TotalBytes;
                                var curVideoBytes = player.VideoDemuxer.VideoPackets.Bytes + player.AudioDemuxer.VideoPackets.Bytes + player.SubtitlesDemuxer.VideoPackets.Bytes;
                                var curAudioBytes = player.VideoDemuxer.AudioPackets.Bytes + player.AudioDemuxer.AudioPackets.Bytes + player.SubtitlesDemuxer.AudioPackets.Bytes;

                                player.BitRate = (curTotalBytes - curStats.TotalBytes) * 8 / 1000.0;
                                player.Video.BitRate = (curVideoBytes - curStats.VideoBytes) * 8 / 1000.0;
                                player.Audio.BitRate = (curAudioBytes - curStats.AudioBytes) * 8 / 1000.0;

                                curStats.TotalBytes = curTotalBytes;
                                curStats.VideoBytes = curVideoBytes;
                                curStats.AudioBytes = curAudioBytes;

                                if (player.IsPlaying)
                                {
                                    player.Video.FpsCurrent = (player.Video.FramesDisplayed - curStats.FramesDisplayed) / curSecond;
                                    curStats.FramesDisplayed = player.Video.FramesDisplayed;
                                }
                            }
                        }
                    }
                }

                if (curLoop == secondLoops)
                {
                    curLoop = 0;
                }

                Action action = () =>
                {
                    try
                    {
                        foreach (var player in Players)
                        {
                            /* 每个 UIRefreshInterval */

                            // 活动模式刷新和隐藏鼠标光标（仅全屏）
                            player.Activity.SetMode();

                            // 当前时间 / 缓冲持续时间（HLS 还需加上 Duration）
                            if (!Config.UICurTimePerSecond)
                            {
                                player.UpdateCurTime();
                            }
                            else if (player.Status == PlayerStatus.Paused)
                            {
                                if (player.MainDemuxer.IsRunning)
                                {
                                    player.UpdateCurTime();
                                }
                                else
                                {
                                    player.UpdateBufferedDuration();
                                }
                            }

                            /* 每秒钟 */
                            if (curLoop == 0)
                            {
                                // 统计信息刷新（比特率 / 帧显示数 / 帧丢失数 / FPS）
                                if (player.Config.Player.Stats)
                                {
                                    player.BitRate = player.BitRate;
                                    player.Video.BitRate = player.Video.BitRate;
                                    player.Audio.BitRate = player.Audio.BitRate;

                                    if (player.IsPlaying)
                                    {
                                        player.Audio.FramesDisplayed = player.Audio.FramesDisplayed;
                                        player.Audio.FramesDropped = player.Audio.FramesDropped;

                                        player.Video.FramesDisplayed = player.Video.FramesDisplayed;
                                        player.Video.FramesDropped = player.Video.FramesDropped;
                                        player.Video.FpsCurrent = player.Video.FpsCurrent;
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                };

                Utils.UI(action);
                Thread.Sleep(Config.UIRefreshInterval);
            }
            catch
            {
                curLoop = 0;
            }
        }
        while (EngineConfig.UIRefresh);

        Log.Info("Thread stopped");
    }

    private static void StartInternal(EngineConfig config = null, bool async = false)
    {
        lock (_lockEngine)
        {
            if (_isLoading)
            {
                return;
            }

            _isLoading = true;

            Config = config ?? new EngineConfig();

            Utils.DispatcherQueue = DispatcherQueue.GetForCurrentThread();
            StartInternalUI();

            if (async)
            {
                Task.Run(() => StartInternalNonUI());
            }
            else
            {
                StartInternalNonUI();
            }
        }
    }

    private static void StartInternalUI()
    {
        Logger.SetOutput();

        Log = new LogHandler("[FlyleafEngine] ");

        Audio = new AudioEngine();
        if (Config.FFmpegDevices)
        {
            AudioDevice.RefreshDevices();
        }
    }

    private static void StartInternalNonUI()
    {
        var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        Log.Info($"BiliPlayer {version.Major}.{version.Minor}.{version.Build}");

        FFmpeg = new FFmpegEngine();
        Video = new VideoEngine();
        if (Config.FFmpegDevices)
        {
            VideoDevice.RefreshDevices();
        }

        Plugins = new PluginsEngine();
        Players = new List<MediaPlayer.Player>();

        IsLoaded = true;
        Loaded?.Invoke(null, null);

        if (EngineConfig.UIRefresh)
        {
            StartThread();
        }
    }
}
