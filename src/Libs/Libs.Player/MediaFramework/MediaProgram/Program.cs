// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Bili.Copilot.Libs.Player.MediaFramework.MediaDemuxer;
using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;
using FFmpeg.AutoGen;
using static FFmpeg.AutoGen.ffmpeg;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaProgram;

/// <summary>
/// 表示一个程序.
/// </summary>
public class Program
{
    /// <summary>
    /// 使用 AVProgram 指针和解复用器创建 Program 实例.
    /// </summary>
    /// <param name="program">AVProgram 指针.</param>
    /// <param name="demuxer">解复用器.</param>
    public unsafe Program(AVProgram* program, Demuxer demuxer)
    {
        ProgramNumber = program->program_num;
        ProgramId = program->id;

        // 加载流信息
        List<StreamBase> streams = new(3);
        for (var s = 0; s < program->nb_stream_indexes; s++)
        {
            var streamIndex = program->stream_index[s];
            StreamBase stream = null;
            stream = demuxer.AudioStreams.FirstOrDefault(it => it.StreamIndex == streamIndex);

            if (stream == null)
            {
                stream = demuxer.VideoStreams.FirstOrDefault(it => it.StreamIndex == streamIndex);
                stream ??= demuxer.SubtitlesStreams.FirstOrDefault(it => it.StreamIndex == streamIndex);
            }

            if (stream != null)
            {
                streams.Add(stream);
            }
        }

        Streams = streams;

        // 加载元数据
        Dictionary<string, string> metadata = new();
        AVDictionaryEntry* b = null;
        while (true)
        {
            b = av_dict_get(program->metadata, string.Empty, b, AV_DICT_IGNORE_SUFFIX);
            if (b == null)
            {
                break;
            }

            metadata.Add(Utils.BytePtrToStringUtf8(b->key), Utils.BytePtrToStringUtf8(b->value));
        }

        Metadata = metadata;
    }

    /// <summary>
    /// 获取或设置程序编号.
    /// </summary>
    public int ProgramNumber { get; internal set; }

    /// <summary>
    /// 获取或设置程序 ID.
    /// </summary>
    public int ProgramId { get; internal set; }

    /// <summary>
    /// 获取程序的元数据.
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; internal set; }

    /// <summary>
    /// 获取程序的流列表.
    /// </summary>
    public IReadOnlyList<StreamBase> Streams { get; internal set; }

    /// <summary>
    /// 获取程序的名称.
    /// </summary>
    public string Name => Metadata.ContainsKey("name") ? Metadata["name"] : string.Empty;
}
