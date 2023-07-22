// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Player.MediaFramework.MediaDemuxer;
using FFmpeg.AutoGen;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaStream;

/// <summary>
/// 字幕流类，继承自 StreamBase 类.
/// </summary>
public unsafe class SubtitlesStream : StreamBase
{
    /// <summary>
    /// 默认构造函数.
    /// </summary>
    public SubtitlesStream()
    {
    }

    /// <summary>
    /// 构造函数，接受 Demuxer 和 AVStream 指针作为参数，并调用基类的 Refresh 方法.
    /// </summary>
    /// <param name="demuxer">解复用器对象.</param>
    /// <param name="st">AVStream 指针.</param>
    public SubtitlesStream(Demuxer demuxer, AVStream* st)
        : base(demuxer, st)
        => Refresh();

    /// <summary>
    /// 获取流的详细信息.
    /// </summary>
    /// <returns>流的详细信息字符串.</returns>
    public override string GetDump()
        => $"[{Type}  #{StreamIndex}-{Language.IdSubLanguage}{(Title != null ? "(" + Title + ")" : string.Empty)}] {Codec} | [BR: {BitRate}] | {Utils.TicksToTime((long)(AVStream->start_time * TimeBase))}/{Utils.TicksToTime((long)(AVStream->duration * TimeBase))} | {Utils.TicksToTime(StartTime)}/{Utils.TicksToTime(Duration)}";
}
