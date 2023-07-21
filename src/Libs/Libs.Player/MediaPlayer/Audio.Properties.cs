// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using Bili.Copilot.Libs.Player.Core;
using Bili.Copilot.Libs.Player.MediaFramework.MediaContext;
using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;
using Vortice.Multimedia;
using Vortice.XAudio2;

namespace Bili.Copilot.Libs.Player.MediaPlayer;

/// <summary>
/// 音频处理.
/// </summary>
public partial class Audio
{
    private readonly object _locker = new();
    private readonly WaveFormat _waveFormat = new(48000, 16, 2);
    private readonly AudioBuffer _audioBuffer = new();

    private string _deviceId = Engine.Audio.DefaultDeviceId;
    private string _codec;
    private bool _isOpened;
    private double _bitRate;
    private int _bits;
    private int _channels;
    private string _channelLayout;
    private int _framesDropped;
    private int _sampleRate;
    private int _framesDisplayed;
    private string _sampleFormat;
    private int _volume;
    private bool _mute = false;
    private string _device = Engine.Audio.DefaultDeviceName;

    private Player _player;
    private Action _uiAction;

    private IXAudio2 _xaudio2;
    private IXAudio2MasteringVoice _masteringVoice;
    private IXAudio2SourceVoice _sourceVoice;
    private double _deviceDelayTimebase;
    private ulong _submittedSamples;

    /// <summary>
    /// 嵌入的音频流.
    /// </summary>
    public ObservableCollection<AudioStream> Streams => Decoder?.VideoDemuxer.AudioStreams;

    /// <summary>
    /// 输入是否有音频并且已配置.
    /// </summary>
    public bool IsOpened { get => _isOpened; internal set => Set(ref _isOpened, value); }

    /// <summary>
    /// 音频编解码器.
    /// </summary>
    public string Codec { get => _codec; internal set => Set(ref _codec, value); }

    /// <summary>
    /// 音频比特率（Kbps）.
    /// </summary>
    public double BitRate
    {
        get => _bitRate;
        internal set => Set(ref _bitRate, value);
    }

    /// <summary>
    /// 比特.
    /// </summary>
    public int Bits
    {
        get => _bits; internal
            set => Set(ref _bits, value);
    }

    /// <summary>
    /// 频道数量.
    /// </summary>
    public int Channels
    {
        get => _channels;
        internal set => Set(ref _channels, value);
    }

    /// <summary>
    /// 音频播放器的输出通道（目前仅支持2个通道）.
    /// </summary>
    public int ChannelsOut { get; } = 2;

    /// <summary>
    /// 输出通道布局.
    /// </summary>
    public string ChannelLayout
    {
        get => _channelLayout;
        internal set => Set(ref _channelLayout, value);
    }

    /// <summary>
    /// 总丢帧数.
    /// </summary>
    public int FramesDropped
    {
        get => _framesDropped;
        internal set => Set(ref _framesDropped, value);
    }

    /// <summary>
    /// 总显示帧数.
    /// </summary>
    public int FramesDisplayed
    {
        get => _framesDisplayed;
        internal set => Set(ref _framesDisplayed, value);
    }

    /// <summary>
    /// 音频采样格式（输入/输出）.
    /// </summary>
    public string SampleFormat
    {
        get => _sampleFormat;
        internal set => Set(ref _sampleFormat, value);
    }

    /// <summary>
    /// 音频采样率（输入/输出）.
    /// </summary>
    public int SampleRate
    {
        get => _sampleRate;
        internal set => Set(ref _sampleRate, value);
    }

    /// <summary>
    /// 音频播放器的音量/放大器（有效值为0-无上限）.
    /// </summary>
    public int Volume
    {
        get
        {
            lock (_locker)
            {
                return _sourceVoice == null || Mute ? _volume : (int)((decimal)_sourceVoice.Volume * 100);
            }
        }

        set
        {
            if (value > Config.Player.VolumeMax || value < 0)
            {
                return;
            }

            if (value == 0)
            {
                Mute = true;
            }
            else if (Mute)
            {
                _volume = value;
                Mute = false;
            }
            else
            {
                if (_sourceVoice != null)
                {
                    _sourceVoice.Volume = Math.Max(0, value / 100.0f);
                }
            }

            Set(ref _volume, value, false);
        }
    }

    /// <summary>
    /// 音频播放器的静音状态.
    /// </summary>
    public bool Mute
    {
        get => _mute;
        set
        {
            lock (_locker)
            {
                if (_sourceVoice == null)
                {
                    return;
                }

                _sourceVoice.Volume = value ? 0 : _volume / 100.0f;
            }

            Set(ref _mute, value, false);
        }
    }

    /// <summary>
    /// 音频播放器的当前设备（可在Engine.Audio.Devices中找到可用设备）.
    /// </summary>
    public string Device
    {
        get => _device;
        set
        {
            if (value == null || _device == value)
            {
                return;
            }

            _device = value;
            _deviceId = Engine.Audio.GetDeviceId(value);

            Initialize();

            Utils.UI(() => Raise(nameof(Device)));
        }
    }

    /// <summary>
    /// 音频设备的ID.
    /// </summary>
    public string DeviceId
    {
        get => _deviceId;
        set
        {
            if (value == null || _deviceId == value)
            {
                return;
            }

            _deviceId = value;
            _device = Engine.Audio.GetDeviceName(value);

            Initialize();

            Utils.UI(() => Raise(nameof(DeviceId)));
        }
    }

    internal Config Config => _player.Config;

    internal DecoderContext Decoder => _player?.Decoder;

    internal void RaiseDevice() => Utils.UI(() => Raise(nameof(Device)));
}
