// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Threading;
using Bili.Copilot.Libs.Player.Core;
using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.Models;
using FFmpeg.AutoGen;
using static Bili.Copilot.Libs.Player.Misc.Logger;
using static Bili.Copilot.Libs.Player.Utils;

namespace Bili.Copilot.Libs.Player.MediaPlayer;

/// <summary>
/// 播放器.
/// </summary>
#pragma warning disable SA1108
public unsafe partial class Player
{
    /// <summary>
    /// 缓冲开始时触发.
    /// 警告：使用 Invoke，来自播放线程，因此无法暂停/停止等操作.如果需要，您需要使用另一个线程.
    /// </summary>
    public event EventHandler BufferingStarted;

    /// <summary>
    /// 缓冲完成时触发（也会在缓冲失败时触发）.
    /// (BufferDration > Config.Player.MinBufferDuration)
    /// 警告：使用 Invoke，来自播放线程，因此无法暂停/停止等操作.如果需要，您需要使用另一个线程.
    /// </summary>
    public event EventHandler<BufferingCompletedEventArgs> BufferingCompleted;

    /// <summary>
    /// 触发缓冲开始事件.
    /// </summary>
    protected virtual void OnBufferingStarted()
    {
        if (_onBufferingStarted != _onBufferingCompleted)
        {
            return;
        }

        BufferingStarted?.Invoke(this, new EventArgs());
        _onBufferingStarted++;

        if (CanDebug)
        {
            Log.Debug($"OnBufferingStarted");
        }
    }

    /// <summary>
    /// 触发缓冲完成事件.
    /// </summary>
    /// <param name="error">错误信息.</param>
    protected virtual void OnBufferingCompleted(string error = null)
    {
        if (_onBufferingStarted - 1 != _onBufferingCompleted)
        {
            return;
        }

        if (error != null && LastError == null)
        {
            UI(() => LastError = error);
        }

        BufferingCompleted?.Invoke(this, new BufferingCompletedEventArgs(error));
        _onBufferingCompleted++;
        if (CanDebug)
        {
            Log.Debug($"OnBufferingCompleted{(error != null ? $" (Error: {error})" : string.Empty)}");
        }
    }

    private void ShowOneFrame()
    {
        SubtitleFrame = null;
        UIAdd(() => Subtitles.SubtitleText = string.Empty);

        if (!VideoDecoder.Frames.IsEmpty)
        {
            VideoDecoder.Frames.TryDequeue(out _videoFrame);
            if (_videoFrame != null) // might come from video input switch interrupt
            {
                Renderer.Present(_videoFrame);
            }

            if (_seeks.IsEmpty)
            {
                if (!VideoDemuxer.IsHLSLive)
                {
                    CurTime = _videoFrame.Timestamp;
                }

                UIAdd(() => UpdateCurTime());
                UIAll();
            }

            // Required for buffering on paused
            if (Decoder.RequiresResync && !IsPlaying && _seeks.IsEmpty)
            {
                Decoder.Resync(_videoFrame.Timestamp);
            }

            _videoFrame = null;
        }

        UIAll();
    }

