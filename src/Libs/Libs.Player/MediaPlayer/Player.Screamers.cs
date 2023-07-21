using System;
using System.Diagnostics;
using System.Threading;

using FFmpeg.AutoGen;

using Bili.Copilot.Libs.Player.MediaFramework.MediaDecoder;

using static FlyleafLib.Utils;
using static FlyleafLib.Logger;

namespace Bili.Copilot.Libs.Player.MediaPlayer;

unsafe partial class Player
{
    /// <summary>
    /// Fires on buffering started
    /// Warning: Uses Invoke and it comes from playback thread so you can't pause/stop etc. You need to use another thread if you have to.
    /// </summary>
    public event EventHandler BufferingStarted;
    protected virtual void OnBufferingStarted()
    {
        if (onBufferingStarted != onBufferingCompleted) return;
        BufferingStarted?.Invoke(this, new EventArgs()); 
        onBufferingStarted++;

        if (CanDebug) Log.Debug($"OnBufferingStarted");
    }

    /// <summary>
    /// Fires on buffering completed (will fire also on failed buffering completed)
    /// (BufferDration > Config.Player.MinBufferDuration)
    /// Warning: Uses Invoke and it comes from playback thread so you can't pause/stop etc. You need to use another thread if you have to.
    /// </summary>
    public event EventHandler<BufferingCompletedArgs> BufferingCompleted;
    protected virtual void OnBufferingCompleted(string error = null)
    {
        if (onBufferingStarted - 1 != onBufferingCompleted) return;

        if (error != null && LastError == null)
        {
            lastError = error;
            UI(() => LastError = LastError);
        }

        BufferingCompleted?.Invoke(this, new BufferingCompletedArgs(error));
        onBufferingCompleted++;
        if (CanDebug) Log.Debug($"OnBufferingCompleted{(error != null ? $" (Error: {error})" : "")}");
    }

    long    onBufferingStarted;
    long    onBufferingCompleted;

    int     vDistanceMs;
    int     aDistanceMs;
    int     sDistanceMs;
    int     sleepMs;
        
    long    elapsedTicks;
    long    elapsedSec;
    long    startTicks;

    int     allowedLateAudioDrops;
    long    lastSpeedChangeTicks;
    long    curLatency;
    internal long curAudioDeviceDelay;

    Stopwatch sw = new();

    private void ShowOneFrame()
    {
        sFrame = null;
        Subtitles._subtitleText = "";
        if (Subtitles._subtitleText != "")
            UIAdd(() => Subtitles.SubtitleText = Subtitles.SubtitleText);

        if (!VideoDecoder.Frames.IsEmpty)
        {
            VideoDecoder.Frames.TryDequeue(out vFrame);
            if (vFrame != null) // might come from video input switch interrupt
                renderer.Present(vFrame);

            if (_seeks.IsEmpty)
            {
                if (!VideoDemuxer.IsHLSLive)
                    curTime = vFrame.timestamp;
                UIAdd(() => UpdateCurTime());
                UIAll();
            }

            // Required for buffering on paused
            if (decoder.RequiresResync && !IsPlaying && _seeks.IsEmpty)
                decoder.Resync(vFrame.timestamp);

            vFrame = null;
        }

        UIAll();
    }

