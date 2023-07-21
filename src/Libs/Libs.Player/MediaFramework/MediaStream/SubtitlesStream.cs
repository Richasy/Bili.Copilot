﻿using FFmpeg.AutoGen;

using Bili.Copilot.Libs.Player.MediaFramework.MediaDemuxer;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaStream;

public unsafe class SubtitlesStream : StreamBase
{
    public override string GetDump()
        => $"[{Type}  #{StreamIndex}-{Language.IdSubLanguage}{(Title != null ? "(" + Title + ")" : "")}] {Codec} | [BR: {BitRate}] | {Utils.TicksToTime((long)(AVStream->start_time * Timebase))}/{Utils.TicksToTime((long)(AVStream->duration * Timebase))} | {Utils.TicksToTime(StartTime)}/{Utils.TicksToTime(Duration)}";

    public SubtitlesStream() { }
    public SubtitlesStream(Demuxer demuxer, AVStream* st) : base(demuxer, st) => base.Refresh();
}