    private bool MediaBuffer()
    {
        if (CanTrace)
        {
            Log.Trace("Buffering");
        }

        while (_isVideoSwitch && IsPlaying)
        {
            Thread.Sleep(10);
        }

        Audio.ClearBuffer();

        VideoDemuxer.Start();
        VideoDecoder.Start();

        if (Audio.IsOpened && Config.Audio.Enabled)
        {
            _curAudioDeviceDelay = Audio.GetDeviceDelay();

            if (AudioDecoder.OnVideoDemuxer)
            {
                AudioDecoder.Start();
            }
            else if (!Decoder.RequiresResync)
            {
                AudioDemuxer.Start();
                AudioDecoder.Start();
            }
        }

        if (Subtitles.IsOpened && Config.Subtitles.Enabled)
        {
            lock (_lockSubtitles)
            {
                if (SubtitlesDecoder.OnVideoDemuxer)
                {
                    SubtitlesDecoder.Start();
                }
                else if (!Decoder.RequiresResync)
                {
                    SubtitlesDemuxer.Start();
                    SubtitlesDecoder.Start();
                }
            }
        }

        MediaFramework.MediaDecoder.VideoDecoder.DisposeFrame(_videoFrame);
        _videoFrame = null;
        AudioFrame = null;
        SubtitleFrame = null;
        SubtitleFramePrev = null;

        var gotAudio = !Audio.IsOpened || Config.Player.MaxLatency != 0;
        var gotVideo = false;
        var shouldStop = false;
        var showOneFrame = true;
        var audioRetries = 4;
        var loops = 0;

        if (Config.Player.MaxLatency != 0)
        {
            _lastSpeedChangeTicks = DateTime.UtcNow.Ticks;
            showOneFrame = false;
            Speed = 1;
        }

        do
        {
            loops++;

            if (showOneFrame && !VideoDecoder.Frames.IsEmpty)
            {
                ShowOneFrame();
                showOneFrame = false;
            }

            // We allo few ms to show a frame before cancelling
            if ((!showOneFrame || loops > 8) && !_seeks.IsEmpty)
            {
                return false;
            }

            if (!gotVideo && !showOneFrame && !VideoDecoder.Frames.IsEmpty)
            {
                VideoDecoder.Frames.TryDequeue(out _videoFrame);
                if (_videoFrame != null)
                {
                    gotVideo = true;
                }
            }

            if (!gotAudio && AudioFrame == null && !AudioDecoder.Frames.IsEmpty)
            {
                AudioDecoder.Frames.TryDequeue(out var audioFrame);
                AudioFrame = audioFrame;
            }

            if (gotVideo)
            {
                if (Decoder.RequiresResync)
                {
                    Decoder.Resync(_videoFrame.Timestamp);
                }

                if (!gotAudio && AudioFrame != null)
                {
                    for (var i = 0; i < Math.Min(50, AudioDecoder.Frames.Count); i++)
                    {
                        if (AudioFrame == null
                            || AudioFrame.Timestamp - _curAudioDeviceDelay > _videoFrame.Timestamp
                            || _videoFrame.Timestamp > Duration)
                        {
                            gotAudio = true;
                            break;
                        }

                        if (CanInfo)
                        {
                            Log.Info($"Drop AudioFrame {TicksToTime(AudioFrame.Timestamp)}");
                        }

                        AudioDecoder.Frames.TryDequeue(out var audioFrame);
                        AudioFrame = audioFrame;
                    }

                    // Avoid infinite loop in case of all audio Timestamps wrong
                    if (!gotAudio)
                    {
                        audioRetries--;

                        if (audioRetries < 1)
                        {
                            gotAudio = true;
                            AudioFrame = null;
                            Log.Warn($"Audio Exhausted 1");
                        }
                    }
                }
            }

            if (!IsPlaying || DecoderHasEnded)
            {
                shouldStop = true;
            }
            else
            {
                if (!VideoDecoder.IsRunning && !_isVideoSwitch)
                {
                    Log.Warn("Video Exhausted");
                    shouldStop = true;
                }

                if (gotVideo && !gotAudio && audioRetries > 0 && (!AudioDecoder.IsRunning || AudioDecoder.Demuxer.Status == ThreadStatus.QueueFull))
                {
                    if (CanWarn)
                    {
                        Log.Warn($"Audio Exhausted 2 | {audioRetries}");
                    }

                    audioRetries--;

                    if (audioRetries < 1)
                    {
                        gotAudio = true;
                    }
                }
            }

            Thread.Sleep(10);
        }
        while (!shouldStop && (!gotVideo || !gotAudio));

        if (shouldStop && !(DecoderHasEnded && IsPlaying && _videoFrame != null))
        {
            Log.Info("Stopped");
            return false;
        }

        if (_videoFrame == null)
        {
            Log.Error("No Frames!");
            return false;
        }

        while (_seeks.IsEmpty && GetBufferedDuration() < Config.Player.MinBufferDuration && IsPlaying && VideoDemuxer.IsRunning && VideoDemuxer.Status != ThreadStatus.QueueFull)
        {
            Thread.Sleep(20);
        }

        if (!_seeks.IsEmpty)
        {
            return false;
        }

        if (CanInfo)
        {
            Log.Info($"Started [V: {TicksToTime(_videoFrame.Timestamp)}]" + (AudioFrame == null ? string.Empty : $" [A: {TicksToTime(AudioFrame.Timestamp)}]"));
        }

        Decoder.OpenedPlugin.OnBufferingCompleted();

        return true;
    }

