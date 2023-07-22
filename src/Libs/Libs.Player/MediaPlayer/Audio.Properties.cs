// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using Bili.Copilot.Libs.Player.Core;
using Bili.Copilot.Libs.Player.MediaFramework.MediaContext;
using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;
using CommunityToolkit.Mvvm.ComponentModel;
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

    private readonly Player _player;
    private readonly Action _uiAction;

    /// <summary>
    /// 音频编解码器.
    /// </summary>
    [ObservableProperty]
    private string _codec;

    /// <summary>
    /// 输入是否有音频并且已配置.
    /// </summary>
    [ObservableProperty]
    private bool _isOpened;

    /// <summary>
    /// 音频比特率（Kbps）.
    /// </summary>
    [ObservableProperty]
    private double _bitRate;

    /// <summary>
    /// 比特.
    /// </summary>
    [ObservableProperty]
    private int _bits;

    /// <summary>
    /// 频道数量.
    /// </summary>
    [ObservableProperty]
    private int _channels;

    /// <summary>
    /// 输出通道布局.
    /// </summary>
    [ObservableProperty]
    private string _channelLayout;

    /// <summary>
    /// 总丢帧数.
    /// </summary>
    [ObservableProperty]
    private int _framesDropped;

    /// <summary>
    /// 音频采样率（输入/输出）.
    /// </summary>
    [ObservableProperty]
    private int _sampleRate;

    /// <summary>
    /// 总显示帧数.
    /// </summary>
    [ObservableProperty]
    private int _framesDisplayed;

    /// <summary>
    /// 音频采样格式（输入/输出）.
    /// </summary>
    [ObservableProperty]
    private string _sampleFormat;

    private int _volume;
    private bool _mute = false;
    private string _device = Engine.Audio.DefaultDeviceName;
    private string _deviceId = Engine.Audio.DefaultDeviceId;

    private IXAudio2 _xAudio2;
    private IXAudio2SourceVoice _sourceVoice;
    private double _deviceDelayTimeBase;
    private ulong _submittedSamples;

    /// <summary>
    /// 嵌入的音频流.
    /// </summary>
    public ObservableCollection<AudioStream> Streams => Decoder?.VideoDemuxer.AudioStreams;

    /// <summary>
    /// 主音频流.
    /// </summary>
    public IXAudio2MasteringVoice MasteringVoice { get; private set; }

    /// <summary>
    /// 音频播放器的输出通道（目前仅支持2个通道）.
    /// </summary>
    public int ChannelsOut { get; } = 2;

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

            SetProperty(ref _volume, value);
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

            SetProperty(ref _mute, value);
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

            Utils.UI(() => OnPropertyChanged(nameof(Device)));
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

            Utils.UI(() => OnPropertyChanged(nameof(DeviceId)));
        }
    }

    internal Config Config => _player.Config;

    internal DecoderContext Decoder => _player?.Decoder;

    internal void RaiseDevice() => Utils.UI(() => OnPropertyChanged(nameof(Device)));
}
