// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.Models;

namespace Bili.Copilot.Libs.Player.Core.Configs;

/// <summary>
/// 字幕配置类.
/// </summary>
public sealed class SubtitleConfig : ObservableObject
{
    private MediaPlayer.Player _player;
    private long _delay;
    private bool _enabled = true;
    private List<Language> _languages;
    private bool _searchLocal = false;
    private bool _searchOnline = false;

    /// <summary>
    /// 字幕延迟的时钟周期数（每次新的字幕流都会重置为0）.
    /// </summary>
    public long Delay
    {
        get => _delay;
        set
        {
            if (_player != null && !_player.Subtitles.IsOpened)
            {
                return;
            }

            if (Set(ref _delay, value))
            {
                _player?.ReSync(_player.Decoder.SubtitlesStream);
            }
        }
    }

    /// <summary>
    /// 是否允许显示字幕.
    /// </summary>
    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (Set(ref _enabled, value))
            {
                if (value)
                {
                    _player?.Subtitles.Enable();
                }
                else
                {
                    _player?.Subtitles.Disable();
                }
            }
        }
    }

    /// <summary>
    /// 字幕语言的优先级列表.
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
    /// 是否使用本地搜索插件（参见 <see cref="SearchLocalOnInputType"/>）.
    /// </summary>
    public bool SearchLocal { get => _searchLocal; set => Set(ref _searchLocal, value); }

    /// <summary>
    /// 允许在本地搜索字幕的输入类型列表（空列表表示允许所有类型）.
    /// </summary>
    public List<InputType> SearchLocalOnInputType { get; set; } = new() { InputType.File, InputType.UNC, InputType.Torrent };

    /// <summary>
    /// 是否使用在线搜索插件（参见 <see cref="SearchOnlineOnInputType"/>）.
    /// </summary>
    public bool SearchOnline { get => _searchOnline; set => Set(ref _searchOnline, value); }

    /// <summary>
    /// 允许在在线搜索字幕的输入类型列表（空列表表示允许所有类型）.
    /// </summary>
    public List<InputType> SearchOnlineOnInputType { get; set; } = new() { InputType.File, InputType.Torrent };

    /// <summary>
    /// 字幕解析器（可用于自定义解析）.
    /// </summary>
    public Action<SubtitleFrame> Parser { get; set; } = ParseSubtitles.Parse;

    /// <summary>
    /// 克隆当前的字幕配置.
    /// </summary>
    /// <returns>克隆后的字幕配置.</returns>
    public SubtitleConfig Clone()
    {
        SubtitleConfig subs = new();
        subs = (SubtitleConfig)MemberwiseClone();

        subs.Languages = new List<Language>();
        if (Languages != null)
        {
            foreach (var lang in Languages)
            {
                subs.Languages.Add(lang);
            }
        }

        subs._player = null;

        return subs;
    }

    internal void SetEnabled(bool enabled)
        => Set(ref _enabled, enabled, true, nameof(Enabled));

    internal void SetDelay(long delay)
        => Set(ref _delay, delay, true, nameof(Delay));

    internal void SetPlayer(MediaPlayer.Player player)
        => _player = player;
}