    private void Screamer()
    {
        while (Status == PlayerStatus.Playing)
        {
            if (_seeks.TryPop(out var seekData))
            {
                _seeks.Clear();
                RequiresBuffering = true;

                Decoder.PauseDecoders(); // TBR: Required to avoid getting packets between Seek and ShowFrame which causes resync issues

                if (Decoder.Seek(seekData.Ms, seekData.Forward, !seekData.Accurate) < 0) // Consider using GetVideoFrame with no Timestamp (any) to ensure keyframe packet for faster seek in HEVC
                {
                    Log.Warn("Seek failed");
                }
                else if (seekData.Accurate)
                {
                    Decoder.GetVideoFrame(seekData.Ms * 10000L);
                }
            }

            if (RequiresBuffering)
            {
                OnBufferingStarted();
                MediaBuffer();
                RequiresBuffering = false;
                if (!_seeks.IsEmpty)
                {
                    continue;
                }

                if (_videoFrame == null)
                {
                    if (DecoderHasEnded)
                    {
                        OnBufferingCompleted();
                    }

                    Log.Warn("[MediaBuffer] No video frame");
                    break;
                }

                // Temp fix to ensure we had enough time to decode one more frame
                var retries = 5;
                while (VideoDecoder.Frames.Count == 0 && retries-- > 0)
                {
                    Thread.Sleep(10);
                }

                OnBufferingCompleted();

                _allowedLateAudioDrops = 7;
                _elapsedSec = 0;
                _startTicks = _videoFrame.Timestamp;
                _sw.Restart();
            }

            if (Status != PlayerStatus.Playing)
            {
                break;
            }

            if (_videoFrame == null)
            {
                if (VideoDecoder.Status == ThreadStatus.Ended)
                {
                    break;
                }

                Log.Warn("No video frames");
                RequiresBuffering = true;
                continue;
            }

            if (AudioFrame == null && !_isAudioSwitch)
            {
                AudioDecoder.Frames.TryDequeue(out var audioFrame);
                AudioFrame = audioFrame;
            }

            if (SubtitleFrame == null && !_isSubsSwitch)
            {
                SubtitlesDecoder.Frames.TryPeek(out var subtitleFrame);
                SubtitleFrame = subtitleFrame;
            }

            _elapsedTicks = (long)(_sw.ElapsedTicks * SWFREQ_TO_TICKS); // Do we really need ticks precision?

            _vDistanceMs =
                  (int)((((_videoFrame.Timestamp - _startTicks) / Speed) - _elapsedTicks) / 10000);

            if (AudioFrame != null)
            {
                _curAudioDeviceDelay = Audio.GetDeviceDelay();
                _aDistanceMs = (int)((((AudioFrame.Timestamp - _startTicks) / Speed) - (_elapsedTicks - _curAudioDeviceDelay)) / 10000);
            }
            else
            {
                _aDistanceMs = int.MaxValue;
            }

            _sDistanceMs = SubtitleFrame != null
                ? (int)((((SubtitleFrame.Timestamp - _startTicks) / Speed) - _elapsedTicks) / 10000)
                : int.MaxValue;

            _sleepMs = Math.Min(_vDistanceMs, _aDistanceMs) - 1;
            if (_sleepMs < 0)
            {
                _sleepMs = 0;
            }

            if (_sleepMs > 2)
            {
                if (_vDistanceMs > 2000)
                {
                    Log.Warn($"vDistanceMs = {_vDistanceMs} (restarting)");
                    RequiresBuffering = true;
                    continue;
                }

                if (Engine.Config.UICurTimePerSecond &&
                    MainDemuxer.IsHLSLive && Math.Abs(_elapsedTicks - _elapsedSec) > 10000000)
                {
                    _elapsedSec = _elapsedTicks;
                    UI(() => UpdateCurTime());
                }

                Thread.Sleep(_sleepMs);
            }

            // Should use different thread for better accurancy (renderer might delay it on high fps) | also on high offset we will have silence between samples
            if (AudioFrame != null)
            {
                if (Math.Abs(_aDistanceMs - _sleepMs) <= 5)
                {
                    if (CanTrace)
                    {
                        Log.Trace($"[A] Presenting {TicksToTime(AudioFrame.Timestamp)}");
                    }

                    Audio.AddSamples(AudioFrame);
                    Audio.FramesDisplayed++;
                    AudioDecoder.Frames.TryDequeue(out var audioFrame);
                    AudioFrame = audioFrame;
                }
                else if (_aDistanceMs > 1000) // Drops few audio frames in case of wrong Timestamps
                {
                    if (_allowedLateAudioDrops > 0)
                    {
                        Audio.FramesDropped++;
                        _allowedLateAudioDrops--;
                        if (CanDebug)
                        {
                            Log.Debug($"aDistanceMs 3 = {_aDistanceMs}");
                        }

                        AudioDecoder.Frames.TryDequeue(out var audioFrame);
                        AudioFrame = audioFrame;
                    }
                }
                else if (_aDistanceMs < -5) // Will be transfered back to Decoder to drop invalid Timestamps
                {
                    if (CanInfo)
                    {
                        Log.Info($"aDistanceMs = {_aDistanceMs} | AudioFrames: {AudioDecoder.Frames.Count} AudioPackets: {AudioDecoder.Demuxer.AudioPackets.Count}");
                    }

                    if (GetBufferedDuration() < Config.Player.MinBufferDuration / 2)
                    {
                        if (CanInfo)
                        {
                            Log.Warn($"Not enough buffer (restarting)");
                        }

                        RequiresBuffering = true;
                        continue;
                    }

                    if (_aDistanceMs < -600)
                    {
                        if (CanTrace)
                        {
                            Log.Trace($"All audio frames disposed");
                        }

                        Audio.FramesDropped += AudioDecoder.Frames.Count;
                        AudioDecoder.DisposeFrames();
                        AudioFrame = null;
                    }
                    else
                    {
                        var maxdrop = Math.Max(Math.Min(_vDistanceMs - _sleepMs - 1, 20), 3);
                        for (var i = 0; i < maxdrop; i++)
                        {
                            if (CanTrace)
                            {
                                Log.Trace($"aDistanceMs 2 = {_aDistanceMs}");
                            }

                            Audio.FramesDropped++;
                            AudioDecoder.Frames.TryDequeue(out var audioFrame);
                            AudioFrame = audioFrame;

                            if (AudioFrame == null || ((AudioFrame.Timestamp - _startTicks) / Speed) - ((long)(_sw.ElapsedTicks * SWFREQ_TO_TICKS) - Audio.GetDeviceDelay() + (8 * 1000)) > 0)
                            {
                                break;
                            }

                            AudioFrame = null;
                        }
                    }
                }
            }

            if (Math.Abs(_vDistanceMs - _sleepMs) <= 2)
            {
                if (CanTrace)
                {
                    Log.Trace($"[V] Presenting {TicksToTime(_videoFrame.Timestamp)}");
                }

                if (Decoder.VideoDecoder.Renderer.Present(_videoFrame))
                {
                    Video.FramesDisplayed++;
                }
                else
                {
                    Video.FramesDropped++;
                }

                lock (_seeks)
                {
                    if (_seeks.IsEmpty)
                    {
                        CurTime = !MainDemuxer.IsHLSLive ? _videoFrame.Timestamp : VideoDemuxer.CurTime;

                        if (Config.Player.UICurTimePerFrame)
                        {
                            UI(() => UpdateCurTime());
                        }
                    }
                }

                VideoDecoder.Frames.TryDequeue(out _videoFrame);
                if (_videoFrame != null && Config.Player.MaxLatency != 0)
                {
                    CheckLatency();
                }
            }
            else if (_vDistanceMs < -2)
            {
                if (_vDistanceMs < -10 || GetBufferedDuration() < Config.Player.MinBufferDuration / 2)
                {
                    if (CanInfo)
                    {
                        Log.Info($"vDistanceMs = {_vDistanceMs} (restarting)");
                    }

                    RequiresBuffering = true;
                    continue;
                }

                if (CanDebug)
                {
                    Log.Debug($"vDistanceMs = {_vDistanceMs}");
                }

                Video.FramesDropped++;
                MediaFramework.MediaDecoder.VideoDecoder.DisposeFrame(_videoFrame);
                VideoDecoder.Frames.TryDequeue(out _videoFrame);
            }

            if (SubtitleFramePrev != null && ((SubtitleFramePrev.Timestamp - _startTicks + (SubtitleFramePrev.Duration * 10000L)) / Speed) - (long)(_sw.ElapsedTicks * SWFREQ_TO_TICKS) < 0)
            {
                Subtitles.SubtitleText = string.Empty;
                UI(() => Subtitles.SubtitleText = Subtitles.SubtitleText);

                SubtitleFramePrev = null;
            }

            if (SubtitleFrame != null)
            {
                if (Math.Abs(_sDistanceMs - _sleepMs) < 30 || (_sDistanceMs < -30 && SubtitleFrame.Duration + _sDistanceMs > 0))
                {
                    UI(() => Subtitles.SubtitleText = SubtitleFrame.Text);

                    SubtitleFramePrev = SubtitleFrame;
                    SubtitleFrame = null;
                    SubtitlesDecoder.Frames.TryDequeue(out var devnull);
                }
                else if (_sDistanceMs < -30)
                {
                    if (CanDebug)
                    {
                        Log.Debug($"sDistanceMs = {_sDistanceMs}");
                    }

                    SubtitleFrame = null;
                    SubtitlesDecoder.Frames.TryDequeue(out var devnull);
                }
            }
        }

        if (CanInfo)
        {
            Log.Info($"Finished -> {TicksToTime(CurTime)}");
        }
    }

