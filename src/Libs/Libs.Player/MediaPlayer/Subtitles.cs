// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using Bili.Copilot.Libs.Player.Core;
using Bili.Copilot.Libs.Player.MediaFramework.MediaContext;
using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.Libs.Player.MediaPlayer;

/// <summary>
/// 字幕类，继承自 ObservableObject.
/// </summary>
public partial class Subtitles : ObservableObject
{
    private readonly Action _uiAction;
    private readonly Player _player;

    /// <summary>
    /// 输入是否有字幕并且已配置.
    /// </summary>
    [ObservableProperty]
    private bool _isOpened;

    /// <summary>
    /// 字幕编码.
    /// </summary>
    [ObservableProperty]
    private string _codec;

    /// <summary>
    /// 字幕文本（根据应该显示的持续时间在播放时动态更新）.
    /// </summary>
    [ObservableProperty]
    private string _subtitleText = string.Empty;

    /// <summary>
    /// 构造函数.
    /// </summary>
    /// <param name="player">播放器对象.</param>
    public Subtitles(Player player)
    {
        _player = player;
        _uiAction = () =>
        {
            IsOpened = IsOpened;
            Codec = Codec;
            SubtitleText = SubtitleText;
        };
    }

    /// <summary>
    /// 嵌入的流.
    /// </summary>
    public ObservableCollection<SubtitlesStream> Streams => Decoder?.VideoDemuxer.SubtitlesStreams;

    private DecoderContext Decoder => _player?.Decoder;

    private Config Config => _player.Config;

    /// <summary>
    /// 延迟减少字幕显示时间.
    /// </summary>
    public void DelayRemove() => Config.Subtitles.Delay -= Config.Player.SubtitlesDelayOffset;

    /// <summary>
    /// 延迟增加字幕显示时间.
    /// </summary>
    public void DelayAdd() => Config.Subtitles.Delay += Config.Player.SubtitlesDelayOffset;

    /// <summary>
    /// 延迟减少字幕显示时间2.
    /// </summary>
    public void DelayRemove2() => Config.Subtitles.Delay -= Config.Player.SubtitlesDelayOffset2;

    /// <summary>
    /// 延迟增加字幕显示时间2.
    /// </summary>
    public void DelayAdd2() => Config.Subtitles.Delay += Config.Player.SubtitlesDelayOffset2;

    /// <summary>
    /// 切换字幕显示状态.
    /// </summary>
    public void Toggle() => Config.Subtitles.Enabled = !Config.Subtitles.Enabled;

    internal void Reset()
    {
        Codec = null;
        IsOpened = false;
        SubtitleText = string.Empty;

        _player.UIAdd(_uiAction);
    }

    internal void Refresh()
    {
        if (Decoder.SubtitlesStream == null)
        {
            Reset();
            return;
        }

        Codec = Decoder.SubtitlesStream.Codec;
        IsOpened = !Decoder.SubtitlesDecoder.Disposed;

        _player.UIAdd(_uiAction);
    }

    internal void Enable()
    {
        if (!_player.CanPlay)
        {
            return;
        }

        Decoder.OpenSuggestedSubtitles();
        _player.ReSync(Decoder.SubtitlesStream, (int)(_player.CurTime / 10000), true);

        Refresh();
        _player.UIAll();
    }

    internal void Disable()
    {
        if (!IsOpened)
        {
            return;
        }

        Decoder.CloseSubtitles();

        _player.SubtitleFrame = null;
        Reset();
        _player.UIAll();
    }
}
