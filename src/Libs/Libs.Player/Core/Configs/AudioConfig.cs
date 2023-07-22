// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using Bili.Copilot.Libs.Player.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.Libs.Player.Core.Configs;

/// <summary>
/// 音频配置类.
/// </summary>
public sealed partial class AudioConfig : ObservableObject
{
    private MediaPlayer.Player _player;
    private long _delay;
    private bool _enabled = true;
    private bool _filtersEnabled = false;
    private List<Language> _languages;

    /// <summary>
    /// 音频延迟的时钟周期数（每次新的音频流都会重置为0）.
    /// </summary>
    public long Delay
    {
        get => _delay;
        set
        {
            if (_player != null && !_player.Audio.IsOpened)
            {
                return;
            }

            if (SetProperty(ref _delay, value))
            {
                _player?.ReSync(_player.Decoder.AudioStream);
            }
        }
    }

    /// <summary>
    /// 是否允许播放音频.
    /// </summary>
    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (SetProperty(ref _enabled, value))
            {
                if (value)
                {
                    _player?.Audio.Enable();
                }
                else
                {
                    _player?.Audio.Disable();
                }
            }
        }
    }

    /// <summary>
    /// 是否使用滤镜或 SWR 处理音频样本（实验性）.
    /// 1. 需要 FFmpeg avfilter 库.
    /// 2. 如果不需要滤镜，SWR 的性能更好.
    /// </summary>
    public bool FiltersEnabled
    {
        get => _filtersEnabled;
        set
        {
            if (SetProperty(ref _filtersEnabled, value && Engine.FFmpeg.FiltersLoaded))
            {
                _player?.AudioDecoder.SetupFiltersOrSwr();
            }
        }
    }

    /// <summary>
    /// 后处理音频样本的滤镜列表（实验性）.
    /// （需要 FiltersEnabled 为 true）.
    /// </summary>
    public List<Filter> Filters { get; set; }

    /// <summary>
    /// 音频语言的优先级列表.
    /// </summary>
    public List<Language> Languages
    {
        get
        {
            _languages ??= Utils.GetSystemLanguages();
            return _languages;
        }
        set => _languages = value;
    }

    /// <summary>
    /// 克隆当前音频配置对象.
    /// </summary>
    /// <returns>克隆后的音频配置对象.</returns>
    public AudioConfig Clone()
    {
        var audio = (AudioConfig)MemberwiseClone();
        audio._player = null;

        return audio;
    }

    internal void SetEnabled(bool enabled)
        => SetProperty(ref _enabled, enabled, nameof(Enabled));

    internal void SetDelay(long delay)
        => SetProperty(ref _delay, delay, nameof(Delay));

    internal void SetPlayer(MediaPlayer.Player player)
        => _player = player;
}
