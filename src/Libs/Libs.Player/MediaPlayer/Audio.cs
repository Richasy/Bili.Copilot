// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Player.Core;
using Bili.Copilot.Libs.Player.MediaFramework.MediaFrame;
using Bili.Copilot.Libs.Player.Models;
using Vortice.Multimedia;
using static Bili.Copilot.Libs.Player.Misc.Logger;
using static Vortice.XAudio2.XAudio2;

namespace Bili.Copilot.Libs.Player.MediaPlayer;

/// <summary>
/// 音频类.
/// </summary>
public partial class Audio : ObservableObject
{
    /// <summary>
    /// 构造函数，初始化音频对象.
    /// </summary>
    /// <param name="player">播放器对象.</param>
    public Audio(Player player)
    {
        _player = player;

        _uiAction = () =>
        {
            IsOpened = IsOpened;
            Codec = Codec;
            BitRate = BitRate;
            Bits = Bits;
            Channels = Channels;
            ChannelLayout = ChannelLayout;
            SampleFormat = SampleFormat;
            SampleRate = SampleRate;

            FramesDisplayed = FramesDisplayed;
            FramesDropped = FramesDropped;
        };

        Volume = Config.Player.VolumeMax / 2;
        Initialize();
    }

    /// <summary>
    /// 音频帧添加事件
    /// </summary>
    public event EventHandler<AudioFrame> SamplesAdded;

    /// <summary>
    /// 延迟增加.
    /// </summary>
    public void DelayAdd() => Config.Audio.Delay += Config.Player.AudioDelayOffset;

    /// <summary>
    /// 延迟增加2.
    /// </summary>
    public void DelayAdd2() => Config.Audio.Delay += Config.Player.AudioDelayOffset2;

    /// <summary>
    /// 延迟减少.
    /// </summary>
    public void DelayRemove() => Config.Audio.Delay -= Config.Player.AudioDelayOffset;

    /// <summary>
    /// 延迟减少2.
    /// </summary>
    public void DelayRemove2() => Config.Audio.Delay -= Config.Player.AudioDelayOffset2;

    /// <summary>
    /// 切换音频状态.
    /// </summary>
    public void Toggle() => Config.Audio.Enabled = !Config.Audio.Enabled;

    /// <summary>
    /// 切换静音状态.
    /// </summary>
    public void ToggleMute() => Mute = !Mute;

    /// <summary>
    /// 音量增加.
    /// </summary>
    public void VolumeUp()
    {
        if (Volume == Config.Player.VolumeMax)
        {
            return;
        }

        Volume = Math.Min(Volume + Config.Player.VolumeOffset, Config.Player.VolumeMax);
    }

    /// <summary>
    /// 音量减少.
    /// </summary>
    public void VolumeDown()
    {
        if (Volume == 0)
        {
            return;
        }

        Volume = Math.Max(Volume - Config.Player.VolumeOffset, 0);
    }

    /// <summary>
    /// 重新加载滤镜（实验性）.
    /// </summary>
    /// <returns>成功返回0.</returns>
    public int ReloadFilters() => _player.AudioDecoder.ReloadFilters();

    /// <summary>
    /// 更新滤镜属性（实验性）
    /// 注意：这不会更新Config.Audio.Filters中的属性值.
    /// </summary>
    /// <param name="filterId">滤镜的唯一标识符，指定在Config.Audio.Filters中.</param>
    /// <param name="key">要更改的滤镜属性.</param>
    /// <param name="value">滤镜属性值.</param>
    /// <returns>成功返回0.</returns>
    public int UpdateFilter(string filterId, string key, string value) => _player.AudioDecoder.UpdateFilter(filterId, key, value);

    /// <summary>
    /// 初始化音频对象.
    /// </summary>
    internal void Initialize()
    {
        lock (_locker)
        {
            if (Engine.Audio.Failed)
            {
                Config.Audio.Enabled = false;
                return;
            }

            _sampleRate = Decoder != null && Decoder.AudioStream != null && Decoder.AudioStream.SampleRate > 0 ? Decoder.AudioStream.SampleRate : 48000;
            _player.Log.Info($"Initialiazing audio ({Device} @ {SampleRate}Hz)");

            Dispose();

            try
            {
                _xaudio2 = XAudio2Create();

                try
                {
                    _masteringVoice = _xaudio2.CreateMasteringVoice(0, 0, AudioStreamCategory.GameEffects, _device == Engine.Audio.DefaultDeviceName ? null : Engine.Audio.GetDeviceId(_device));
                }
                catch (Exception)
                {
                    _masteringVoice = _xaudio2.CreateMasteringVoice(0, 0, AudioStreamCategory.GameEffects, _device == Engine.Audio.DefaultDeviceName ? null : (@"\\?\swd#mmdevapi#" + Engine.Audio.GetDeviceId(_device).ToLower() + @"#{e6327cad-dcec-4949-ae8a-991e976a79d2}"));
                }

                _sourceVoice = _xaudio2.CreateSourceVoice(_waveFormat, false);
                _sourceVoice.SetSourceSampleRate(SampleRate);
                _sourceVoice.Start();

                _submittedSamples = 0;
                _deviceDelayTimebase = 1000 * 10000.0 / _sampleRate;
                _masteringVoice.Volume = Config.Player.VolumeMax / 100.0f;
                _sourceVoice.Volume = _mute ? 0 : Math.Max(0, _volume / 100.0f);
            }
            catch (Exception e)
            {
                _player.Log.Info($"Audio initialization failed ({e.Message})");
                Config.Audio.Enabled = false;
            }
        }
    }

