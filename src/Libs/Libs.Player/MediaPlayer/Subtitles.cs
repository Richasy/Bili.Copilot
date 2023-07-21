// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using Bili.Copilot.Libs.Player.Core;
using Bili.Copilot.Libs.Player.MediaFramework.MediaContext;
using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;
using Bili.Copilot.Libs.Player.Models;

namespace Bili.Copilot.Libs.Player.MediaPlayer;

/// <summary>
/// 字幕类，继承自 ObservableObject.
/// </summary>
public class Subtitles : ObservableObject
{
    private readonly Action _uiAction;
    private readonly Player _player;
    private bool _isOpened;
    private string _codec;
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

    /// <summary>
    /// 输入是否有字幕并且已配置.
    /// </summary>
    public bool IsOpened { get => _isOpened; internal set => Set(ref _isOpened, value); }

    /// <summary>
    /// 字幕编码.
    /// </summary>
    public string Codec { get => _codec; internal set => Set(ref _codec, value); }

    /// <summary>
    /// 字幕文本（根据应该显示的持续时间在播放时动态更新）.
    /// </summary>
    public string SubtitleText { get => _subtitleText; internal set => Set(ref _subtitleText, value); }

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
        _codec = null;
        _isOpened = false;
        _subtitleText = string.Empty;

        _player.UIAdd(_uiAction);
    }

    internal void Refresh()
    {
        if (Decoder.SubtitlesStream == null)
        {
            Reset();
            return;
        }

        _codec = Decoder.SubtitlesStream.Codec;
        _isOpened = !Decoder.SubtitlesDecoder.Disposed;

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