    private void CheckLatency()
    {
        _curLatency = GetBufferedDuration();

        if (CanDebug)
        {
            Log.Debug($"[Latency {_curLatency / 10000}ms] Frames: {VideoDecoder.Frames.Count} Packets: {VideoDemuxer.VideoPackets.Count} Speed: {Speed}");
        }

        if (_curLatency < 1 || VideoDemuxer.VideoPackets.Count < 1) // No buffer
        {
            ChangeSpeedWithoutBuffering(1);
            return;
        }
        else if (_curLatency <= Config.Player.MinLatency) // We've reached the down limit (back to speed x1)
        {
            ChangeSpeedWithoutBuffering(1);
            return;
        }
        else if (_curLatency < Config.Player.MaxLatency)
        {
            return;
        }

        var newSpeed = Math.Max(Math.Round((double)_curLatency / Config.Player.MaxLatency, 1, MidpointRounding.ToPositiveInfinity), 1.1);

        if (newSpeed > 4) // TBR: dispose only as much as required to avoid rebuffering
        {
            Decoder.Flush();
            RequiresBuffering = true;
            Log.Debug($"[Latency {_curLatency / 10000}ms] Clearing queue");
            return;
        }

        ChangeSpeedWithoutBuffering(newSpeed);
    }

    private void ChangeSpeedWithoutBuffering(double newSpeed)
    {
        if (Speed == newSpeed)
        {
            return;
        }

        var curTicks = DateTime.UtcNow.Ticks;

        if (newSpeed != 1 && curTicks - _lastSpeedChangeTicks < Config.Player.LatencySpeedChangeInterval)
        {
            return;
        }

        _lastSpeedChangeTicks = curTicks;

        if (CanDebug)
        {
            Log.Debug($"[Latency {_curLatency / 10000}ms] Speed changed x{Speed} -> x{newSpeed}");
        }

        if (AudioFrame != null)
        {
            AudioDecoder.FixSample(AudioFrame, Speed, newSpeed);
        }

        Speed = newSpeed;
        RequiresBuffering
                    = false;
        _startTicks = CurTime;
        _elapsedSec = 0;
        _sw.Restart();
    }