    /// <summary>
    /// 释放音频对象.
    /// </summary>
    internal void Dispose()
    {
        lock (_locker)
        {
            if (_xaudio2 == null)
            {
                return;
            }

            _xaudio2.Dispose();
            _sourceVoice?.Dispose();
            _masteringVoice?.Dispose();
            _xaudio2 = null;
            _sourceVoice = null;
            _masteringVoice = null;
        }
    }

    [System.Security.SecurityCritical]
    /// <summary>
    /// 添加音频帧.
    /// </summary>
    /// <param name="aFrame">音频帧对象.</param>
    internal void AddSamples(AudioFrame aFrame)
    {
        try
        {
            var bufferedMs = (int)((_submittedSamples - _sourceVoice.State.SamplesPlayed) * _deviceDelayTimebase / 10000);

            if (bufferedMs > 30)
            {
                if (CanDebug)
                {
                    _player.Log.Debug($"Audio desynced by {bufferedMs}ms, clearing buffers");
                }

                ClearBuffer();
            }

            _submittedSamples += (ulong)(aFrame.DataLen / 4); // ASampleBytes
            SamplesAdded?.Invoke(this, aFrame);

            _audioBuffer.AudioDataPointer = aFrame.DataPtr;
            _audioBuffer.AudioBytes = aFrame.DataLen;
            _sourceVoice.SubmitSourceBuffer(_audioBuffer);
        }
        catch (Exception e) // Happens on audio device changed/removed
        {
            if (CanDebug)
            {
                _player.Log.Debug($"[Audio] Submitting samples failed ({e.Message})");
            }

            ClearBuffer();
        }
    }

    /// <summary>
    /// 获取设备延迟.
    /// </summary>
    /// <returns>设备延迟时间.</returns>
    internal long GetDeviceDelay() => (long)((_xaudio2.PerformanceData.CurrentLatencyInSamples * _deviceDelayTimebase) - 80000); // TODO: VBlack delay (8ms correction for now)

    /// <summary>
    /// 清空缓冲区.
    /// </summary>
    internal void ClearBuffer()
    {
        lock (_locker)
        {
            if (_sourceVoice == null)
            {
                return;
            }

            _sourceVoice.Stop();
            _sourceVoice.FlushSourceBuffers();
            _sourceVoice.Start();
            _submittedSamples = _sourceVoice.State.SamplesPlayed;
        }
    }

    /// <summary>
    /// 重置音频对象.
    /// </summary>
    internal void Reset()
    {
        _codec = null;
        _bitRate = 0;
        _bits = 0;
        _channels = 0;
        _channelLayout = null;
        _sampleFormat = null;
        _isOpened = false;

        ClearBuffer();
        _player.UIAdd(_uiAction);
    }

    /// <summary>
    /// 刷新音频对象.
    /// </summary>
    internal void Refresh()
    {
        if (Decoder.AudioStream == null)
        {
            Reset();
            return;
        }

        _codec = Decoder.AudioStream.Codec;
        _bits = Decoder.AudioStream.Bits;
        _channels = Decoder.AudioStream.Channels;
        _channelLayout = Decoder.AudioStream.ChannelLayoutStr;
        _sampleFormat = Decoder.AudioStream.SampleFormatStr;
        _isOpened = !Decoder.AudioDecoder.Disposed;

        _framesDisplayed = 0;
        _framesDropped = 0;

        if (SampleRate != Decoder.AudioStream.SampleRate)
        {
            Initialize();
        }

        _player.UIAdd(_uiAction);
    }

    /// <summary>
    /// 启用音频.
    /// </summary>
    internal void Enable()
    {
        var wasPlaying = _player.IsPlaying;

        Decoder.OpenSuggestedAudio();

        _player.ReSync(Decoder.AudioStream, (int)(_player.CurTime / 10000), true);

        Refresh();
        _player.UIAll();

        if (wasPlaying || Config.Player.AutoPlay)
        {
            _player.Play();
        }
    }

    /// <summary>
    /// 禁用音频.
    /// </summary>
    internal void Disable()
    {
        if (!IsOpened)
        {
            return;
        }

        Decoder.CloseAudio();

        _player.AudioFrame = null;

        if (!_player.Video.IsOpened)
        {
            _player.CanPlay = false;
            _player.UIAdd(() => _player.CanPlay = _player.CanPlay);
        }

        Reset();
        _player.UIAll();
    }
}
