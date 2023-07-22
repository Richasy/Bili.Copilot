// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Player.Core;
using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.MediaFramework.MediaDemuxer;
using Bili.Copilot.Libs.Player.MediaFramework.MediaRemuxer;
using CommunityToolkit.Mvvm.ComponentModel;
using FFmpeg.AutoGen;
using static Bili.Copilot.Libs.Player.Misc.Logger;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaContext;

/// <summary>
/// 下载或重新封装任何 ffmpeg 可用的输入 URL 到不同的格式.
/// </summary>
public unsafe partial class Downloader : RunThreadBase
{
    private double _downPercentageFactor; // 下载进度的因子

    /// <summary>
    /// 当前帧的时间戳（从0开始，以 Ticks 表示）.
    /// </summary>
    [ObservableProperty]
    private long _curTime; // 当前帧的时间戳（从0开始，以 Ticks 表示）

    /// <summary>
    /// 当前下载进度的百分比（对于直播流为0）.
    /// </summary>
    [ObservableProperty]
    private double _downloadPercentage;

    /// <summary>
    /// 输入的总时长（以 Ticks 表示）.
    /// </summary>
    [ObservableProperty]
    private long _duration;

    /// <summary>
    /// 构造函数.
    /// </summary>
    /// <param name="config">配置信息.</param>
    /// <param name="uniqueId">唯一标识符.</param>
    public Downloader(Config config, int uniqueId = -1)
        : base(uniqueId)
    {
        DecoderContext = new DecoderContext(config, UniqueId, false);
        DecoderContext.AudioDemuxer.Config = config.Demuxer.Clone(); // 更改缓冲区持续时间，避免更改视频解封装器的配置

        Remuxer = new Remuxer(UniqueId);
        ThreadName = "Downloader";
    }

    /// <summary>
    /// 当下载完成或失败时触发.
    /// </summary>
    public event EventHandler<bool> DownloadCompleted;

    /// <summary>
    /// 解码器上下文.
    /// </summary>
    public DecoderContext DecoderContext { get; private set; }

    /// <summary>
    /// 后端重新封装器.通常不应直接访问此属性.
    /// </summary>
    public Remuxer Remuxer { get; private set; }

    /// <summary>
    /// 音频解封装器.
    /// </summary>
    internal Demuxer AudioDemuxer => DecoderContext.AudioDemuxer;

    /// <summary>
    /// 打开一个新的媒体文件（音频/视频）并准备下载（阻塞）.
    /// </summary>
    /// <param name="url">媒体文件的 URL.</param>
    /// <param name="defaultPlaylistItem">是否打开默认输入.</param>
    /// <param name="defaultVideo">是否打开插件建议的默认视频流.</param>
    /// <param name="defaultAudio">是否打开插件建议的默认音频流.</param>
    /// <returns>错误信息.</returns>
    public string Open(string url, bool defaultPlaylistItem = true, bool defaultVideo = true, bool defaultAudio = true)
    {
        lock (LockActions)
        {
            Dispose();

            Disposed = false;
            Status = ThreadStatus.Opening;
            var ret = DecoderContext.Open(url, defaultPlaylistItem, defaultVideo, defaultAudio, false);
            if (ret != null && ret.Error != null)
            {
                return ret.Error;
            }

            CurTime = 0;
            DownloadPercentage = 0;

            var demuxer = !DecoderContext.VideoDemuxer.Disposed ? DecoderContext.VideoDemuxer : DecoderContext.AudioDemuxer;
            Duration = demuxer.IsLive ? 0 : demuxer.Duration;
            _downPercentageFactor = Duration / 100.0;

            return null;
        }
    }