    // !!! NEEDS RECODING (We show one frame, we dispose it, we get another one and we show it also after buffering which can be in 'no time' which can leave us without any more decoded frames so we rebuffer)
    private bool MediaBuffer()
    {
        if (CanTrace) Log.Trace("Buffering");

        while (isVideoSwitch && IsPlaying) Thread.Sleep(10);

        Audio.ClearBuffer();

        VideoDemuxer.Start();
        VideoDecoder.Start();

        if (Audio.isOpened && Config.Audio.Enabled)
        {
            curAudioDeviceDelay = Audio.GetDeviceDelay();

            if (AudioDecoder.OnVideoDemuxer)
                AudioDecoder.Start();
            else if (!decoder.RequiresResync)
            {
                AudioDemuxer.Start();
                AudioDecoder.Start();
            }
        }

        if (Subtitles.isOpened && Config.Subtitles.Enabled)
        {
            lock (lockSubtitles)
            if (SubtitlesDecoder.OnVideoDemuxer)
                SubtitlesDecoder.Start();
            else if (!decoder.RequiresResync)
            {
                SubtitlesDemuxer.Start();
                SubtitlesDecoder.Start();
            }
        }

        VideoDecoder.DisposeFrame(vFrame);
        vFrame = null;
        aFrame = null;
        sFrame = null;
        sFramePrev = null;

        bool gotAudio       = !Audio.IsOpened || Config.Player.MaxLatency != 0;
        bool gotVideo       = false;
        bool shouldStop     = false;
        bool showOneFrame   = true;
        int  audioRetries   = 4;
        int  loops          = 0;

        if (Config.Player.MaxLatency != 0)
        {
            lastSpeedChangeTicks = DateTime.UtcNow.Ticks;
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
                return false;

            if (!gotVideo && !showOneFrame && !VideoDecoder.Frames.IsEmpty)
            {
                VideoDecoder.Frames.TryDequeue(out vFrame);
                if (vFrame != null) gotVideo = true;
            }

            if (!gotAudio && aFrame == null && !AudioDecoder.Frames.IsEmpty)
                AudioDecoder.Frames.TryDequeue(out aFrame);

            if (gotVideo)
            {
                if (decoder.RequiresResync)
                    decoder.Resync(vFrame.timestamp);

                if (!gotAudio && aFrame != null)
                {
                    for (int i=0; i<Math.Min(50, AudioDecoder.Frames.Count); i++)
                    {
                        if (aFrame == null 
                            || aFrame.timestamp - curAudioDeviceDelay > vFrame.timestamp 
                            || vFrame.timestamp > Duration)
                        {
                            gotAudio = true;
                            break;
                        }

                        if (CanInfo) Log.Info($"Drop aFrame {TicksToTime(aFrame.timestamp)}");
                        AudioDecoder.Frames.TryDequeue(out aFrame);
                    }

                    // Avoid infinite loop in case of all audio timestamps wrong
                    if (!gotAudio)
                    {
                        audioRetries--;

                        if (audioRetries < 1)
                        {
                            gotAudio = true;
                            aFrame = null;
                            Log.Warn($"Audio Exhausted 1");
                        }
                    }
                }
            }

            if (!IsPlaying || decoderHasEnded)
                shouldStop = true;
            else
            {
                if (!VideoDecoder.IsRunning && !isVideoSwitch)
                {
                    Log.Warn("Video Exhausted");
                    shouldStop= true;
                }

                if (gotVideo && !gotAudio && audioRetries > 0 && (!AudioDecoder.IsRunning || AudioDecoder.Demuxer.Status == MediaFramework.Status.QueueFull))
                {
                    if (CanWarn) Log.Warn($"Audio Exhausted 2 | {audioRetries}");

                    audioRetries--;

                    if (audioRetries < 1)
                        gotAudio  = true;
                }
            }

            Thread.Sleep(10);

        } while (!shouldStop && (!gotVideo || !gotAudio));

        if (shouldStop && !(decoderHasEnded && IsPlaying && vFrame != null))
        {
            Log.Info("Stopped");
            return false;
        }

        if (vFrame == null)
        {
            Log.Error("No Frames!");
            return false;
        }

        while(_seeks.IsEmpty && GetBufferedDuration() < Config.Player.MinBufferDuration && IsPlaying && VideoDemuxer.IsRunning && VideoDemuxer.Status != MediaFramework.Status.QueueFull) Thread.Sleep(20);

        if (!_seeks.IsEmpty)
            return false;

        if (CanInfo) Log.Info($"Started [V: {TicksToTime(vFrame.timestamp)}]" + (aFrame == null ? "" : $" [A: {TicksToTime(aFrame.timestamp)}]"));

        decoder.OpenedPlugin.OnBufferingCompleted();

        return true;
    }
    private void Screamer()
    {
        while (Status == Status.Playing)
        {
            if (_seeks.TryPop(out var seekData))
            {
                _seeks.Clear();
                requiresBuffering = true;

                decoder.PauseDecoders(); // TBR: Required to avoid getting packets between Seek and ShowFrame which causes resync issues

                if (decoder.Seek(seekData.ms, seekData.forward, !seekData.accurate) < 0) // Consider using GetVideoFrame with no timestamp (any) to ensure keyframe packet for faster seek in HEVC
                    Log.Warn("Seek failed");
                else if (seekData.accurate)
                    decoder.GetVideoFrame(seekData.ms * (long)10000);
            }
            
            if (requiresBuffering)
            {
                OnBufferingStarted();
                MediaBuffer();
                requiresBuffering = false;
                if (!_seeks.IsEmpty)
                    continue;

                if (vFrame == null)
                {
                    if (decoderHasEnded)
                        OnBufferingCompleted();

                    Log.Warn("[MediaBuffer] No video frame");
                    break;
                }

                // Temp fix to ensure we had enough time to decode one more frame
                int retries = 5;
                while (VideoDecoder.Frames.Count == 0 && retries-- > 0)
                    Thread.Sleep(10);
                
                OnBufferingCompleted();

                allowedLateAudioDrops = 7;
                elapsedSec = 0;
                startTicks = vFrame.timestamp;
                sw.Restart();
            }

            if (Status != Status.Playing)
                break;

            if (vFrame == null)
            {
                if (VideoDecoder.Status == MediaFramework.Status.Ended)
                    break;

                Log.Warn("No video frames");
                requiresBuffering = true;
                continue;
            }

            if (aFrame == null && !isAudioSwitch)
                AudioDecoder.Frames.TryDequeue(out aFrame);

            if (sFrame == null && !isSubsSwitch )
                SubtitlesDecoder.Frames.TryPeek(out sFrame);

            elapsedTicks = (long) (sw.ElapsedTicks * SWFREQ_TO_TICKS); // Do we really need ticks precision?

            vDistanceMs = 
                  (int) ((((vFrame.timestamp - startTicks) / speed) - elapsedTicks) / 10000);

            if (aFrame != null)
            {
                curAudioDeviceDelay = Audio.GetDeviceDelay();
                aDistanceMs = (int) ((((aFrame.timestamp - startTicks) / speed) - (elapsedTicks - curAudioDeviceDelay)) / 10000);
            }
            else
                aDistanceMs = int.MaxValue;
            
            sDistanceMs = sFrame != null 
                ? (int) ((((sFrame.timestamp - startTicks) / speed) - elapsedTicks) / 10000) 
                : int.MaxValue;

            sleepMs = Math.Min(vDistanceMs, aDistanceMs) - 1;
            if (sleepMs < 0)
                sleepMs = 0;

            if (sleepMs > 2)
            {
                if (vDistanceMs > 2000)
                {
                    Log.Warn($"vDistanceMs = {vDistanceMs} (restarting)");
                    requiresBuffering = true;
                    continue;
                }

                if (Engine.Config.UICurTimePerSecond &&  (
                    (!MainDemuxer.IsHLSLive && curTime / 10000000 != _CurTime / 10000000) ||
                    (MainDemuxer.IsHLSLive && Math.Abs(elapsedTicks - elapsedSec) > 10000000)))
                {
                    elapsedSec  = elapsedTicks;
                    UI(() => UpdateCurTime());
                }

                Thread.Sleep(sleepMs);
            }

            // Should use different thread for better accurancy (renderer might delay it on high fps) | also on high offset we will have silence between samples
            if (aFrame != null)
            {
                if (Math.Abs(aDistanceMs - sleepMs) <= 5)
                {
                    if (CanTrace) Log.Trace($"[A] Presenting {TicksToTime(aFrame.timestamp)}");
                    Audio.AddSamples(aFrame);
                    Audio._framesDisplayed++;
                    AudioDecoder.Frames.TryDequeue(out aFrame);
                }
                else if (aDistanceMs > 1000) // Drops few audio frames in case of wrong timestamps
                {
                    if (allowedLateAudioDrops > 0)
                    {
                        Audio._framesDropped++;
                        allowedLateAudioDrops--;
                        if (CanDebug) Log.Debug($"aDistanceMs 3 = {aDistanceMs}");
                        AudioDecoder.Frames.TryDequeue(out aFrame);
                    }
                }
                else if (aDistanceMs < -5) // Will be transfered back to decoder to drop invalid timestamps
                {
                    if (CanInfo) Log.Info($"aDistanceMs = {aDistanceMs} | AudioFrames: {AudioDecoder.Frames.Count} AudioPackets: {AudioDecoder.Demuxer.AudioPackets.Count}");

                    if (GetBufferedDuration() < Config.Player.MinBufferDuration / 2)
                    {
                        if (CanInfo)
                            Log.Warn($"Not enough buffer (restarting)");

                        requiresBuffering = true;
                        continue;
                    }

                    if (aDistanceMs < -600)
                    {
                        if (CanTrace) Log.Trace($"All audio frames disposed");
                        Audio._framesDropped += AudioDecoder.Frames.Count;
                        AudioDecoder.DisposeFrames();
                        aFrame = null;
                    }
                    else
                    {
                        int maxdrop = Math.Max(Math.Min(vDistanceMs - sleepMs - 1, 20), 3);
                        for (int i=0; i<maxdrop; i++)
                        {
                            if (CanTrace) Log.Trace($"aDistanceMs 2 = {aDistanceMs}");
                            Audio._framesDropped++;
                            AudioDecoder.Frames.TryDequeue(out aFrame);

                            if (aFrame == null || ((aFrame.timestamp - startTicks) / speed) - ((long) (sw.ElapsedTicks * SWFREQ_TO_TICKS) - Audio.GetDeviceDelay() + 8 * 1000) > 0)
                                break;

                            aFrame = null;
                        }
                    }
                }
            }

            if (Math.Abs(vDistanceMs - sleepMs) <= 2)
            {
                if (CanTrace) Log.Trace($"[V] Presenting {TicksToTime(vFrame.timestamp)}");

                if (decoder.VideoDecoder.Renderer.Present(vFrame))
                    Video.framesDisplayed++;
                else
                    Video.framesDropped++;

                lock (_seeks)
                    if (_seeks.IsEmpty)
                    {
                        curTime = !MainDemuxer.IsHLSLive ? vFrame.timestamp : VideoDemuxer.CurTime;

                        if (Config.Player.UICurTimePerFrame)
                            UI(() => UpdateCurTime());
                    }

                VideoDecoder.Frames.TryDequeue(out vFrame);
                if (vFrame != null && Config.Player.MaxLatency != 0)
                    CheckLatency();
            }
            else if (vDistanceMs < -2)
            {
                if (vDistanceMs < -10 || GetBufferedDuration() < Config.Player.MinBufferDuration / 2)
                {
                    if (CanInfo)
                        Log.Info($"vDistanceMs = {vDistanceMs} (restarting)");

                    requiresBuffering = true;
                    continue;
                }

                if (CanDebug)
                    Log.Debug($"vDistanceMs = {vDistanceMs}");

                Video.framesDropped++;
                VideoDecoder.DisposeFrame(vFrame);
                VideoDecoder.Frames.TryDequeue(out vFrame);
            }

            if (sFramePrev != null && ((sFramePrev.timestamp - startTicks + (sFramePrev.duration * (long)10000)) / speed) - (long) (sw.ElapsedTicks * SWFREQ_TO_TICKS) < 0)
            {
                Subtitles._subtitleText = "";
                UI(() => Subtitles.SubtitleText = Subtitles.SubtitleText);

                sFramePrev = null;
            }

            if (sFrame != null)
            {
                if (Math.Abs(sDistanceMs - sleepMs) < 30 || (sDistanceMs < -30 && sFrame.duration + sDistanceMs > 0))
                {
                    Subtitles._subtitleText = sFrame.text;
                    UI(() => Subtitles.SubtitleText = Subtitles.SubtitleText);
                    
                    sFramePrev = sFrame;
                    sFrame = null;
                    SubtitlesDecoder.Frames.TryDequeue(out var devnull);
                }
                else if (sDistanceMs < -30)
                {
                    if (CanDebug) Log.Debug($"sDistanceMs = {sDistanceMs}");

                    sFrame = null;
                    SubtitlesDecoder.Frames.TryDequeue(out var devnull);
                }
            }
        }

        if (CanInfo) Log.Info($"Finished -> {TicksToTime(CurTime)}");
    }

