// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Player.Enums;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.Libs.Player.Core.Configs;

/// <summary>
/// 解码器配置类，用于配置解码器的参数和选项.
/// </summary>
public sealed partial class DecoderConfig : ObservableObject
{
    private MediaPlayer.Player _player;
    private int _maxVideoFrames = 4;
    private ZeroCopyState _zeroCopy = ZeroCopyState.Auto;

    /// <summary>
    /// 是否允许即使编解码器的配置不匹配也进行视频加速.
    /// </summary>
    [ObservableProperty]
    private bool _allowProfileMismatch;

    /// <summary>
    /// 是否显示损坏的帧（解析 AV_CODEC_FLAG_OUTPUT_CORRUPT 到 AVCodecContext）.
    /// </summary>
    [ObservableProperty]
    private bool _showCorrupted;

    /// <summary>
    /// 用于解码的线程数.
    /// </summary>
    public int VideoThreads { get; set; } = Environment.ProcessorCount;

    /// <summary>
    /// 最大视频帧数，用于解码和渲染.
    /// </summary>
    public int MaxVideoFrames
    {
        get => _maxVideoFrames;
        set
        {
            if (SetProperty(ref _maxVideoFrames, value))
            {
                _player?.RefreshMaxVideoFrames();
            }
        }
    }

    /// <summary>
    /// 最大音频帧数，用于解码和播放.
    /// </summary>
    public int MaxAudioFrames { get; set; } = 10;

    /// <summary>
    /// 最大字幕帧数，用于解码.
    /// </summary>
    public int MaxSubsFrames { get; set; } = 2;

    /// <summary>
    /// 在停止之前允许的最大错误数.
    /// </summary>
    public int MaxErrors { get; set; } = 200;

    /// <summary>
    /// 是否直接使用解码器的纹理作为着色器资源.
    /// （性能更好，但在视频输入有填充或不受旧版 Direct3D 支持时可能需要禁用）.
    /// </summary>
    public ZeroCopyState ZeroCopy
    {
        get => _zeroCopy;
        set
        {
            if (SetProperty(ref _zeroCopy, value) && _player != null && _player.Video.IsOpened)
            {
                _player.VideoDecoder?.RecalculateZeroCopy();
            }
        }
    }

    /// <summary>
    /// 克隆解码器配置对象.
    /// </summary>
    /// <returns>克隆后的解码器配置对象.</returns>
    public DecoderConfig Clone()
    {
        var decoder = (DecoderConfig)MemberwiseClone();
        decoder._player = null;

        return decoder;
    }

    internal void SetPlayer(MediaPlayer.Player player)
        => _player = player;
}