    private long GetBufferedDuration()
    {
        if (VideoDecoder.Frames.IsEmpty)
        {
            return 0;
        }

        var decoder = VideoDecoder.Frames.ToArray()[^1].Timestamp - _videoFrame.Timestamp;
        var demuxer = VideoDemuxer.VideoPackets.LastTimestamp == ffmpeg.AV_NOPTS_VALUE
            ? 0 :
            VideoDemuxer.VideoPackets.LastTimestamp - VideoDemuxer.StartTime - _videoFrame.Timestamp;

        return Math.Max(decoder, demuxer);
    }

    private bool AudioBuffer()
    {
        while ((_isVideoSwitch || _isAudioSwitch) && IsPlaying)
        {
            Thread.Sleep(10);
        }

        if (!IsPlaying)
        {
            return false;
        }

        AudioFrame = null;
        Audio.ClearBuffer();
        Decoder.AudioStream.Demuxer.Start();
        AudioDecoder.Start();

        while (AudioDecoder.Frames.IsEmpty && IsPlaying && AudioDecoder.IsRunning)
        {
            Thread.Sleep(10);
        }

        AudioDecoder.Frames.TryDequeue(out var audioFrame);
        AudioFrame = audioFrame;
        if (AudioFrame == null)
        {
            return false;
        }

        lock (_seeks)
        {
            if (_seeks.IsEmpty)
            {
                if (MainDemuxer.IsHLSLive)
                {
                    CurTime = AudioFrame.Timestamp;
                }

                UI(() => UpdateCurTime());
            }
        }

        while (_seeks.IsEmpty && Decoder.AudioStream.Demuxer.BufferedDuration < Config.Player.MinBufferDuration && IsPlaying && Decoder.AudioStream.Demuxer.IsRunning && Decoder.AudioStream.Demuxer.Status != ThreadStatus.QueueFull)
        {
            Thread.Sleep(20);
        }

        return IsPlaying && !AudioDecoder.Frames.IsEmpty && _seeks.IsEmpty;
    }

