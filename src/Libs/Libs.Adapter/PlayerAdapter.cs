// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.Data.Player;

namespace Bili.Copilot.Libs.Adapter;

/// <summary>
/// 播放器数据适配器.
/// </summary>
public static class PlayerAdapter
{
    /// <summary>
    /// 将视频格式 <see cref="VideoFormat"/> 转换成格式信息.
    /// </summary>
    /// <param name="format">视频格式.</param>
    /// <returns><see cref="FormatInformation"/>.</returns>
    public static FormatInformation ConvertToFormatInformation(VideoFormat format)
        => new(
            format.Quality,
            TextToolkit.ConvertToTraditionalChineseIfNeeded(format.Description),
            !string.IsNullOrEmpty(format.Superscript));

    /// <summary>
    /// 将视频分段条目 <see cref="DashItem"/> 转换成分片信息.
    /// </summary>
    /// <param name="item">视频分段条目.</param>
    /// <returns><see cref="SegmentInformation"/>.</returns>
    public static SegmentInformation ConvertToSegmentInformation(DashItem item)
    {
        return new SegmentInformation(
            item.Id.ToString(),
            item.BaseUrl,
            item.BackupUrl,
            item.BandWidth,
            item.MimeType,
            item.Codecs,
            item.Width,
            item.Height,
            item.SegmentBase.Initialization,
            item.SegmentBase.IndexRange);
    }

    /// <summary>
    /// 将播放器信息 <see cref="PlayerInformation"/> 转换成媒体播放信息.
    /// </summary>
    /// <param name="information">播放器信息.</param>
    /// <returns><see cref="MediaInformation"/>.</returns>
    public static MediaInformation ConvertToMediaInformation(PlayerInformation information)
    {
        var dash = information.VideoInformation;
        if (dash == null)
        {
            return default;
        }

        var minBuffer = dash.MinBufferTime;
        var videos = dash.Video?.Count > 0
            ? dash.Video.Select(ConvertToSegmentInformation).ToList()
            : null;
        var audios = dash.Audio?.Count > 0
            ? dash.Audio.Select(ConvertToSegmentInformation).ToList()
            : null;
        var formats = information.SupportFormats.Select(ConvertToFormatInformation).ToList();
        return new MediaInformation(minBuffer, videos, audios, formats);
    }

    /// <summary>
    /// 将视频播放器信息 <see cref="PlayerInformation"/> 转换成媒体播放信息.
    /// </summary>
    /// <param name="reply">播放器信息.</param>
    /// <returns><see cref="MediaInformation"/>.</returns>
    public static MediaInformation ConvertToMediaInformation(Bilibili.App.Playurl.V1.PlayViewReply reply)
    {
        var info = reply.VideoInfo;
        if (info == null)
        {
            return default;
        }

        var videoStreams = info.StreamList.ToList();
        var audioStreams = info.DashAudio != null
            ? info.DashAudio.ToList()
            : new List<Bilibili.App.Playurl.V1.DashItem>();
        var videos = new List<SegmentInformation>();
        var audios = new List<SegmentInformation>();
        var formats = new List<FormatInformation>();

        foreach (var video in videoStreams)
        {
            if (video.DashVideo == null)
            {
                continue;
            }

            var seg = new SegmentInformation(
                video.StreamInfo.Quality.ToString(),
                video.DashVideo.BaseUrl,
                video.DashVideo.BackupUrl.ToList(),
                default,
                default,
                video.DashVideo.Codecid.ToString(),
                default,
                default,
                default,
                default);
            var format = new FormatInformation(
                Convert.ToInt32(video.StreamInfo.Quality),
                video.StreamInfo.Description,
                video.StreamInfo.NeedVip);
            videos.Add(seg);
            formats.Add(format);
        }

        foreach (var audio in audioStreams)
        {
            var seg = new SegmentInformation(
                audio.Id.ToString(),
                audio.BaseUrl,
                audio.BackupUrl.ToList(),
                default,
                default,
                audio.Codecid.ToString(),
                default,
                default,
                default,
                default);

            audios.Add(seg);
        }

        return new MediaInformation(default, videos, audios, formats);
    }

    /// <summary>
    /// 将字幕索引条目 <see cref="SubtitleIndexItem"/> 转换成字幕元数据.
    /// </summary>
    /// <param name="item">索引条目.</param>
    /// <returns><see cref="SubtitleMeta"/>.</returns>
    public static SubtitleMeta ConvertToSubtitleMeta(SubtitleIndexItem item)
        => new(item.Id.ToString(), item.DisplayLanguage, item.Url);

    /// <summary>
    /// 将字幕条目 <see cref="SubtitleItem"/> 转换成字幕信息.
    /// </summary>
    /// <param name="item">字幕条目.</param>
    /// <returns><see cref="SubtitleInformation"/>.</returns>
    public static SubtitleInformation ConvertToSubtitleInformation(SubtitleItem item)
        => new(item.From, item.To, item.Content);

    /// <summary>
    /// 将弹幕条目 <see cref="DanmakuElem"/> 转化成弹幕信息.
    /// </summary>
    /// <param name="danmaku">弹幕条目.</param>
    /// <returns><see cref="DanmakuInformation"/>.</returns>
    public static DanmakuInformation ConvertToDanmakuInformation(Bilibili.Community.Service.Dm.V1.DanmakuElem danmaku)
        => new(
            danmaku.Id.ToString(),
            TextToolkit.ConvertToTraditionalChineseIfNeeded(danmaku.Content),
            danmaku.Mode,
            danmaku.Progress / 1000.0,
            danmaku.Color,
            danmaku.Fontsize);

    /// <summary>
    /// 将互动选项 <see cref="InteractionChoice"/> 转换成互动条目信息.
    /// </summary>
    /// <param name="choice">互动选项.</param>
    /// <param name="variables">关联变量.</param>
    /// <returns><see cref="InteractionInformation"/>.</returns>
    public static InteractionInformation ConvertToInteractionInformation(InteractionChoice choice, IEnumerable<InteractionHiddenVariable> variables)
    {
        var id = choice.Id.ToString();
        var condition = choice.Condition ?? string.Empty;
        var partId = choice.PartId.ToString();
        var text = TextToolkit.ConvertToTraditionalChineseIfNeeded(choice.Option);
        var isValid = true;

        if (!string.IsNullOrEmpty(condition) && variables != null)
        {
            var v = variables.FirstOrDefault(p => condition.Contains(p.Id));
            var minString = Regex.Match(condition, ">=([0-9]{1,}[.][0-9]*)").Value.Replace(">=", string.Empty);
            var maxString = Regex.Match(condition, "<=([0-9]{1,}[.][0-9]*)").Value.Replace("<=", string.Empty);
            var min = string.IsNullOrEmpty(minString) ? 0 : Convert.ToDouble(minString);
            var max = string.IsNullOrEmpty(maxString) ? -1 : Convert.ToDouble(maxString);
            isValid = v != null && v.Value >= min && (max == -1 || v.Value <= max);
        }

        return new InteractionInformation(id, condition, partId, text, isValid);
    }
}