    /// <summary>
    /// 下载当前配置的 AVS 流.
    /// </summary>
    /// <param name="filename">下载视频的文件名.文件扩展名将由解封装器选择输出格式（例如 mp4）.如果使用 RecommendedExtension，则会更新为相应的扩展名.</param>
    /// <param name="useRecommendedExtension">尝试将输出容器与输入容器匹配.</param>
    public void Download(ref string filename, bool useRecommendedExtension = true)
    {
        lock (LockActions)
        {
            if (Status != ThreadStatus.Opening || Disposed)
            {
                OnDownloadCompleted(false);
                return;
            }

            if (useRecommendedExtension)
            {
                filename = $"{filename}.{(!DecoderContext.VideoDemuxer.Disposed ? DecoderContext.VideoDemuxer.Extension : DecoderContext.AudioDemuxer.Extension)}";
            }

            var ret = Remuxer.Open(filename);
            if (ret != 0)
            {
                OnDownloadCompleted(false);
                return;
            }

            AddStreams(DecoderContext.VideoDemuxer);
            AddStreams(DecoderContext.AudioDemuxer);

            if (!Remuxer.HasStreams || Remuxer.WriteHeader() != 0)
            {
                OnDownloadCompleted(false);
                return;
            }

            Start();
        }
    }

    /// <summary>
    /// 停止并释放下载器.
    /// </summary>
    public void Dispose()
    {
        if (Disposed)
        {
            return;
        }

        lock (LockActions)
        {
            if (Disposed)
            {
                return;
            }

            Stop();

            DecoderContext.Dispose();
            Remuxer.Dispose();

            Status = ThreadStatus.Stopped;
            Disposed = true;
        }
    }

    /// <summary>
    /// 当下载完成或失败时触发.
    /// </summary>
    /// <param name="success">是否成功.</param>
    protected virtual void OnDownloadCompleted(bool success)
        => Task.Run(() =>
        {
            Dispose();
            DownloadCompleted?.Invoke(this, success);
        });