    private void CheckLatency()
    {
        curLatency = GetBufferedDuration();

        if (CanDebug)
            Log.Debug($"[Latency {curLatency/10000}ms] Frames: {VideoDecoder.Frames.Count} Packets: {VideoDemuxer.VideoPackets.Count} Speed: {speed}");

        if (curLatency < 1 || VideoDemuxer.VideoPackets.Count < 1) // No buffer
        {
            ChangeSpeedWithoutBuffering(1);
            return;
        }
        else if (curLatency <= Config.Player.MinLatency) // We've reached the down limit (back to speed x1)
        {
            ChangeSpeedWithoutBuffering(1);
            return;
        }
        else if (curLatency < Config.Player.MaxLatency)
            return;

        #if NET5_0_OR_GREATER
        var newSpeed = Math.Max(Math.Round((double)curLatency / Config.Player.MaxLatency, 1, MidpointRounding.ToPositiveInfinity), 1.1);
        #else
        var newSpeed = Math.Max(Math.Round((double)curLatency / (curLatency - Config.Player.MinLatency), 1), 1.1);
        #endif

        if (newSpeed > 4) // TBR: dispose only as much as required to avoid rebuffering
        {
            decoder.Flush();
            requiresBuffering = true;
            Log.Debug($"[Latency {curLatency/10000}ms] Clearing queue");
            return;
        }

        ChangeSpeedWithoutBuffering(newSpeed);
    }
    private void ChangeSpeedWithoutBuffering(double newSpeed)
    {
        if (speed == newSpeed)
            return;

        long curTicks = DateTime.UtcNow.Ticks;

        if (newSpeed != 1 && curTicks - lastSpeedChangeTicks < Config.Player.LatencySpeedChangeInterval)
            return;

        lastSpeedChangeTicks = curTicks;

        if (CanDebug)
            Log.Debug($"[Latency {curLatency/10000}ms] Speed changed x{speed} -> x{newSpeed}");

        if (aFrame != null)
            AudioDecoder.FixSample(aFrame, speed, newSpeed);

        Speed       = newSpeed;
        requiresBuffering
                    = false;
        startTicks  = curTime;
        elapsedSec  = 0;
        sw.Restart();
    }
    private long GetBufferedDuration()
    {
        if (VideoDecoder.Frames.IsEmpty)
            return 0;

        var decoder = VideoDecoder.Frames.ToArray()[^1].Timestamp - vFrame.timestamp;
        var demuxer = VideoDemuxer.VideoPackets.LastTimestamp == ffmpeg.AV_NOPTS_VALUE
            ? 0 : 
            (VideoDemuxer.VideoPackets.LastTimestamp - VideoDemuxer.StartTime) - vFrame.timestamp;

        return Math.Max(decoder, demuxer);
    }

