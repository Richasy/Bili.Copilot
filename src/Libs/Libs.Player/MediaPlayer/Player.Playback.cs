// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Player.Core;
using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.MediaFramework.MediaDecoder;
using Bili.Copilot.Libs.Player.Misc;
using Bili.Copilot.Libs.Player.Models;
using static Bili.Copilot.Libs.Player.Misc.Logger;
using static Bili.Copilot.Libs.Player.Utils;

namespace Bili.Copilot.Libs.Player.MediaPlayer;

/// <summary>
/// 播放器.
/// </summary>
public partial class Player
{
    /// <summary>
    /// 当播放停止时，由错误或成功完成/结束触发 <see cref="Status"/>。
    /// 警告：使用 Invoke，并且它来自播放线程，因此无法暂停/停止等操作。如果需要，您需要使用另一个线程。
    /// </summary>
    public event EventHandler<PlaybackStoppedEventArgs> PlaybackStopped;

    /// <summary>
    /// 播放 AVS 流.
    /// </summary>
    public void Play()
    {
        lock (_lockActions)
        {
            if (!CanPlay || Status == PlayerStatus.Playing || Status == PlayerStatus.Ended)
            {
                return;
            }

            UI(() => Status = PlayerStatus.Playing);
        }

        while (_taskPlayRuns || _taskSeekRuns)
        {
            Thread.Sleep(5);
        }

        _taskPlayRuns = true;

        // Long-Run Task
        Thread t = new(() =>
        {
            try
            {
                Engine.TimeBeginPeriod1();
                NativeMethods.SetThreadExecutionState(NativeMethods.EXECUTION_STATE.ES_CONTINUOUS | NativeMethods.EXECUTION_STATE.ES_SYSTEM_REQUIRED | NativeMethods.EXECUTION_STATE.ES_DISPLAY_REQUIRED);

                _onBufferingStarted = 0;
                _onBufferingCompleted = 0;
                RequiresBuffering = true;

                UI(() => LastError = null);

                if (Config.Player.Usage == PlayerUsage.Audio || !Video.IsOpened)
                {
                    ScreamerAudioOnly();
                }
                else
                {
                    if (ReversePlayback)
                    {
                        ScreamerReverse();
                    }
                    else
                    {
                        Screamer();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"播放失败 ({e.Message})");
            }
            finally
            {
                VideoDecoder.DisposeFrame(_videoFrame);
                _videoFrame = null;

                if (Status == PlayerStatus.Stopped)
                {
                    Decoder?.Initialize();
                }
                else if (Decoder != null)
                {
                    Decoder.PauseOnQueueFull();
                    Decoder.PauseDecoders();
                }

                Audio.ClearBuffer();
                Engine.TimeEndPeriod1();
                NativeMethods.SetThreadExecutionState(NativeMethods.EXECUTION_STATE.ES_CONTINUOUS);
                _stoppedWithError = false;

                if (IsPlaying)
                {
                    if (DecoderHasEnded)
                    {
                        Status = PlayerStatus.Ended;
                    }
                    else
                    {
                        if (_onBufferingStarted - 1 == _onBufferingCompleted)
                        {
                            _stoppedWithError = true;
                            OnBufferingCompleted("缓冲失败");
                        }
                        else
                        {
                            _stoppedWithError = !ReversePlayback ? IsLive || Math.Abs(Duration - CurTime) > 3 * 1000 * 10000 : CurTime > 3 * 1000 * 10000;
                        }

                        Status = PlayerStatus.Paused;
                    }
                }

                OnPlaybackStopped(_stoppedWithError ? "播放意外停止" : null);
                if (CanDebug)
                {
                    Log.Debug($"[SCREAMER] 完成 (状态: {Status}, 错误: {(_stoppedWithError ? "播放意外停止" : string.Empty)})");
                }

                UI(() =>
                {
                    Status = Status;
                    UpdateCurTime();
                });

                _taskPlayRuns = false;
            }
        });
        t.Priority = Config.Player.ThreadPriority;
        t.Name = $"[#{PlayerId}] 播放";
        t.IsBackground = true;
        t.Start();
    }

    /// <summary>
    /// 暂停 AVS 流.
    /// </summary>
    public void Pause()
    {
        lock (_lockActions)
        {
            if (!CanPlay || Status == PlayerStatus.Ended)
            {
                return;
            }

            UI(() => Status = PlayerStatus.Paused);

            while (_taskPlayRuns)
            {
                Thread.Sleep(5);
            }
        }
    }

    /// <summary>
    /// 切换播放/暂停状态.
    /// </summary>
    public void TogglePlayPause()
    {
        if (IsPlaying)
        {
            Pause();
        }
        else
        {
            Play();
        }
    }

    /// <summary>
    /// 切换反向播放状态.
    /// </summary>
    public void ToggleReversePlayback() => ReversePlayback = !ReversePlayback;

    /// <summary>
    /// 根据指定的毫秒数向前或向后寻找最近的关键帧.
    /// </summary>
    /// <param name="ms">毫秒值.</param>
    /// <param name="forward">是否向前查找.</param>
    public void Seek(int ms, bool forward = false) => Seek(ms, forward, false);

    /// <summary>
    /// 在精确的时间戳上寻找（具有半帧距离精度）.
    /// </summary>
    /// <param name="ms">毫秒值.</param>
    public void SeekAccurate(int ms) => Seek(ms, false, !IsLive);

    /// <summary>
    /// 切换精确搜索.
    /// </summary>
    public void ToggleSeekAccurate()
        => Config.Player.SeekAccurate = !Config.Player.SeekAccurate;

    /// <summary>
    /// 刷新缓冲区（解复用器（数据包）和解码器（帧））
    /// 这对于实时流主要用于将播放推到最后（低延迟）很有用.
    /// </summary>
    public void Flush() => Decoder.Flush();

    /// <summary>
    /// 停止并关闭 AVS 流.
    /// </summary>
    public void Stop()
    {
        lock (_lockActions)
        {
            Initialize();
            Renderer.Flush();
        }
    }

    /// <summary>
    /// 当播放停止时，由错误或成功完成/结束触发 <see cref="Status"/>.
    /// </summary>
    /// <param name="error">错误信息.</param>
    protected virtual void OnPlaybackStopped(string error = null)
    {
        if (error != null && LastError == null)
        {
            LastError = error;
            UI(() => LastError = LastError);
        }

        PlaybackStopped?.Invoke(this, new PlaybackStoppedEventArgs(error));
    }

    private void Seek(int ms, bool forward, bool accurate)
    {
        if (!CanPlay)
        {
            return;
        }

        lock (_seeks)
        {
            CurTime = ms * 10000L;
            _seeks.Push(new SeekData(ms, forward, accurate));
        }

        OnPropertyChanged(nameof(CurTime));

        if (Status == PlayerStatus.Playing)
        {
            return;
        }

        lock (_lockActions)
        {
            if (_taskSeekRuns)
            {
                return;
            }

            _taskSeekRuns = true;
        }

        Task.Run(() =>
        {
            int ret;
            var wasEnded = false;

            try
            {
                Engine.TimeBeginPeriod1();

                while (_seeks.TryPop(out var seekData) && CanPlay && !IsPlaying)
                {
                    _seeks.Clear();

                    if (Status == PlayerStatus.Ended)
                    {
                        wasEnded = true;
                        Status = PlayerStatus.Paused;
                        UI(() => Status = Status);
                    }

                    if (!Video.IsOpened)
                    {
                        if (AudioDecoder.OnVideoDemuxer)
                        {
                            ret = Decoder.Seek(seekData.Ms, seekData.Forward);
                            if (CanWarn && ret < 0)
                            {
                                Log.Warn("寻找失败 2");
                            }

                            VideoDemuxer.Start();
                        }
                        else
                        {
                            ret = Decoder.SeekAudio(seekData.Ms, seekData.Forward);
                            if (CanWarn && ret < 0)
                            {
                                Log.Warn("寻找失败 3");
                            }

                            AudioDemuxer.Start();
                        }

                        Decoder.PauseOnQueueFull();
                    }
                    else
                    {
                        Decoder.PauseDecoders();
                        ret = Decoder.Seek(seekData.Ms, seekData.Forward, !seekData.Accurate);
                        if (ret < 0)
                        {
                            if (CanWarn)
                            {
                                Log.Warn("寻找失败");
                            }
                        }
                        else if (!ReversePlayback && CanPlay)
                        {
                            Decoder.GetVideoFrame(seekData.Accurate ? seekData.Ms * 10000L : -1);
                            ShowOneFrame();
                            VideoDemuxer.Start();
                            AudioDemuxer.Start();
                            SubtitlesDemuxer.Start();
                            Decoder.PauseOnQueueFull();
                        }
                    }

                    Thread.Sleep(20);
                }
            }
            catch (Exception e)
            {
                Log.Error($"寻找失败 ({e.Message})");
            }
            finally
            {
                Decoder.OpenedPlugin?.OnBufferingCompleted();
                Engine.TimeEndPeriod1();
                lock (_lockActions)
                {
                    _taskSeekRuns = false;
                }

                if ((wasEnded && Config.Player.AutoPlay) || _stoppedWithError)
                {
                    Play();
                }
            }
        });
    }
}