    private void ScreamerAudioOnly()
    {
        while (IsPlaying)
        {
            if (_seeks.TryPop(out var seekData))
            {
                _seeks.Clear();
                RequiresBuffering = true;

                if (AudioDecoder.OnVideoDemuxer)
                {
                    if (Decoder.Seek(seekData.Ms, seekData.Forward) < 0)
                    {
                        Log.Warn("Seek failed 1");
                    }
                }
                else
                {
                    if (Decoder.SeekAudio(seekData.Ms, seekData.Forward) < 0)
                    {
                        Log.Warn("Seek failed 2");
                    }
                }
            }

            if (RequiresBuffering)
            {
                OnBufferingStarted();
                AudioBuffer();
                RequiresBuffering = false;
                if (!_seeks.IsEmpty)
                {
                    continue;
                }

                OnBufferingCompleted();
                if (AudioFrame == null)
                {
                    Log.Warn("[MediaBuffer] No audio frame");
                    break;
                }

                _startTicks = (long)(AudioFrame.Timestamp / Speed);

                Audio.AddSamples(AudioFrame);
                AudioDecoder.Frames.TryDequeue(out var aFrame);
                AudioFrame = aFrame;
                if (AudioFrame != null)
                {
                    AudioFrame.Timestamp = (long)(AudioFrame.Timestamp / Speed);
                }

                _sw.Restart();
                _elapsedSec = 0;
            }

            if (AudioFrame == null)
            {
                if (AudioDecoder.Status == ThreadStatus.Ended)
                {
                    break;
                }

                Log.Warn("No audio frames");
                RequiresBuffering = true;
                continue;
            }

            if (Status != PlayerStatus.Playing)
            {
                break;
            }

            _elapsedTicks = _startTicks + (long)(_sw.ElapsedTicks * SWFREQ_TO_TICKS);
            _aDistanceMs = (int)((AudioFrame.Timestamp - _elapsedTicks) / 10000);

            if (_aDistanceMs > 1000 || _aDistanceMs < -10)
            {
                RequiresBuffering = true;
                continue;
            }

            if (_aDistanceMs > 2)
            {
                if (Engine.Config.UICurTimePerSecond &&
                    MainDemuxer.IsHLSLive && Math.Abs(_elapsedTicks - _elapsedSec) > 10000000)
                {
                    _elapsedSec = _elapsedTicks;
                    UI(() => UpdateCurTime());
                }

                Thread.Sleep(_aDistanceMs);
            }

            lock (_seeks)
            {
                if (!MainDemuxer.IsHLSLive && _seeks.IsEmpty)
                {
                    CurTime = (long)(AudioFrame.Timestamp * Speed);

                    if (Config.Player.UICurTimePerFrame)
                    {
                        UI(() => UpdateCurTime());
                    }
                }
            }

            Audio.AddSamples(AudioFrame);
            AudioDecoder.Frames.TryDequeue(out var audioFrame);
            AudioFrame = audioFrame;
            if (AudioFrame != null)
            {
                AudioFrame.Timestamp = (long)(AudioFrame.Timestamp / Speed);
            }
        }
    }