    private bool AudioBuffer()
    {
        while ((isVideoSwitch || isAudioSwitch) && IsPlaying) Thread.Sleep(10);
        if (!IsPlaying) return false;

        aFrame = null;
        Audio.ClearBuffer();
        decoder.AudioStream.Demuxer.Start();
        AudioDecoder.Start();

        while(AudioDecoder.Frames.IsEmpty && IsPlaying && AudioDecoder.IsRunning) Thread.Sleep(10);
        AudioDecoder.Frames.TryDequeue(out aFrame);
        if (aFrame == null) 
            return false;

        lock (_seeks)
            if (_seeks.IsEmpty)
            {
                if (MainDemuxer.IsHLSLive)
                    curTime = aFrame.timestamp;
                UI(() => UpdateCurTime());
            }

        while(_seeks.IsEmpty && decoder.AudioStream.Demuxer.BufferedDuration < Config.Player.MinBufferDuration && IsPlaying && decoder.AudioStream.Demuxer.IsRunning && decoder.AudioStream.Demuxer.Status != MediaFramework.Status.QueueFull)
            Thread.Sleep(20);

        return IsPlaying && !AudioDecoder.Frames.IsEmpty && _seeks.IsEmpty;
    }
    private void ScreamerAudioOnly()
    {
        while (IsPlaying)
        {
            if (_seeks.TryPop(out var seekData))
            {
                _seeks.Clear();
                requiresBuffering = true;
                
                if (AudioDecoder.OnVideoDemuxer)
                {
                    if (decoder.Seek(seekData.ms, seekData.forward) < 0)
                        Log.Warn("Seek failed 1");
                }
                else
                {
                    if (decoder.SeekAudio(seekData.ms, seekData.forward) < 0)
                        Log.Warn("Seek failed 2");
                }
            }

            if (requiresBuffering)
            {
                OnBufferingStarted();
                AudioBuffer();
                requiresBuffering = false;
                if (!_seeks.IsEmpty)
                    continue;
                OnBufferingCompleted();
                if (aFrame == null) { Log.Warn("[MediaBuffer] No audio frame"); break; }
                startTicks = (long) (aFrame.timestamp / Speed);

                Audio.AddSamples(aFrame);
                AudioDecoder.Frames.TryDequeue(out aFrame);
                if (aFrame != null)
                    aFrame.timestamp = (long)(aFrame.timestamp / Speed);
                sw.Restart();
                elapsedSec = 0;
            }

            if (aFrame == null)
            {
                if (AudioDecoder.Status == MediaFramework.Status.Ended)
                    break;

                Log.Warn("No audio frames");
                requiresBuffering = true;
                continue;
            }

            if (Status != Status.Playing) break;

            elapsedTicks = startTicks + (long) (sw.ElapsedTicks * SWFREQ_TO_TICKS);
            aDistanceMs  = (int) ((aFrame.timestamp - elapsedTicks) / 10000);

            if (aDistanceMs > 1000 || aDistanceMs < -10)
            {
                requiresBuffering = true;
                continue;
            }

            if (aDistanceMs > 2)
            {
                if (Engine.Config.UICurTimePerSecond && (
                    (!MainDemuxer.IsHLSLive && curTime / 10000000 != _CurTime / 10000000) ||
                    (MainDemuxer.IsHLSLive && Math.Abs(elapsedTicks - elapsedSec) > 10000000)))
                {
                    elapsedSec = elapsedTicks;
                    UI(() => UpdateCurTime());
                }

                Thread.Sleep(aDistanceMs);
            }

            lock (_seeks)
                if (!MainDemuxer.IsHLSLive && _seeks.IsEmpty)
                {
                    curTime = (long) (aFrame.timestamp * Speed);

                    if (Config.Player.UICurTimePerFrame)
                        UI(() => UpdateCurTime());
                }

            //Log($"{Utils.TicksToTime(aFrame.timestamp)}");
            Audio.AddSamples(aFrame);
            AudioDecoder.Frames.TryDequeue(out aFrame);
            if (aFrame != null)
                aFrame.timestamp = (long)(aFrame.timestamp / Speed);
        }
    }