    /// <summary>
    /// 内部运行方法.
    /// </summary>
    protected override void RunInternal()
    {
        if (!Remuxer.HasStreams)
        {
            OnDownloadCompleted(false);
            return;
        }

        // 不允许音频在没有视频的情况下改变我们的时长（需要等待视频解封装器的时间戳）
        long resetBufferDuration = -1;
        var hasAVDemuxers = !DecoderContext.VideoDemuxer.Disposed && !DecoderContext.AudioDemuxer.Disposed;
        if (hasAVDemuxers)
        {
            resetBufferDuration = DecoderContext.AudioDemuxer.Config.BufferDuration;
            DecoderContext.AudioDemuxer.Config.BufferDuration = 100 * 10000;
        }

        DecoderContext.Start();

        var demuxer = !DecoderContext.VideoDemuxer.Disposed ? DecoderContext.VideoDemuxer : DecoderContext.AudioDemuxer;
        var startTime = demuxer.hlsCtx == null ? demuxer.StartTime : demuxer.hlsCtx->first_timestamp * 10;
        Duration = demuxer.IsLive ? 0 : demuxer.Duration;
        _downPercentageFactor = Duration / 100.0;

        var startedAt = DateTime.UtcNow.Ticks;
        var secondTicks = startedAt;
        var isAudioDemuxer = DecoderContext.VideoDemuxer.Disposed && !DecoderContext.AudioDemuxer.Disposed;
        var pktPtr = IntPtr.Zero;
        AVPacket* packet = null;
        AVPacket* packet2 = null;

        do
        {
            if ((demuxer.Packets.Count == 0 && AudioDemuxer.Packets.Count == 0) || (hasAVDemuxers && (demuxer.Packets.Count == 0 || AudioDemuxer.Packets.Count == 0)))
            {
                lock (LockStatus)
                {
                    if (Status == ThreadStatus.Running)
                    {
                        Status = ThreadStatus.QueueEmpty;
                    }
                }

                while (demuxer.Packets.Count == 0 && Status == ThreadStatus.QueueEmpty)
                {
                    if (demuxer.Status == ThreadStatus.Ended)
                    {
                        Status = ThreadStatus.Ended;
                        if (demuxer.Interrupter.Interrupted == 0)
                        {
                            CurTime = Duration;
                            DownloadPercentage = 100;
                        }

                        break;
                    }
                    else if (!demuxer.IsRunning)
                    {
                        if (CanDebug)
                        {
                            Log.Debug($"Demuxer is not running [Demuxer Status: {demuxer.Status}]");
                        }

                        lock (demuxer.LockStatus)
                            lock (LockStatus)
                            {
                                if (demuxer.Status == ThreadStatus.Pausing || demuxer.Status == ThreadStatus.Paused)
                                {
                                    Status = ThreadStatus.Pausing;
                                }
                                else if (demuxer.Status != ThreadStatus.Ended)
                                {
                                    Status = ThreadStatus.Stopping;
                                }
                                else
                                {
                                    continue;
                                }
                            }

                        break;
                    }

                    Thread.Sleep(20);
                }

                if (hasAVDemuxers && Status == ThreadStatus.QueueEmpty && AudioDemuxer.Packets.Count == 0)
                {
                    while (AudioDemuxer.Packets.Count == 0 && Status == ThreadStatus.QueueEmpty && AudioDemuxer.IsRunning)
                    {
                        Thread.Sleep(20);
                    }
                }

                lock (LockStatus)
                {
                    if (Status != ThreadStatus.QueueEmpty)
                    {
                        break;
                    }

                    Status = ThreadStatus.Running;
                }
            }

            if (hasAVDemuxers)
            {
                if (AudioDemuxer.Packets.Count == 0)
                {
                    packet = demuxer.Packets.Dequeue();
                    isAudioDemuxer = false;
                }
                else if (demuxer.Packets.Count == 0)
                {
                    packet = AudioDemuxer.Packets.Dequeue();
                    isAudioDemuxer = true;
                }
                else
                {
                    packet = demuxer.Packets.Peek();
                    packet2 = AudioDemuxer.Packets.Peek();

                    var ts1 = (long)((packet->dts * demuxer.AVStreamToStream[packet->stream_index].TimeBase) - demuxer.StartTime);
                    var ts2 = (long)((packet2->dts * AudioDemuxer.AVStreamToStream[packet2->stream_index].TimeBase) - AudioDemuxer.StartTime);

                    if (ts2 <= ts1)
                    {
                        AudioDemuxer.Packets.Dequeue();
                        isAudioDemuxer = true;
                        packet = packet2;
                    }
                    else
                    {
                        demuxer.Packets.Dequeue();
                        isAudioDemuxer = false;
                    }
                }
            }
            else
            {
                packet = demuxer.Packets.Dequeue();
            }

            var curDT = DateTime.UtcNow.Ticks;
            if (curDT - secondTicks > 1000 * 10000 && (!isAudioDemuxer || DecoderContext.VideoDemuxer.Disposed))
            {
                secondTicks = curDT;

                CurTime = demuxer.hlsCtx != null
                    ? (long)((packet->dts * demuxer.AVStreamToStream[packet->stream_index].TimeBase) - startTime)
                    : demuxer.CurTime + demuxer.BufferedDuration;

                if (Duration > 0)
                {
                    DownloadPercentage = CurTime / _downPercentageFactor;
                }
            }

            Remuxer.Write(packet, isAudioDemuxer);
        }
        while (Status == ThreadStatus.Running);

        if (resetBufferDuration != -1)
        {
            DecoderContext.AudioDemuxer.Config.BufferDuration = resetBufferDuration;
        }

        if (Status != ThreadStatus.Pausing && Status != ThreadStatus.Paused)
        {
            OnDownloadCompleted(Remuxer.WriteTrailer() == 0);
        }
        else
        {
            demuxer.Pause();
        }
    }

    private void AddStreams(Demuxer demuxer)
    {
        for (var i = 0; i < demuxer.EnabledStreams.Count; i++)
        {
            if (Remuxer.AddStream(demuxer.AVStreamToStream[demuxer.EnabledStreams[i]].AVStream, demuxer.Type == MediaType.Audio) != 0)
            {
                Log.Warn($"Failed to add stream {demuxer.AVStreamToStream[demuxer.EnabledStreams[i]].Type} {demuxer.AVStreamToStream[demuxer.EnabledStreams[i]].StreamIndex}");
            }
        }
    }
}