    private void ScreamerReverse()
    {
        while (Status == PlayerStatus.Playing)
        {
            if (_seeks.TryPop(out var seekData))
            {
                _seeks.Clear();
                if (Decoder.Seek(seekData.Ms, seekData.Forward) < 0)
                {
                    Log.Warn("Seek failed");
                }
            }

            if (_videoFrame == null)
            {
                if (VideoDecoder.Status == ThreadStatus.Ended)
                {
                    break;
                }

                OnBufferingStarted();
                if (_reversePlaybackResync)
                {
                    Decoder.Flush();
                    VideoDemuxer.EnableReversePlayback(CurTime);
                    _reversePlaybackResync = false;
                }

                VideoDemuxer.Start();
                VideoDecoder.Start();

                while (VideoDecoder.Frames.IsEmpty && Status == PlayerStatus.Playing && VideoDecoder.IsRunning)
                {
                    Thread.Sleep(15);
                }

                OnBufferingCompleted();
                VideoDecoder.Frames.TryDequeue(out _videoFrame);
                if (_videoFrame == null)
                {
                    Log.Warn("No video frame");
                    break;
                }

                _videoFrame.Timestamp = (long)(_videoFrame.Timestamp / Speed);

                _startTicks = _videoFrame.Timestamp;
                _sw.Restart();
                _elapsedSec = 0;

                if (!MainDemuxer.IsHLSLive && _seeks.IsEmpty)
                {
                    CurTime = (long)(_videoFrame.Timestamp * Speed);
                }

                UI(() => UpdateCurTime());
            }

            _elapsedTicks = _startTicks - (long)(_sw.ElapsedTicks * SWFREQ_TO_TICKS);
            _vDistanceMs = (int)((_elapsedTicks - _videoFrame.Timestamp) / 10000);
            _sleepMs = _vDistanceMs - 1;

            if (_sleepMs < 0)
            {
                _sleepMs = 0;
            }

            if (Math.Abs(_vDistanceMs - _sleepMs) > 5)
            {
                MediaFramework.MediaDecoder.VideoDecoder.DisposeFrame(_videoFrame);
                _videoFrame = null;
                Thread.Sleep(5);
                continue;
            }

            if (_sleepMs > 2)
            {
                if (_sleepMs > 1000)
                {
                    MediaFramework.MediaDecoder.VideoDecoder.DisposeFrame(_videoFrame);
                    _videoFrame = null;
                    Thread.Sleep(5);
                    continue; // rebuffer
                }

                // Every seconds informs the application with CurTime / Bitrates (invokes UI thread to ensure the updates will actually happen)
                if (Engine.Config.UICurTimePerSecond &&
                    MainDemuxer.IsHLSLive && Math.Abs(_elapsedTicks - _elapsedSec) > 10000000)
                {
                    _elapsedSec = _elapsedTicks;
                    UI(() => UpdateCurTime());
                }

                Thread.Sleep(_sleepMs);
            }

            Decoder.VideoDecoder.Renderer.Present(_videoFrame);
            if (!MainDemuxer.IsHLSLive && _seeks.IsEmpty)
            {
                CurTime = (long)(_videoFrame.Timestamp * Speed);

                if (Config.Player.UICurTimePerFrame)
                {
                    UI(UpdateCurTime);
                }
            }

            VideoDecoder.Frames.TryDequeue(out _videoFrame);
            if (_videoFrame != null)
            {
                _videoFrame.Timestamp = (long)(_videoFrame.Timestamp / Speed);
            }
        }
    }
}