    private void ScreamerReverse()
    {
        while (Status == Status.Playing)
        {
            if (_seeks.TryPop(out var seekData))
            {
                _seeks.Clear();
                if (decoder.Seek(seekData.ms, seekData.forward) < 0)
                    Log.Warn("Seek failed");
            }

            if (vFrame == null)
            {
                if (VideoDecoder.Status == MediaFramework.Status.Ended)
                    break;

                OnBufferingStarted();
                if (reversePlaybackResync)
                {
                    decoder.Flush();
                    VideoDemuxer.EnableReversePlayback(CurTime);
                    reversePlaybackResync = false;
                }
                VideoDemuxer.Start();
                VideoDecoder.Start();

                while (VideoDecoder.Frames.IsEmpty && Status == Status.Playing && VideoDecoder.IsRunning) Thread.Sleep(15);
                OnBufferingCompleted();
                VideoDecoder.Frames.TryDequeue(out vFrame);
                if (vFrame == null) { Log.Warn("No video frame"); break; }
                vFrame.timestamp = (long) (vFrame.timestamp / Speed);

                startTicks = vFrame.timestamp;
                sw.Restart();
                elapsedSec = 0;

                if (!MainDemuxer.IsHLSLive && _seeks.IsEmpty)
                    curTime = (long) (vFrame.timestamp * Speed);
                UI(() => UpdateCurTime());
            }

            elapsedTicks    = startTicks - (long) (sw.ElapsedTicks * SWFREQ_TO_TICKS);
            vDistanceMs     = (int) ((elapsedTicks - vFrame.timestamp) / 10000);
            sleepMs         = vDistanceMs - 1;

            if (sleepMs < 0) sleepMs = 0;

            if (Math.Abs(vDistanceMs - sleepMs) > 5)
            {
                //Log($"vDistanceMs |-> {vDistanceMs}");
                VideoDecoder.DisposeFrame(vFrame);
                vFrame = null;
                Thread.Sleep(5);
                continue; // rebuffer
            }

            if (sleepMs > 2)
            {
                if (sleepMs > 1000)
                {
                    //Log($"sleepMs -> {sleepMs} , vDistanceMs |-> {vDistanceMs}");
                    VideoDecoder.DisposeFrame(vFrame);
                    vFrame = null;
                    Thread.Sleep(5);
                    continue; // rebuffer
                }

                // Every seconds informs the application with CurTime / Bitrates (invokes UI thread to ensure the updates will actually happen)
                if (Engine.Config.UICurTimePerSecond && (
                    (!MainDemuxer.IsHLSLive && curTime / 10000000 != _CurTime / 10000000) || 
                    (MainDemuxer.IsHLSLive && Math.Abs(elapsedTicks - elapsedSec) > 10000000)))
                {
                    elapsedSec  = elapsedTicks;
                    UI(() => UpdateCurTime());
                }

                Thread.Sleep(sleepMs);
            }

            decoder.VideoDecoder.Renderer.Present(vFrame);
            if (!MainDemuxer.IsHLSLive && _seeks.IsEmpty)
            {
                curTime = (long) (vFrame.timestamp * Speed);

                if (Config.Player.UICurTimePerFrame)
                    UI(() => UpdateCurTime());
            }
                
            VideoDecoder.Frames.TryDequeue(out vFrame);
            if (vFrame != null)
                vFrame.timestamp = (long) (vFrame.timestamp / Speed);
        }
    }
}

public class BufferingCompletedArgs : EventArgs
{
    public string   Error       { get; }
    public bool     Success     { get; }
        
    public BufferingCompletedArgs(string error)
    {
        Error   = error;
        Success = Error == null;
    }
}
