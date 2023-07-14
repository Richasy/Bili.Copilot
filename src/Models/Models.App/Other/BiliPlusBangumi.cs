// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.App.Other;

/// <summary>
/// Bili Plus 返回的动漫信息.
/// </summary>
public class BiliPlusBangumi
{
    /// <summary>
    /// 剧集 Id.
    /// </summary>
    [JsonPropertyName("season_id")]
    public string SeasonId { get; set; }

    /// <summary>
    /// 是否需要调转，1-需要，0-不需要.
    /// </summary>
    [JsonPropertyName("is_jump")]
    public int IsJump { get; set; }

    /// <summary>
    /// 剧集名.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 封面.
    /// </summary>
    [JsonPropertyName("cover")]
    public string Cover { get; set; }

    /// <summary>
    /// 是否已完结.
    /// </summary>
    [JsonPropertyName("is_finish")]
    public string IsFinish { get; set; }

    /// <summary>
    /// 播放地址.
    /// </summary>
    [JsonPropertyName("ogv_play_url")]
    public string PlayUrl { get; set; }
}

